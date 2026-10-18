namespace LeviMixer.Domain.Levels;

public readonly record struct Decibels
{
    public Decibels(float value)
    {
        if (float.IsNaN(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Decibels cannot be NaN.");
        }

        Value = value;
    }

    public static Decibels MinusInfinity { get; } = new(float.NegativeInfinity);

    public static Decibels Unity { get; } = new(0f);

    public float Value { get; }

    public bool IsSilent => float.IsNegativeInfinity(Value);

    public LinearGain ToLinear() => IsSilent ? LinearGain.Silence : new LinearGain(MathF.Pow(10f, Value / 20f));

    public static Decibels FromLinear(float amplitude) =>
        amplitude <= 0f ? MinusInfinity : new Decibels(20f * MathF.Log10(amplitude));
}
