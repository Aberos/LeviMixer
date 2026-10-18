using Mixer.Application.Results;
using Mixer.Application.Session;
using Mixer.Application.Settings;
using Mixer.Domain.Mixing;

namespace Mixer.Application.UseCases;

public sealed class ApplySession(StartMixing startMixing, MixerSession session)
{
    public CommandResult Execute(SessionSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        var started = startMixing.Execute(new StartMixingRequest(settings.Input, settings.Output, settings.Format));
        return started.IsSuccess ? session.Change(state => Restore(state, settings)) : started;
    }

    private static MixerState Restore(MixerState state, SessionSettings settings) =>
        settings.Channels
            .Where(channel => state.Contains(channel.Id))
            .Aggregate(state, (current, channel) => current.UpdateChannel(channel.Id, channel.ApplyTo))
            .WithMasterGain(settings.MasterGain);
}
