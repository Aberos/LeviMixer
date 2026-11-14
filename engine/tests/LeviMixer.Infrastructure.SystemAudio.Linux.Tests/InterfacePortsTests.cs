using System.Collections.Immutable;
using LeviMixer.Application.Devices;
using LeviMixer.Infrastructure.SystemAudio.Linux.Duplex;
using LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire.Graph;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.Tests;

public sealed class InterfacePortsTests
{
    private static readonly GraphSnapshot Graph = GraphSnapshot.Parse(PipeWireDump.Json);

    private static readonly ImmutableArray<GraphPort> FilterInputs =
    [
        new(80, 90, PortDirection.Input, "in_1", 0, false),
        new(81, 90, PortDirection.Input, "in_2", 1, false),
    ];

    private static readonly ImmutableArray<GraphPort> FilterOutputs =
    [
        new(82, 90, PortDirection.Output, "out_FL", 0, false),
        new(83, 90, PortDirection.Output, "out_FR", 1, false),
    ];

    [Fact]
    public void Finds_capture_and_playback_ports_of_the_chosen_interface()
    {
        var ports = InterfacePorts.Find(Graph, Request(PipeWireDump.InterfaceName, PipeWireDump.InterfaceName));

        Assert.Equal(["capture_FL", "capture_FR"], ports.Capture.Select(port => port.Name));
        Assert.Equal(["playback_FL", "playback_FR"], ports.Playback.Select(port => port.Name));
    }

    [Fact]
    public void Missing_node_is_a_device_error()
    {
        Assert.Throws<AudioDeviceException>(() => InterfacePorts.Find(Graph, Request("Monitor of HDMI", PipeWireDump.InterfaceName)));
    }

    [Fact]
    public void Links_capture_to_filter_inputs_and_filter_outputs_to_playback()
    {
        var ports = InterfacePorts.Find(Graph, Request(PipeWireDump.InterfaceName, PipeWireDump.InterfaceName));

        var links = ports.LinksTo(FilterInputs, FilterOutputs);

        Assert.Equal(["69->80", "70->81", "82->65", "83->66"], links.Select(Describe));
    }

    [Fact]
    public void Mono_output_receives_both_filter_outputs()
    {
        var ports = InterfacePorts.Find(Graph, Request(PipeWireDump.InterfaceName, "HDMI"));

        var links = ports.LinksTo(FilterInputs, FilterOutputs);

        Assert.Equal(["82->62", "83->62"], links.Skip(2).Select(Describe));
    }

    private static AudioConnectionRequest Request(string input, string output) =>
        new(new AudioEndpoint(new AudioDeviceId(input), input, 2, true), new AudioEndpoint(new AudioDeviceId(output), output, 2, true), StreamFormat.Default);

    private static string Describe(PortLink link) => $"{link.From.Id}->{link.To.Id}";
}
