using System.Collections.Immutable;
using LeviMixer.Application.Devices;
using LeviMixer.Application.Settings;
using LeviMixer.Testing.Fakes;
using LeviMixer.Application.UseCases;
using LeviMixer.Domain.Channels;
using LeviMixer.Domain.Levels;
using LeviMixer.Domain.Mixing;

namespace LeviMixer.Application.Tests.UseCases;

public sealed class SessionAndNamingTests
{
    private static readonly ChannelId FirstInput = DefaultChannelLayout.InputChannelId(0);

    [Fact]
    public void Renaming_a_channel_updates_its_name()
    {
        using var mixer = new MixerFixture().StartWithDefaults();

        var result = new RenameChannel(mixer.Session).Execute(FirstInput, "Vocal");

        Assert.True(result.IsSuccess);
        Assert.Equal("Vocal", mixer.Session.State.Channels[0].Name);
    }

    [Fact]
    public void Blank_name_is_rejected()
    {
        using var mixer = new MixerFixture().StartWithDefaults();

        var result = new RenameChannel(mixer.Session).Execute(FirstInput, " ");

        Assert.Equal("invalidChannelName", result.Error?.Code);
    }

    [Fact]
    public void Applying_a_session_starts_mixing_and_restores_channels_and_master()
    {
        using var mixer = new MixerFixture();
        var settings = new SessionSettings(
            FakeAudioDeviceCatalog.TwoChannelInterfaceInput.Id,
            FakeAudioDeviceCatalog.StereoOutput.Id,
            StreamFormat.Default,
            [new ChannelSettings(FirstInput, "Vocal", new Decibels(-3f), new Pan(-0.5f), true, false)],
            new Decibels(-6f));

        var result = new ApplySession(mixer.StartMixing, mixer.Session).Execute(settings);

        var vocal = mixer.Session.State.Channels[0];
        Assert.True(result.IsSuccess);
        Assert.True(mixer.Session.IsRunning);
        Assert.Equal(("Vocal", new Decibels(-3f), new Pan(-0.5f), true), (vocal.Name, vocal.Gain, vocal.Pan, vocal.IsMuted));
        Assert.Equal(new Decibels(-6f), mixer.Session.State.Master.Gain);
    }

    [Fact]
    public void Applying_a_session_ignores_channels_the_device_does_not_have()
    {
        using var mixer = new MixerFixture();
        var settings = new SessionSettings(null, null, StreamFormat.Default,
            [new ChannelSettings(DefaultChannelLayout.InputChannelId(7), "Ghost", Decibels.Unity, Pan.Center, false, false)],
            Decibels.Unity);

        var result = new ApplySession(mixer.StartMixing, mixer.Session).Execute(settings);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(mixer.Session.State.Channels, channel => channel.Name == "Ghost");
    }

    [Fact]
    public void Applying_a_session_with_missing_device_reports_it()
    {
        using var mixer = new MixerFixture();
        var settings = new SessionSettings(new AudioDeviceId("gone"), null, StreamFormat.Default, ImmutableArray<ChannelSettings>.Empty, Decibels.Unity);

        var result = new ApplySession(mixer.StartMixing, mixer.Session).Execute(settings);

        Assert.Equal("deviceNotFound", result.Error?.Code);
    }

    [Fact]
    public void Status_exposes_the_active_connection()
    {
        using var mixer = new MixerFixture();
        Assert.False(new GetMixerStatus(mixer.Session).Execute().IsRunning);

        mixer.StartWithDefaults();

        Assert.Equal(FakeAudioDeviceCatalog.StereoOutput, new GetMixerStatus(mixer.Session).Execute().Connection?.Output);
    }
}
