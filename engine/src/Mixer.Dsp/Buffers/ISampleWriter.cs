namespace Mixer.Dsp.Buffers;

public interface ISampleWriter
{
    int Write(ReadOnlySpan<float> samples);
}
