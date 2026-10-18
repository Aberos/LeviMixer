using LeviMixer.Application.Session;

namespace LeviMixer.Application.UseCases;

public sealed class StopMixing(MixerSession session)
{
    public void Execute() => session.Stop();
}
