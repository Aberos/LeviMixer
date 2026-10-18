using Mixer.Application.Devices;
using Mixer.Testing.Fakes;
using Mixer.Application.UseCases;

namespace Mixer.Application.Tests.UseCases;

public sealed class StartMixingTests
{
    [Fact]
    public void Starts_with_default_devices_and_one_strip_per_input_plus_pc()
    {
        using var mixer = new MixerFixture();

        var result = mixer.StartMixing.Execute(new StartMixingRequest(null, null, StreamFormat.Default));

        Assert.True(result.IsSuccess);
        Assert.True(mixer.Session.IsRunning);
        Assert.Equal(["in-1", "in-2", "pc"], mixer.Session.State.Channels.Select(channel => channel.Id.Value));
        Assert.Equal(FakeAudioDeviceCatalog.TwoChannelInterfaceInput, mixer.Connections.Current.Request.Input);
    }

    [Fact]
    public void Uses_the_requested_input_device()
    {
        using var mixer = new MixerFixture();

        mixer.StartMixing.Execute(new StartMixingRequest(FakeAudioDeviceCatalog.FourChannelInterfaceInput.Id, null, StreamFormat.Default));

        Assert.Equal(5, mixer.Session.State.Channels.Length);
    }

    [Fact]
    public void Unknown_device_fails_without_opening_a_connection()
    {
        using var mixer = new MixerFixture();

        var result = mixer.StartMixing.Execute(new StartMixingRequest(new AudioDeviceId("missing"), null, StreamFormat.Default));

        Assert.Equal("deviceNotFound", result.Error?.Code);
        Assert.Empty(mixer.Connections.Opened);
    }

    [Fact]
    public void Device_failure_is_reported_as_unavailable()
    {
        using var mixer = new MixerFixture();
        mixer.Connections.FailureReason = "Device busy";

        var result = mixer.StartMixing.Execute(new StartMixingRequest(null, null, StreamFormat.Default));

        Assert.Equal("deviceUnavailable", result.Error?.Code);
        Assert.False(mixer.Session.IsRunning);
    }

    [Fact]
    public void Restarting_closes_the_previous_connection()
    {
        using var mixer = new MixerFixture();
        mixer.StartWithDefaults();
        var first = mixer.Connections.Current;

        mixer.StartWithDefaults();

        Assert.True(first.IsClosed);
        Assert.False(mixer.Connections.Current.IsClosed);
    }

    [Fact]
    public void Stopping_closes_the_connection()
    {
        using var mixer = new MixerFixture();
        mixer.StartWithDefaults();

        new StopMixing(mixer.Session).Execute();

        Assert.True(mixer.Connections.Current.IsClosed);
        Assert.False(mixer.Session.IsRunning);
    }
}
