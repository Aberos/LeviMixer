using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;
using LeviMixer.Infrastructure.SystemAudio.Linux.Duplex;
using LeviMixer.Testing.Fakes;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.Tests;

public sealed class PipeWireConnectionFactoryTests
{
    private const string UnknownDevice = "USB Mic";

    [Fact]
    public void Unreadable_graph_falls_back()
    {
        var fallback = new FakeAudioConnectionFactory();

        using var connection = new PipeWireConnectionFactory(fallback, new RecordingCommandRunner("not json")).Open(Request(), new SilentProcessor());

        Assert.Same(fallback.Current, connection);
    }

    [Fact]
    public void Device_missing_from_the_graph_falls_back()
    {
        var fallback = new FakeAudioConnectionFactory();

        using var connection = new PipeWireConnectionFactory(fallback, new RecordingCommandRunner(PipeWireDump.Json)).Open(Request(), new SilentProcessor());

        Assert.Same(fallback.Current, connection);
    }

    private static AudioConnectionRequest Request() =>
        new(new AudioEndpoint(new AudioDeviceId(UnknownDevice), UnknownDevice, 1, true), new AudioEndpoint(new AudioDeviceId(UnknownDevice), UnknownDevice, 2, true), StreamFormat.Default);

    private sealed class SilentProcessor : IAudioBlockProcessor
    {
        public void Process(AudioBlock block) => block.Output.Samples.Clear();
    }
}
