using LeviMixer.Application.Devices;
using LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire.Graph;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.Tests;

public sealed class PipeWireGraphTests
{
    [Fact]
    public void Links_ports_by_id()
    {
        var runner = new RecordingCommandRunner();
        var from = new GraphPort(69, 46, PortDirection.Output, "capture_FL", 0, false);
        var to = new GraphPort(80, 90, PortDirection.Input, "in_1", 0, false);

        new PipeWireGraph(runner).Connect(new PortLink(from, to));

        Assert.Equal("pw-link 69 80", runner.Commands.Single());
    }

    [Fact]
    public void Unparseable_dump_is_a_device_error()
    {
        Assert.Throws<AudioDeviceException>(() => new PipeWireGraph(new RecordingCommandRunner("not json")).Read());
    }
}
