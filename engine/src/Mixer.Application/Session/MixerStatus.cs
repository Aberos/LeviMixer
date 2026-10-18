using Mixer.Application.Devices;
using Mixer.Domain.Mixing;

namespace Mixer.Application.Session;

public sealed record MixerStatus(MixerState State, AudioConnectionRequest? Connection, string? SystemAudioError)
{
    public bool IsRunning => Connection is not null;
}
