namespace Mixer.Domain.Levels;

public static class FaderRange
{
    public static Decibels Maximum { get; } = new(10f);

    public static Decibels Clamp(Decibels gain) => gain.Value > Maximum.Value ? Maximum : gain;
}
