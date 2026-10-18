using Hexa.NET.MiniAudio;
using LeviMixer.Application.Devices;

namespace LeviMixer.Infrastructure.Miniaudio;

internal sealed record NativeEndpoint(AudioEndpoint Endpoint, MaDeviceId NativeId);
