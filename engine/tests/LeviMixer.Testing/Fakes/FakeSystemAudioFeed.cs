using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Testing.Fakes;

public sealed class FakeSystemAudioFeed(ISampleWriter destination, StreamFormat format) : IDisposable
{
    public StreamFormat Format { get; } = format;

    public bool IsStopped { get; private set; }

    public void Push(float value, int frames) => destination.Write(Enumerable.Repeat(value, frames * 2).ToArray());

    public void Dispose() => IsStopped = true;
}
