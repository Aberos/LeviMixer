using LeviMixer.Application.Devices;

namespace LeviMixer.Engine.Host.Composition;

internal sealed record PlatformAudio(IAudioDeviceCatalog Catalog, ISystemAudioCapture SystemAudioCapture, IDisposable? Resources);
