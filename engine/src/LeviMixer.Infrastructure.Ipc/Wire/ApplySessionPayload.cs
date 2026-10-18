namespace LeviMixer.Infrastructure.Ipc.Wire;

internal sealed record ApplySessionPayload(string? InputDeviceId, string? OutputDeviceId, int? SampleRate, int? BufferFrames, ChannelSettingsWire[]? Channels, float? MasterGainDecibels);
