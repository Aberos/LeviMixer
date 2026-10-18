using Mixer.Application.Results;
using Mixer.Application.Session;
using Mixer.Domain.Channels;

namespace Mixer.Application.UseCases;

public sealed class RenameChannel(MixerSession session)
{
    public CommandResult Execute(ChannelId channel, string name) =>
        ChannelName.IsValid(name)
            ? session.ChangeChannel(channel, strip => strip with { Name = name })
            : CommandResult.Failure(CommandError.InvalidChannelName(ChannelName.MaxLength));
}
