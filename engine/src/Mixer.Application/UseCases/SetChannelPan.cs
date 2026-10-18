using Mixer.Application.Results;
using Mixer.Application.Session;
using Mixer.Domain.Channels;

namespace Mixer.Application.UseCases;

public sealed class SetChannelPan(MixerSession session)
{
    public CommandResult Execute(ChannelId channel, Pan pan) =>
        session.ChangeChannel(channel, strip => strip with { Pan = pan });
}
