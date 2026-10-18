namespace LeviMixer.Infrastructure.Ipc.Wire;

internal sealed record ChannelWire(string Id, string Name, string Source, int? InputIndex, float? GainDecibels, float Pan, bool Muted, bool Soloed);
