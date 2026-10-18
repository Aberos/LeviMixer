using Mixer.Application.Devices;

namespace Mixer.Application.UseCases;

public sealed record StartMixingRequest(AudioDeviceId? Input, AudioDeviceId? Output, StreamFormat Format);
