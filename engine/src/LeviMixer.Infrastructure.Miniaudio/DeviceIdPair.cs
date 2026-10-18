using Hexa.NET.MiniAudio;

namespace LeviMixer.Infrastructure.Miniaudio;

internal readonly record struct DeviceIdPair(MaDeviceId Capture, MaDeviceId Playback);
