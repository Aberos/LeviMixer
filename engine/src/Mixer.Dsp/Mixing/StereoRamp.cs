using Mixer.Dsp.Routing;

namespace Mixer.Dsp.Mixing;

public readonly record struct StereoRamp(GainRamp Left, GainRamp Right)
{
    public static StereoRamp Between(StereoGain start, StereoGain target, int frameCount) =>
        new(GainRamp.Between(start.Left, target.Left, frameCount), GainRamp.Between(start.Right, target.Right, frameCount));

    public StereoGain At(int frame) => new(Left.At(frame), Right.At(frame));
}
