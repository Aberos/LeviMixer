using LeviMixer.Application.Results;
using LeviMixer.Application.Session;
using LeviMixer.Domain.Channels;

namespace LeviMixer.Application.UseCases;

public sealed class RenameChannel(MixerSession session)
{
    public CommandResult Execute(ChannelId channel, string name) =>
        ChannelName.IsValid(name)
            ? session.ChangeChannel(channel, strip => strip with { Name = name })
            : CommandResult.Failure(CommandError.InvalidChannelName(ChannelName.MaxLength));
}
