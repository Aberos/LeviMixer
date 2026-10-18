using LeviMixer.Domain.Channels;
using LeviMixer.Domain.Levels;
using LeviMixer.Domain.Mixing;

namespace LeviMixer.Domain.Tests.Mixing;

public sealed class MixerStateTests
{
    private static readonly ChannelId Mic = DefaultChannelLayout.InputChannelId(0);
    private static readonly ChannelId Guitar = DefaultChannelLayout.InputChannelId(1);

    [Fact]
    public void Every_unmuted_channel_is_audible_when_nothing_is_soloed()
    {
        var state = DefaultChannelLayout.ForInterface(2);

        Assert.All(state.Channels, channel => Assert.True(state.IsAudible(channel)));
    }

    [Fact]
    public void Muted_channel_is_not_audible()
    {
        var state = DefaultChannelLayout.ForInterface(2).UpdateChannel(Mic, channel => channel with { IsMuted = true });

        Assert.False(state.IsAudible(ChannelOf(state, Mic)));
        Assert.True(state.IsAudible(ChannelOf(state, Guitar)));
    }

    [Fact]
    public void Soloing_a_channel_silences_the_others()
    {
        var state = DefaultChannelLayout.ForInterface(2).UpdateChannel(Mic, channel => channel with { IsSoloed = true });

        Assert.True(state.IsAudible(ChannelOf(state, Mic)));
        Assert.False(state.IsAudible(ChannelOf(state, Guitar)));
        Assert.False(state.IsAudible(ChannelOf(state, DefaultChannelLayout.SystemAudioChannelId)));
    }

    [Fact]
    public void Mute_wins_over_solo()
    {
        var state = DefaultChannelLayout.ForInterface(2)
            .UpdateChannel(Mic, channel => channel with { IsSoloed = true, IsMuted = true });

        Assert.False(state.IsAudible(ChannelOf(state, Mic)));
    }

    [Fact]
    public void Updating_unknown_channel_fails()
    {
        var state = DefaultChannelLayout.ForInterface(2);

        Assert.Throws<KeyNotFoundException>(() => state.UpdateChannel(new ChannelId("missing"), channel => channel));
    }

    [Fact]
    public void Duplicated_channel_ids_are_rejected()
    {
        var strip = new ChannelStrip(Mic, "Mic", new InterfaceInput(0));

        Assert.Throws<ArgumentException>(() => MixerState.Create([strip, strip], MasterBus.AtUnity));
    }

    [Fact]
    public void Master_gain_is_clamped_to_fader_maximum()
    {
        var state = DefaultChannelLayout.ForInterface(2).WithMasterGain(new Decibels(99f));

        Assert.Equal(FaderRange.Maximum, state.Master.Gain);
    }

    [Fact]
    public void Default_layout_has_one_strip_per_input_plus_system_audio()
    {
        var state = DefaultChannelLayout.ForInterface(2);

        Assert.Equal(["in-1", "in-2", "pc"], state.Channels.Select(channel => channel.Id.Value));
        Assert.IsType<SystemAudio>(state.Channels[^1].Source);
    }

    private static ChannelStrip ChannelOf(MixerState state, ChannelId id) => state.Channels.Single(channel => channel.Id == id);
}
