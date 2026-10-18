using Mixer.Application.UseCases;
using Mixer.Domain.Channels;
using Mixer.Domain.Levels;
using Mixer.Domain.Mixing;

namespace Mixer.Application.Tests.UseCases;

public sealed class ChannelControlTests
{
    private static readonly ChannelId FirstInput = DefaultChannelLayout.InputChannelId(0);
    private static readonly ChannelId SecondInput = DefaultChannelLayout.InputChannelId(1);

    [Fact]
    public void Setting_channel_gain_updates_state_and_audio()
    {
        using var mixer = new MixerFixture().StartWithDefaults();
        new SetChannelMute(mixer.Session).Execute(SecondInput, true);
        new SetChannelPan(mixer.Session).Execute(FirstInput, new Pan(Pan.HardLeft));

        var result = new SetChannelGain(mixer.Session).Execute(FirstInput, new Decibels(-6.0206f));

        Assert.True(result.IsSuccess);
        Assert.Equal(new Decibels(-6.0206f), mixer.Session.State.Channels[0].Gain);
        Assert.Equal(0.5f, mixer.SettledLeftOutputFor(1f), 0.0001f);
    }

    [Fact]
    public void Muting_every_input_silences_the_output()
    {
        using var mixer = new MixerFixture().StartWithDefaults();

        new SetChannelMute(mixer.Session).Execute(FirstInput, true);
        new SetChannelMute(mixer.Session).Execute(SecondInput, true);

        Assert.Equal(0f, mixer.SettledLeftOutputFor(1f));
    }

    [Fact]
    public void Soloing_marks_the_channel_as_soloed()
    {
        using var mixer = new MixerFixture().StartWithDefaults();

        new SetChannelSolo(mixer.Session).Execute(SecondInput, true);

        Assert.True(mixer.Session.State.HasSoloedChannel);
        Assert.False(mixer.Session.State.IsAudible(mixer.Session.State.Channels[0]));
    }

    [Fact]
    public void Unknown_channel_is_reported()
    {
        using var mixer = new MixerFixture().StartWithDefaults();

        var result = new SetChannelGain(mixer.Session).Execute(new ChannelId("in-99"), Decibels.Unity);

        Assert.Equal("unknownChannel", result.Error?.Code);
    }

    [Fact]
    public void Master_gain_at_minus_infinity_silences_the_output()
    {
        using var mixer = new MixerFixture().StartWithDefaults();

        new SetMasterGain(mixer.Session).Execute(Decibels.MinusInfinity);

        Assert.Equal(0f, mixer.SettledLeftOutputFor(1f));
    }

    [Fact]
    public void Meters_are_unavailable_until_mixing_starts()
    {
        using var mixer = new MixerFixture();

        Assert.Null(new ReadMeters(mixer.Session).Execute());

        mixer.StartWithDefaults();
        Assert.NotNull(new ReadMeters(mixer.Session).Execute());
    }
}
