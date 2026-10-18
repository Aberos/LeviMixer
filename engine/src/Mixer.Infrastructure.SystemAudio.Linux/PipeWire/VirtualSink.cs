namespace Mixer.Infrastructure.SystemAudio.Linux.PipeWire;

internal static class VirtualSink
{
    public const string NodeName = "mixer_pc";
    public const string Description = "Mixer PC";

    public static string NodeProperties =>
        $"{{ factory.name=support.null-audio-sink node.name={NodeName} node.description=\"{Description}\" media.class=Audio/Sink audio.position=[FL FR] }}";

    public static bool IsVirtualEndpoint(string endpointName) =>
        endpointName.EndsWith(Description, StringComparison.Ordinal);

    public static bool IsMonitor(string endpointName) =>
        IsVirtualEndpoint(endpointName) && endpointName.Length > Description.Length;
}
