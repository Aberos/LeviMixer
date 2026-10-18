using LeviMixer.Application.Devices;

namespace LeviMixer.Application.UseCases;

public sealed record StartMixingRequest(AudioDeviceId? Input, AudioDeviceId? Output, StreamFormat Format);
