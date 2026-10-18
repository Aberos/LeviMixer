using Mixer.Application.Session;
using Mixer.Dsp.Metering;

namespace Mixer.Application.UseCases;

public sealed class ReadMeters(MixerSession session)
{
    public MeterReport? Execute() => session.TakeMeterReport();
}
