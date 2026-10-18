using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Testing.Fakes;

public sealed class FakeSystemAudioFeed(ISampleWriter destination) : IDisposable
{
    public bool IsStopped { get; private set; }

    public void Push(float value, int frames) => destination.Write(Enumerable.Repeat(value, frames * 2).ToArray());

    public void Dispose() => IsStopped = true;
}
