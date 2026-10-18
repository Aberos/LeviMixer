using Mixer.Application.Session;
using Mixer.Application.UseCases;
using Mixer.Infrastructure.Miniaudio;

namespace Mixer.Engine.Host.Composition;

internal sealed class EngineComposition : IDisposable
{
    private readonly MiniaudioContext _audioContext = new();
    private readonly PlatformAudio _platform;
    private readonly MixerSession _session;
    private int _isDisposed;

    public EngineComposition()
    {
        _platform = PlatformAudioFactory.Create(_audioContext);
        _session = new MixerSession(new MiniaudioConnectionFactory(_audioContext), _platform.SystemAudioCapture);
        UseCases = new MixerUseCases(_platform.Catalog, _session);
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
    }

    public MixerUseCases UseCases { get; }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) == 1)
        {
            return;
        }

        AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
        _session.Dispose();
        _platform.Resources?.Dispose();
        _audioContext.Dispose();
    }

    private void OnProcessExit(object? sender, EventArgs eventArgs) => Dispose();
}
