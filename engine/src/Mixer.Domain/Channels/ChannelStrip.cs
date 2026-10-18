using Mixer.Domain.Levels;

namespace Mixer.Domain.Channels;

public sealed record ChannelStrip(ChannelId Id, string Name, ChannelSource Source)
{
    private readonly Decibels _gain = Decibels.Unity;

    public Decibels Gain
    {
        get => _gain;
        init => _gain = FaderRange.Clamp(value);
    }

    public Pan Pan { get; init; } = Pan.Center;

    public bool IsMuted { get; init; }

    public bool IsSoloed { get; init; }
}
