using Mixer.Application.Results;
using Mixer.Application.Session;
using Mixer.Domain.Levels;

namespace Mixer.Application.UseCases;

public sealed class SetMasterGain(MixerSession session)
{
    public CommandResult Execute(Decibels gain) => session.Change(state => state.WithMasterGain(gain));
}
