using Hexa.NET.MiniAudio;

namespace Mixer.Infrastructure.Miniaudio;

internal readonly record struct DuplexDevicePair(MaDeviceId Capture, MaDeviceId Playback);
