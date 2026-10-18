namespace Mixer.Dsp.Buffers;

public readonly ref struct InterleavedInput
{
    public InterleavedInput(ReadOnlySpan<float> samples, int channelCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(channelCount);
        Samples = samples;
        ChannelCount = channelCount;
    }

    public static InterleavedInput None => new([], 1);

    public ReadOnlySpan<float> Samples { get; }

    public int ChannelCount { get; }

    public int FrameCount => Samples.Length / ChannelCount;

    public bool Covers(int frameCount, int channel) => channel < ChannelCount && FrameCount >= frameCount;

    public float Sample(int frame, int channel) => Samples[(frame * ChannelCount) + channel];
}
