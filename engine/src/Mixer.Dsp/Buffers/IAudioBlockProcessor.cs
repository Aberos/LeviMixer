namespace Mixer.Dsp.Buffers;

public interface IAudioBlockProcessor
{
    void Process(AudioBlock block);
}
