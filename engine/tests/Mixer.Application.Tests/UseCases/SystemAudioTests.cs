using Mixer.Application.Devices;
using Mixer.Application.UseCases;
using Mixer.Domain.Mixing;

namespace Mixer.Application.Tests.UseCases;

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
    public void Capture_failure_is_reported_but_mixing_continues()
    {
        using var mixer = new MixerFixture();
        mixer.SystemAudio.FailureReason = "Install VB-Cable";

        var result = mixer.StartMixing.Execute(new StartMixingRequest(null, null, StreamFormat.Default));

        Assert.True(result.IsSuccess);
        Assert.Equal("Install VB-Cable", mixer.Session.Status.SystemAudioError);
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
