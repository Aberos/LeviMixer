namespace Mixer.Infrastructure.Ipc.Wire;

internal sealed record ChannelSettingsWire(string Id, string Name, float? GainDecibels, float Pan, bool Muted, bool Soloed);
