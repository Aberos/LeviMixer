using LeviMixer.Domain.Levels;

namespace LeviMixer.Domain.Mixing;

public sealed record MasterBus
{
    private readonly Decibels _gain = Decibels.Unity;

    public static MasterBus AtUnity { get; } = new();

    public Decibels Gain
    {
        get => _gain;
        init => _gain = FaderRange.Clamp(value);
    }
}
