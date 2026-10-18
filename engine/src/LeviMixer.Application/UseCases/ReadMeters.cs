using LeviMixer.Application.Session;
using LeviMixer.Dsp.Metering;

namespace LeviMixer.Application.UseCases;

public sealed class ReadMeters(MixerSession session)
{
    public MeterReport? Execute() => session.TakeMeterReport();
}
