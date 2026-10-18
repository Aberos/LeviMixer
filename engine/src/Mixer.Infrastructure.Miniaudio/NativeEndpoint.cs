using Hexa.NET.MiniAudio;
using Mixer.Application.Devices;

namespace Mixer.Infrastructure.Miniaudio;

internal sealed record NativeEndpoint(AudioEndpoint Endpoint, MaDeviceId NativeId);
