namespace Mixer.Infrastructure.Ipc.Wire;

internal sealed record MixerStatePayload(bool Running, ConnectionWire? Connection, ChannelWire[] Channels, float? MasterGainDecibels, string? SystemAudioError);
