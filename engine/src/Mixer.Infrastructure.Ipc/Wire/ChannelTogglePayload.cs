namespace Mixer.Infrastructure.Ipc.Wire;

internal sealed record ChannelTogglePayload(string ChannelId, bool Value);
