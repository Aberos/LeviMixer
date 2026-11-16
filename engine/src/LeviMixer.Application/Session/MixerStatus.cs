using LeviMixer.Application.Devices;
using LeviMixer.Domain.Mixing;

namespace LeviMixer.Application.Session;

public sealed record MixerStatus(MixerState State, AudioConnectionRequest? Connection, SystemAudioFailure? SystemAudioFailure)
{
    public bool IsRunning => Connection is not null;
}
