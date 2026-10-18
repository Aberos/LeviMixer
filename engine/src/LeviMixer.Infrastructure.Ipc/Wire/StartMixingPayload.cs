namespace LeviMixer.Infrastructure.Ipc.Wire;

internal sealed record StartMixingPayload(string? InputDeviceId, string? OutputDeviceId, int? SampleRate, int? BufferFrames);
