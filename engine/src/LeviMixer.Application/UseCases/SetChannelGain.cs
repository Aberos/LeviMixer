using LeviMixer.Application.Results;
using LeviMixer.Application.Session;
using LeviMixer.Domain.Channels;
using LeviMixer.Domain.Levels;

namespace LeviMixer.Application.UseCases;

public sealed class SetChannelGain(MixerSession session)
{
    public CommandResult Execute(ChannelId channel, Decibels gain) =>
        session.ChangeChannel(channel, strip => strip with { Gain = gain });
}
