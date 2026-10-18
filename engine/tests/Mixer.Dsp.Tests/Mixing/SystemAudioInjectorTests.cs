using Mixer.Domain.Mixing;
using Mixer.Dsp.Buffers;
using Mixer.Dsp.Mixing;
using Mixer.Dsp.Resampling;

namespace Mixer.Dsp.Tests.Mixing;

public sealed class SystemAudioInjectorTests
{
    [Fact]
    public void System_audio_from_the_ring_reaches_the_output()
    {
        var engine = new MixEngine(new MixEngineCapacity(48_000, 64, 4));
        engine.Apply(MixSnapshot.From(DefaultChannelLayout.ForInterface(1)));
        var ring = new SpscRingBuffer(4096);
        ring.Write(Enumerable.Repeat(0.25f, 2048).ToArray());
        var injector = new SystemAudioInjector(engine, new DriftCompensatingReader(ring, new DriftControl(256, 64)), 64);
        var output = new float[128];

        for (var block = 0; block < 3; block++)
        {
            injector.Process(new AudioBlock(new InterleavedInput(new float[64], 1), InterleavedInput.None, new InterleavedOutput(output, 2)));
        }

        Assert.Equal(0.25f, output[^2], 0.0001f);
        Assert.Equal(0.25f, output[^1], 0.0001f);
    }
}
