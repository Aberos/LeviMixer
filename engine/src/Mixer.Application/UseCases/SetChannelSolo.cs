using Mixer.Application.Results;
using Mixer.Application.Session;
using Mixer.Domain.Channels;

namespace Mixer.Application.UseCases;

public sealed class SetChannelSolo(MixerSession session)
{
    public CommandResult Execute(ChannelId channel, bool isSoloed) =>
        session.ChangeChannel(channel, strip => strip with { IsSoloed = isSoloed });
}
