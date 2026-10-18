using Hexa.NET.MiniAudio;

namespace Mixer.Infrastructure.Miniaudio;

internal readonly record struct DeviceIdPair(MaDeviceId Capture, MaDeviceId Playback);
