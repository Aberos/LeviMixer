namespace Mixer.Domain.Levels;

public readonly record struct LinearGain
{
    public LinearGain(float value)
    {
        if (float.IsNaN(value) || value < 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Linear gain must be a non-negative number.");
        }

        Value = value;
    }

    public static LinearGain Silence { get; } = new(0f);

    public static LinearGain Unity { get; } = new(1f);

    public float Value { get; }
}
