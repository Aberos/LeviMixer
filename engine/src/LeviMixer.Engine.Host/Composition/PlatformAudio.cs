using LeviMixer.Application.Devices;

namespace LeviMixer.Engine.Host.Composition;

internal sealed record PlatformAudio(IAudioDeviceCatalog Catalog, IAudioConnectionFactory Connections, ISystemAudioCapture SystemAudioCapture, IDisposable? Resources);
