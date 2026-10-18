using Mixer.Domain.Channels;
using Mixer.Domain.Levels;
using Mixer.Domain.Mixing;
using Mixer.Dsp.Mixing;
using Mixer.Dsp.Routing;

namespace Mixer.Dsp.Tests.Mixing;

public sealed class MixSnapshotTests
{
    private static readonly ChannelId Mic = DefaultChannelLayout.InputChannelId(0);

    [Fact]
    public void Inputs_route_as_mono_and_system_audio_as_stereo()
    {
        var snapshot = MixSnapshot.From(DefaultChannelLayout.ForInterface(2));

        Assert.Equal(StripRoute.FromInterface(0), snapshot.Strips[0].Route);
        Assert.Equal(StripRoute.FromInterface(1), snapshot.Strips[1].Route);
        Assert.Equal(StripRoute.FromSystemAudio(), snapshot.Strips[2].Route);
    }

    [Fact]
    public void Inaudible_channels_get_silent_gain()
    {
        var state = DefaultChannelLayout.ForInterface(2).UpdateChannel(Mic, channel => channel with { IsSoloed = true });

        var snapshot = MixSnapshot.From(state);

        Assert.NotEqual(StereoGain.Silence, snapshot.Strips[0].Gain);
        Assert.Equal(StereoGain.Silence, snapshot.Strips[1].Gain);
        Assert.Equal(StereoGain.Silence, snapshot.Strips[2].Gain);
    }

    [Fact]
    public void Fader_gain_scales_the_pan_gain()
    {
        var state = DefaultChannelLayout.ForInterface(1)
            .UpdateChannel(Mic, channel => channel with { Gain = new Decibels(-6.0206f), Pan = new Pan(Pan.HardLeft) });

        var gain = MixSnapshot.From(state).Strips[0].Gain;

        Assert.Equal(0.5f, gain.Left, 0.0001f);
        Assert.Equal(0f, gain.Right, 0.0001f);
    }

    [Fact]
    public void Master_gain_is_converted_to_linear()
    {
        var state = DefaultChannelLayout.ForInterface(1).WithMasterGain(Decibels.MinusInfinity);

        Assert.Equal(LinearGain.Silence, MixSnapshot.From(state).Master);
    }
}
