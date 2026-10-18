using LeviMixer.Application.Results;
using LeviMixer.Application.Session;
using LeviMixer.Domain.Channels;

namespace LeviMixer.Application.UseCases;

public sealed class SetChannelMute(MixerSession session)
{
    public CommandResult Execute(ChannelId channel, bool isMuted) =>
        session.ChangeChannel(channel, strip => strip with { IsMuted = isMuted });
}
