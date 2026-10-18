using Mixer.Dsp.Buffers;
using Mixer.Dsp.Mixing;
using Mixer.Dsp.Resampling;

namespace Mixer.Application.Session;

internal sealed class SystemAudioPipeline
{
    private const int RingCapacitySamples = 32_768;
    private const int MinimumLatencyFrames = 1_024;
    private const int BlocksOfLatency = 4;

    public SystemAudioPipeline(IAudioBlockProcessor mixer, int bufferFrames, int maxFramesPerBlock)
    {
        Ring = new SpscRingBuffer(RingCapacitySamples);
        var control = new DriftControl(Math.Max(MinimumLatencyFrames, bufferFrames * BlocksOfLatency), maxFramesPerBlock);
        Processor = new SystemAudioInjector(mixer, new DriftCompensatingReader(Ring, control), maxFramesPerBlock);
    }

    public SpscRingBuffer Ring { get; }

    public IAudioBlockProcessor Processor { get; }
}
