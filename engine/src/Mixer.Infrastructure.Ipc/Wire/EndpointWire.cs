namespace Mixer.Infrastructure.Ipc.Wire;

internal sealed record EndpointWire(string Id, string Name, int ChannelCount, bool IsDefault);
