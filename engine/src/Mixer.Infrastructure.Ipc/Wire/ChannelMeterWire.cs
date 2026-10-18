namespace Mixer.Infrastructure.Ipc.Wire;

internal sealed record ChannelMeterWire(string ChannelId, float Peak, float Rms);
