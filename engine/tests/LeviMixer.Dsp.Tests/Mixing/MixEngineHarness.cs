using LeviMixer.Domain.Mixing;
using LeviMixer.Dsp.Buffers;
using LeviMixer.Dsp.Mixing;

namespace LeviMixer.Dsp.Tests.Mixing;

internal sealed class MixEngineHarness
{
    public const int FrameCount = 64;
    public const int OutputChannels = 2;

    private readonly float[] _interfaceInput;
    private readonly float[] _systemAudio = new float[FrameCount * 2];

    public MixEngineHarness(int interfaceChannels)
    {
        InterfaceChannels = interfaceChannels;
        _interfaceInput = new float[FrameCount * interfaceChannels];
        Engine = new MixEngine(new MixEngineCapacity(48_000, FrameCount, 8));
    }

    public MixEngine Engine { get; }

    public int InterfaceChannels { get; }

    public float[] Output { get; } = new float[FrameCount * OutputChannels];

    public MixEngineHarness With(MixerState state)
    {
        Engine.Apply(MixSnapshot.From(state));
        return this;
    }

    public MixEngineHarness FeedInterfaceChannel(int channel, float value)
    {
        for (var frame = 0; frame < FrameCount; frame++)
        {
            _interfaceInput[(frame * InterfaceChannels) + channel] = value;
        }

        return this;
    }

    public MixEngineHarness FeedSystemAudio(float left, float right)
    {
        for (var frame = 0; frame < FrameCount; frame++)
        {
            _systemAudio[frame * 2] = left;
            _systemAudio[(frame * 2) + 1] = right;
        }

        return this;
    }

    public MixEngineHarness Process()
    {
        Engine.Process(new AudioBlock(
            new InterleavedInput(_interfaceInput, InterfaceChannels),
            new InterleavedInput(_systemAudio, 2),
            new InterleavedOutput(Output, OutputChannels)));
        return this;
    }

    public MixEngineHarness ProcessUntilSettled() => Process().Process();

    public float LastLeft => Output[^2];

    public float LastRight => Output[^1];
}
