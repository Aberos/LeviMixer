namespace Mixer.Infrastructure.Ipc.Wire;

internal sealed record ChannelGainPayload(string ChannelId, float? GainDecibels);
