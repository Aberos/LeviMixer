namespace Mixer.Dsp.Mixing;

public sealed record MixEngineCapacity
{
    public MixEngineCapacity(int sampleRate, int maxFrames, int maxStrips)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sampleRate);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxFrames);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxStrips);
        SampleRate = sampleRate;
        MaxFrames = maxFrames;
        MaxStrips = maxStrips;
    }

    public int SampleRate { get; }

    public int MaxFrames { get; }

    public int MaxStrips { get; }
}
