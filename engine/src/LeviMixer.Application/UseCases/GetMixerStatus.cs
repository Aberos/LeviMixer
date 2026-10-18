using LeviMixer.Application.Session;

namespace LeviMixer.Application.UseCases;

public sealed class GetMixerStatus(MixerSession session)
{
    public MixerStatus Execute() => session.Status;
}
