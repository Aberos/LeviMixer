using Mixer.Application.Devices;

namespace Mixer.Engine.Host.Composition;

internal sealed record PlatformAudio(IAudioDeviceCatalog Catalog, ISystemAudioCapture SystemAudioCapture, IDisposable? Resources);
