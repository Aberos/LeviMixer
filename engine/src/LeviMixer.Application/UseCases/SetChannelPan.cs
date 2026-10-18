using LeviMixer.Application.Results;
using LeviMixer.Application.Session;
using LeviMixer.Domain.Channels;

namespace LeviMixer.Application.UseCases;

public sealed class SetChannelPan(MixerSession session)
{
    public CommandResult Execute(ChannelId channel, Pan pan) =>
        session.ChangeChannel(channel, strip => strip with { Pan = pan });
}
