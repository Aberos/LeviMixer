using Mixer.Application.Session;

namespace Mixer.Application.UseCases;

public sealed class GetMixerStatus(MixerSession session)
{
    public MixerStatus Execute() => session.Status;
}
