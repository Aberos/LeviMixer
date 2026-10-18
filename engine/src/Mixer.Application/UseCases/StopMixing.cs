using Mixer.Application.Session;

namespace Mixer.Application.UseCases;

public sealed class StopMixing(MixerSession session)
{
    public void Execute() => session.Stop();
}
