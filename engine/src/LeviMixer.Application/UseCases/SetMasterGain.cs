using LeviMixer.Application.Results;
using LeviMixer.Application.Session;
using LeviMixer.Domain.Levels;

namespace LeviMixer.Application.UseCases;

public sealed class SetMasterGain(MixerSession session)
{
    public CommandResult Execute(Decibels gain) => session.Change(state => state.WithMasterGain(gain));
}
