using LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire.Graph;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.Tests;

public sealed class GraphSnapshotTests
{
    private readonly GraphSnapshot _graph = GraphSnapshot.Parse(PipeWireDump.Json);

    [Fact]
    public void Reads_nodes_and_ports_and_skips_other_objects()
    {
        Assert.Equal(3, _graph.Nodes.Length);
        Assert.Equal(6, _graph.Ports.Length);
    }

    [Fact]
    public void Finds_a_node_by_media_class_and_description()
    {
        var sink = _graph.FindNode("Audio/Sink", PipeWireDump.InterfaceName);

        Assert.Equal("alsa_output.umc22", sink?.Name);
    }

    [Fact]
    public void Audio_ports_follow_port_order()
    {
        var capture = _graph.AudioPortsOf(46, PortDirection.Output);

        Assert.Equal(["capture_FL", "capture_FR"], capture.Select(port => port.Name));
    }

    [Fact]
    public void Monitor_ports_are_not_audio_ports()
    {
        Assert.Empty(_graph.AudioPortsOf(47, PortDirection.Output));
    }
}
