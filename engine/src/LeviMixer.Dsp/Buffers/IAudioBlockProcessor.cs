namespace LeviMixer.Dsp.Buffers;

public interface IAudioBlockProcessor
{
    void Process(AudioBlock block);
}
