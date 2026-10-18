namespace Mixer.Dsp.Buffers;

public readonly ref struct InterleavedOutput
{
    public InterleavedOutput(Span<float> samples, int channelCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(channelCount);
        Samples = samples;
        ChannelCount = channelCount;
    }

    public Span<float> Samples { get; }

    public int ChannelCount { get; }

    public int FrameCount => Samples.Length / ChannelCount;
}
