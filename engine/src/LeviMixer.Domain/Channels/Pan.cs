namespace LeviMixer.Domain.Channels;

public readonly record struct Pan
{
    public const float HardLeft = -1f;
    public const float HardRight = 1f;

    public Pan(float position)
    {
        if (float.IsNaN(position))
        {
            throw new ArgumentOutOfRangeException(nameof(position), "Pan position cannot be NaN.");
        }

        Position = Math.Clamp(position, HardLeft, HardRight);
    }

    public static Pan Center { get; } = new(0f);

    public float Position { get; }
}
