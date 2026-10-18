namespace LeviMixer.Infrastructure.Ipc.Wire;

internal sealed record ConnectionWire(string InputDeviceId, string OutputDeviceId, int SampleRate, int BufferFrames);
