using System.Collections.Immutable;
using System.Text.Json;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire.Graph;

internal sealed record GraphSnapshot(ImmutableArray<GraphNode> Nodes, ImmutableArray<GraphPort> Ports)
{
    private const string NodeType = "PipeWire:Interface:Node";
    private const string PortType = "PipeWire:Interface:Port";

    public static GraphSnapshot Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        var nodes = ImmutableArray.CreateBuilder<GraphNode>();
        var ports = ImmutableArray.CreateBuilder<GraphPort>();
        foreach (var item in document.RootElement.EnumerateArray())
        {
            if (!TryGetProperties(item, out var properties))
            {
                continue;
            }

            var id = item.GetProperty("id").GetUInt32();
            switch (item.GetProperty("type").GetString())
            {
                case NodeType:
                    nodes.Add(NodeFrom(id, properties));
                    break;
                case PortType:
                    ports.Add(PortFrom(id, properties));
                    break;
            }
        }

        return new GraphSnapshot(nodes.ToImmutable(), ports.ToImmutable());
    }

    public GraphNode? FindNode(string mediaClass, string description) =>
        Nodes.FirstOrDefault(node => node.MediaClass == mediaClass && node.Description == description);

    public ImmutableArray<GraphPort> AudioPortsOf(uint nodeId, PortDirection direction) =>
        [.. Ports.Where(port => port.NodeId == nodeId && port.Direction == direction && !port.IsMonitor).OrderBy(port => port.Order)];

    private static bool TryGetProperties(JsonElement item, out JsonElement properties)
    {
        properties = default;
        return item.TryGetProperty("info", out var info)
            && info.ValueKind == JsonValueKind.Object
            && info.TryGetProperty("props", out properties)
            && properties.ValueKind == JsonValueKind.Object;
    }

    private static GraphNode NodeFrom(uint id, JsonElement properties) =>
        new(id, Text(properties, "node.name"), Text(properties, "node.description"), Text(properties, "media.class"));

    private static GraphPort PortFrom(uint id, JsonElement properties) => new(
        id,
        Number(properties, "node.id"),
        Text(properties, "port.direction") == "in" ? PortDirection.Input : PortDirection.Output,
        Text(properties, "port.name"),
        Number(properties, "port.id"),
        Flag(properties, "port.monitor"));

    private static string Text(JsonElement properties, string key) =>
        properties.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString()! : string.Empty;

    private static uint Number(JsonElement properties, string key) =>
        properties.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.Number ? value.GetUInt32() : uint.MaxValue;

    private static bool Flag(JsonElement properties, string key) =>
        properties.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.True;
}
