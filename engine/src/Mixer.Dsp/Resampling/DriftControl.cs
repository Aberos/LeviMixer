namespace Mixer.Dsp.Resampling;

public sealed record DriftControl
{
    public DriftControl(int targetFrames, int maxFramesPerRead)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(targetFrames);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxFramesPerRead);
        TargetFrames = targetFrames;
        MaxFramesPerRead = maxFramesPerRead;
    }

    public int TargetFrames { get; }

    public int MaxFramesPerRead { get; }

    public double MaxRatioDeviation { get; init; } = 0.005;

    public double Responsiveness { get; init; } = 0.005;

    public double FillSmoothing { get; init; } = 0.02;

    public int OverflowFrames => TargetFrames * 4;
}
