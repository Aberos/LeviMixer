using Mixer.Application.Session;
using Mixer.Application.UseCases;
using Mixer.Infrastructure.Miniaudio;

namespace Mixer.Engine.Host.Composition;

internal sealed class EngineComposition : IDisposable
{
    private readonly MiniaudioContext _audioContext = new();
    private readonly MixerSession _session;

    public EngineComposition()
    {
        var catalog = new MiniaudioDeviceCatalog(_audioContext);
        _session = new MixerSession(new MiniaudioConnectionFactory(_audioContext));
        ListAudioDevices = new ListAudioDevices(catalog);
        StartMixing = new StartMixing(catalog, _session);
        StopMixing = new StopMixing(_session);
        SetChannelGain = new SetChannelGain(_session);
        SetChannelMute = new SetChannelMute(_session);
        SetChannelSolo = new SetChannelSolo(_session);
        SetChannelPan = new SetChannelPan(_session);
        SetMasterGain = new SetMasterGain(_session);
        ReadMeters = new ReadMeters(_session);
    }

    public ListAudioDevices ListAudioDevices { get; }

    public StartMixing StartMixing { get; }

    public StopMixing StopMixing { get; }

    public SetChannelGain SetChannelGain { get; }

    public SetChannelMute SetChannelMute { get; }

    public SetChannelSolo SetChannelSolo { get; }

    public SetChannelPan SetChannelPan { get; }

    public SetMasterGain SetMasterGain { get; }

    public ReadMeters ReadMeters { get; }

    public void Dispose()
    {
        _session.Dispose();
        _audioContext.Dispose();
    }
}
