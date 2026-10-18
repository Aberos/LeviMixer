using LeviMixer.Domain.Channels;
using LeviMixer.Domain.Levels;

namespace LeviMixer.Application.Settings;

public sealed record ChannelSettings(ChannelId Id, string Name, Decibels Gain, Pan Pan, bool IsMuted, bool IsSoloed)
{
    public ChannelStrip ApplyTo(ChannelStrip strip)
    {
        ArgumentNullException.ThrowIfNull(strip);
        return strip with
        {
            Name = ChannelName.IsValid(Name) ? Name : strip.Name,
            Gain = Gain,
            Pan = Pan,
            IsMuted = IsMuted,
            IsSoloed = IsSoloed,
        };
    }
}
