namespace LeviMixer.Application.Devices;

public sealed record AudioEndpoint(AudioDeviceId Id, string Name, int ChannelCount, bool IsDefault);
