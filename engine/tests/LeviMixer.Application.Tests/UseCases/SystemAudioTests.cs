using LeviMixer.Application.Devices;
using LeviMixer.Application.Session;
using LeviMixer.Application.UseCases;
using LeviMixer.Domain.Mixing;

namespace LeviMixer.Application.Tests.UseCases;

public sealed class SystemAudioTests
{
    [Fact]
    public void Captured_system_audio_reaches_the_output()
    {
        using var mixer = new MixerFixture().StartWithDefaults();
        new SetChannelMute(mixer.Session).Execute(DefaultChannelLayout.InputChannelId(0), true);
        new SetChannelMute(mixer.Session).Execute(DefaultChannelLayout.InputChannelId(1), true);

        mixer.SystemAudio.Started[^1].Push(0.25f, 4_096);

        Assert.Equal(0.25f, mixer.SettledLeftOutputFor(0f), 0.0001f);
    }

    [Fact]
    public void Capture_uses_the_session_stream_format()
    {
        using var mixer = new MixerFixture();
        var format = new StreamFormat(48_000, 256);

        mixer.StartMixing.Execute(new StartMixingRequest(null, null, format));

        Assert.Equal(format, mixer.SystemAudio.Started[^1].Format);
    }

    [Fact]
    public void Capture_failure_is_reported_but_mixing_continues()
    {
        using var mixer = new MixerFixture();
        mixer.SystemAudio.FailureReason = "Device busy";

        var result = mixer.StartMixing.Execute(new StartMixingRequest(null, null, StreamFormat.Default));

        Assert.True(result.IsSuccess);
        Assert.Equal(SystemAudioFailure.Unavailable("Device busy"), mixer.Session.Status.SystemAudioFailure);
    }

    [Fact]
    public void Missing_driver_is_reported_with_its_own_code()
    {
        using var mixer = new MixerFixture();
        mixer.SystemAudio.MissingDriverReason = "Install VB-Cable";

        mixer.StartWithDefaults();

        Assert.Equal("driverMissing", mixer.Session.Status.SystemAudioFailure?.Code);
        Assert.True(mixer.Session.IsRunning);
    }

    [Fact]
    public void Successful_restart_clears_the_previous_failure()
    {
        using var mixer = new MixerFixture();
        mixer.SystemAudio.MissingDriverReason = "Install VB-Cable";
        mixer.StartWithDefaults();
        mixer.SystemAudio.MissingDriverReason = null;

        mixer.StartWithDefaults();

        Assert.Null(mixer.Session.Status.SystemAudioFailure);
    }

    [Fact]
    public void Stopping_stops_the_capture()
    {
        using var mixer = new MixerFixture().StartWithDefaults();

        new StopMixing(mixer.Session).Execute();

        Assert.True(mixer.SystemAudio.Started[^1].IsStopped);
    }

    [Fact]
    public void Failed_device_open_releases_the_capture()
    {
        using var mixer = new MixerFixture();
        mixer.Connections.FailureReason = "busy";

        mixer.StartWithDefaults();

        Assert.True(mixer.SystemAudio.Started[^1].IsStopped);
    }
}
