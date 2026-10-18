using LeviMixer.Domain.Levels;

namespace LeviMixer.Dsp.Routing;

public readonly record struct StereoGain(float Left, float Right)
{
    public static StereoGain Silence { get; } = new(0f, 0f);

    public StereoGain Scale(LinearGain gain) => new(Left * gain.Value, Right * gain.Value);
}
