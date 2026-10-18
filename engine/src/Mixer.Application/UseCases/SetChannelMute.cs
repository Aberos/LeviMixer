using Mixer.Application.Results;
using Mixer.Application.Session;
using Mixer.Domain.Channels;

namespace Mixer.Application.UseCases;

public sealed class SetChannelMute(MixerSession session)
{
    public CommandResult Execute(ChannelId channel, bool isMuted) =>
        session.ChangeChannel(channel, strip => strip with { IsMuted = isMuted });
}
