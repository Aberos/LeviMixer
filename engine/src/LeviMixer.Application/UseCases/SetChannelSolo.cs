using LeviMixer.Application.Results;
using LeviMixer.Application.Session;
using LeviMixer.Domain.Channels;

namespace LeviMixer.Application.UseCases;

public sealed class SetChannelSolo(MixerSession session)
{
    public CommandResult Execute(ChannelId channel, bool isSoloed) =>
        session.ChangeChannel(channel, strip => strip with { IsSoloed = isSoloed });
}
