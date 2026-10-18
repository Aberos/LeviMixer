using Mixer.Application.Results;
using Mixer.Application.Session;
using Mixer.Domain.Channels;
using Mixer.Domain.Levels;

namespace Mixer.Application.UseCases;

public sealed class SetChannelGain(MixerSession session)
{
    public CommandResult Execute(ChannelId channel, Decibels gain) =>
        session.ChangeChannel(channel, strip => strip with { Gain = gain });
}
