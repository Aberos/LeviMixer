using LeviMixer.Domain.Levels;

namespace LeviMixer.Domain.Channels;

public sealed record ChannelStrip(ChannelId Id, string Name, ChannelSource Source)
{
    private readonly string _name = ChannelName.Normalize(Name);
    private readonly Decibels _gain = Decibels.Unity;

    public string Name
    {
        get => _name;
        init => _name = ChannelName.Normalize(value);
    }

    public Decibels Gain
    {
        get => _gain;
        init => _gain = FaderRange.Clamp(value);
    }

    public Pan Pan { get; init; } = Pan.Center;

    public bool IsMuted { get; init; }

    public bool IsSoloed { get; init; }
}
