using Mixer.Domain.Channels;

namespace Mixer.Dsp.Routing;

public static class PanLaw
{
    public static StereoGain ConstantPower(Pan pan)
    {
        var angle = (pan.Position + 1f) * MathF.PI / 4f;
        return new StereoGain(MathF.Cos(angle), MathF.Sin(angle));
    }

    public static StereoGain Balance(Pan pan) =>
        new(MathF.Min(1f, 1f - pan.Position), MathF.Min(1f, 1f + pan.Position));
}
