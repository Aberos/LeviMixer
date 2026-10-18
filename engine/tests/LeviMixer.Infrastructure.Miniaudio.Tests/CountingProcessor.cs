using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Infrastructure.Miniaudio.Tests;

internal sealed class CountingProcessor : IAudioBlockProcessor
{
    private int _blocks;
    private int _lastFrameCount;

    public int Blocks => Volatile.Read(ref _blocks);

    public int LastFrameCount => Volatile.Read(ref _lastFrameCount);

    public void Process(AudioBlock block)
    {
        block.Output.Samples.Clear();
        Volatile.Write(ref _lastFrameCount, block.FrameCount);
        Interlocked.Increment(ref _blocks);
    }
}
