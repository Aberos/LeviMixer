using LeviMixer.Application.Devices;
using LeviMixer.Application.Session;

namespace LeviMixer.Application.UseCases;

public sealed class MixerUseCases
{
    public MixerUseCases(IAudioDeviceCatalog catalog, MixerSession session)
    {
        ListAudioDevices = new ListAudioDevices(catalog);
        StartMixing = new StartMixing(catalog, session);
        StopMixing = new StopMixing(session);
        ApplySession = new ApplySession(StartMixing, session);
        GetMixerStatus = new GetMixerStatus(session);
        SetChannelGain = new SetChannelGain(session);
        SetChannelMute = new SetChannelMute(session);
        SetChannelSolo = new SetChannelSolo(session);
        SetChannelPan = new SetChannelPan(session);
        RenameChannel = new RenameChannel(session);
        SetMasterGain = new SetMasterGain(session);
        ReadMeters = new ReadMeters(session);
    }

    public ListAudioDevices ListAudioDevices { get; }

    public StartMixing StartMixing { get; }

    public StopMixing StopMixing { get; }

    public ApplySession ApplySession { get; }

    public GetMixerStatus GetMixerStatus { get; }

    public SetChannelGain SetChannelGain { get; }

    public SetChannelMute SetChannelMute { get; }

    public SetChannelSolo SetChannelSolo { get; }

    public SetChannelPan SetChannelPan { get; }

    public RenameChannel RenameChannel { get; }

    public SetMasterGain SetMasterGain { get; }

    public ReadMeters ReadMeters { get; }
}
