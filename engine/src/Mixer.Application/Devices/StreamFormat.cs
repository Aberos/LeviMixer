namespace Mixer.Application.Devices;

public sealed record StreamFormat
{
    public StreamFormat(int sampleRate, int bufferFrames)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sampleRate);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bufferFrames);
        SampleRate = sampleRate;
        BufferFrames = bufferFrames;
    }

    public static StreamFormat Default { get; } = new(48_000, 128);

    public int SampleRate { get; }

    public int BufferFrames { get; }
}
