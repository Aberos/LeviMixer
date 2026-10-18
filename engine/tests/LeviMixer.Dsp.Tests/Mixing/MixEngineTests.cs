using LeviMixer.Domain.Channels;
using LeviMixer.Domain.Levels;
using LeviMixer.Domain.Mixing;
using LeviMixer.Dsp.Buffers;
using LeviMixer.Dsp.Mixing;

namespace LeviMixer.Dsp.Tests.Mixing;

public sealed class MixEngineTests
{
    private static readonly ChannelId Mic = DefaultChannelLayout.InputChannelId(0);
    private static readonly ChannelId Guitar = DefaultChannelLayout.InputChannelId(1);
    private static readonly float CenterGain = MathF.Sqrt(0.5f);

    [Fact]
    public void Mono_input_at_unity_reaches_both_outputs_through_center_pan()
    {
        var harness = new MixEngineHarness(2).With(DefaultChannelLayout.ForInterface(2)).FeedInterfaceChannel(0, 0.5f);

        harness.ProcessUntilSettled();

        Assert.Equal(0.5f * CenterGain, harness.LastLeft, 0.0001f);
        Assert.Equal(0.5f * CenterGain, harness.LastRight, 0.0001f);
    }

    [Fact]
    public void Inputs_and_system_audio_are_summed()
    {
        var harness = new MixEngineHarness(2)
            .With(DefaultChannelLayout.ForInterface(2))
            .FeedInterfaceChannel(0, 0.1f)
            .FeedInterfaceChannel(1, 0.2f)
            .FeedSystemAudio(0.3f, 0.4f);

        harness.ProcessUntilSettled();

        Assert.Equal((0.3f * CenterGain) + 0.3f, harness.LastLeft, 0.0001f);
        Assert.Equal((0.3f * CenterGain) + 0.4f, harness.LastRight, 0.0001f);
    }

    [Fact]
    public void Muted_channel_contributes_silence_once_the_ramp_ends()
    {
        var state = DefaultChannelLayout.ForInterface(2).UpdateChannel(Mic, channel => channel with { IsMuted = true });
        var harness = new MixEngineHarness(2).With(state).FeedInterfaceChannel(0, 1f);

        harness.ProcessUntilSettled();

        Assert.Equal(0f, harness.LastLeft);
        Assert.Equal(0f, harness.LastRight);
    }

    [Fact]
    public void Gain_change_ramps_smoothly_across_the_block()
    {
        var harness = new MixEngineHarness(1)
            .With(DefaultChannelLayout.ForInterface(1).UpdateChannel(Mic, channel => channel with { Pan = new Pan(Pan.HardLeft) }))
            .FeedInterfaceChannel(0, 1f)
            .ProcessUntilSettled();

        harness.With(DefaultChannelLayout.ForInterface(1)
            .UpdateChannel(Mic, channel => channel with { Pan = new Pan(Pan.HardLeft), Gain = Decibels.MinusInfinity }))
            .Process();

        var lefts = Enumerable.Range(0, MixEngineHarness.FrameCount).Select(frame => harness.Output[frame * 2]).ToArray();
        Assert.Equal(1f, lefts[0], 0.0001f);
        Assert.All(lefts.Zip(lefts.Skip(1)), pair => Assert.True(pair.Second < pair.First));
        Assert.True(lefts[^1] < 0.05f);
    }

    [Fact]
    public void Master_gain_scales_the_whole_mix()
    {
        var state = DefaultChannelLayout.ForInterface(1).WithMasterGain(new Decibels(-6.0206f));
        var harness = new MixEngineHarness(1).With(state).FeedInterfaceChannel(0, 1f);

        harness.ProcessUntilSettled();

        Assert.Equal(0.5f * CenterGain, harness.LastLeft, 0.0001f);
    }

    [Fact]
    public void Input_channel_missing_from_device_is_treated_as_silence()
    {
        var harness = new MixEngineHarness(1).With(DefaultChannelLayout.ForInterface(2)).FeedInterfaceChannel(0, 1f);

        harness.ProcessUntilSettled();

        Assert.Equal(CenterGain, harness.LastLeft, 0.0001f);
    }

    [Fact]
    public void Extra_output_channels_are_silent()
    {
        var engine = new MixEngine(new MixEngineCapacity(48_000, 4, 4));
        engine.Apply(MixSnapshot.From(DefaultChannelLayout.ForInterface(1)));
        var output = Enumerable.Repeat(9f, 4 * 4).ToArray();

        engine.Process(new AudioBlock(new InterleavedInput(Enumerable.Repeat(1f, 4).ToArray(), 1), InterleavedInput.None, new InterleavedOutput(output, 4)));

        Assert.All(Enumerable.Range(0, 4), frame => Assert.Equal(0f, output[(frame * 4) + 2]));
        Assert.All(Enumerable.Range(0, 4), frame => Assert.Equal(0f, output[(frame * 4) + 3]));
    }

    [Fact]
    public void Meters_report_post_fader_peak_per_channel_and_master()
    {
        var state = DefaultChannelLayout.ForInterface(2).UpdateChannel(Guitar, channel => channel with { IsMuted = true });
        var harness = new MixEngineHarness(2).With(state).FeedInterfaceChannel(0, 0.5f).FeedInterfaceChannel(1, 0.5f).ProcessUntilSettled();

        var report = harness.Engine.TakeMeterReport();

        Assert.Equal(0.5f * CenterGain, report.Channels.Single(meter => meter.ChannelId == Mic).Reading.Peak, 0.0001f);
        Assert.Equal(0f, report.Channels.Single(meter => meter.ChannelId == Guitar).Reading.Peak);
        Assert.Equal(0.5f * CenterGain, report.MasterLeft.Peak, 0.0001f);
    }

    [Fact]
    public void Taking_a_meter_report_resets_the_peak()
    {
        var harness = new MixEngineHarness(1).With(DefaultChannelLayout.ForInterface(1)).FeedInterfaceChannel(0, 1f).ProcessUntilSettled();
        harness.Engine.TakeMeterReport();

        var report = harness.Engine.TakeMeterReport();

        Assert.Equal(0f, report.MasterLeft.Peak);
    }

    [Fact]
    public void Snapshot_larger_than_capacity_is_rejected()
    {
        var engine = new MixEngine(new MixEngineCapacity(48_000, 64, 2));

        Assert.Throws<ArgumentException>(() => engine.Apply(MixSnapshot.From(DefaultChannelLayout.ForInterface(4))));
    }

    [Fact]
    public void Processing_does_not_allocate()
    {
        var harness = new MixEngineHarness(2)
            .With(DefaultChannelLayout.ForInterface(2))
            .FeedInterfaceChannel(0, 0.5f)
            .FeedSystemAudio(0.2f, 0.2f)
            .ProcessUntilSettled();

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var block = 0; block < 1_000; block++)
        {
            harness.Process();
        }

        Assert.Equal(before, GC.GetAllocatedBytesForCurrentThread());
    }
}
