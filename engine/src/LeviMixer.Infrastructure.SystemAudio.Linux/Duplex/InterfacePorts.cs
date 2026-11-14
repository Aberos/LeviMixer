using System.Collections.Immutable;
using LeviMixer.Application.Devices;
using LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire.Graph;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.Duplex;

internal sealed record InterfacePorts(ImmutableArray<GraphPort> Capture, ImmutableArray<GraphPort> Playback)
{
    private const string SourceClass = "Audio/Source";
    private const string SinkClass = "Audio/Sink";

    public static InterfacePorts Find(GraphSnapshot graph, AudioConnectionRequest request)
    {
        var source = graph.FindNode(SourceClass, request.Input.Name) ?? throw NotFound(request.Input);
        var sink = graph.FindNode(SinkClass, request.Output.Name) ?? throw NotFound(request.Output);
        return new InterfacePorts(graph.AudioPortsOf(source.Id, PortDirection.Output), graph.AudioPortsOf(sink.Id, PortDirection.Input));
    }

    public ImmutableArray<PortLink> LinksTo(ImmutableArray<GraphPort> filterInputs, ImmutableArray<GraphPort> filterOutputs)
    {
        var links = ImmutableArray.CreateBuilder<PortLink>();
        for (var index = 0; index < Math.Min(Capture.Length, filterInputs.Length); index++)
        {
            links.Add(new PortLink(Capture[index], filterInputs[index]));
        }

        for (var index = 0; index < filterOutputs.Length && !Playback.IsEmpty; index++)
        {
            links.Add(new PortLink(filterOutputs[index], Playback[Math.Min(index, Playback.Length - 1)]));
        }

        return links.ToImmutable();
    }

    private static AudioDeviceException NotFound(AudioEndpoint endpoint) =>
        new($"No PipeWire node found for '{endpoint.Name}'.");
}
