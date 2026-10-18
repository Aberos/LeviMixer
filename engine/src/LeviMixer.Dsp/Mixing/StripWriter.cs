using LeviMixer.Dsp.Buffers;
using LeviMixer.Dsp.Metering;

namespace LeviMixer.Dsp.Mixing;

public ref struct StripWriter
{
    private readonly StereoBus _bus;
    private readonly StereoRamp _ramp;
    private BlockLevel _level;

    public StripWriter(StereoBus bus, StereoRamp ramp)
    {
        _bus = bus;
        _ramp = ramp;
    }

    public readonly int FrameCount => _bus.FrameCount;

    public readonly BlockLevel Level => _level;

    public void Write(int frame, float left, float right)
    {
        var gain = _ramp.At(frame);
        var leftSample = left * gain.Left;
        var rightSample = right * gain.Right;
        _bus.Add(frame, leftSample, rightSample);
        _level.Add(MathF.Max(MathF.Abs(leftSample), MathF.Abs(rightSample)));
    }
}
