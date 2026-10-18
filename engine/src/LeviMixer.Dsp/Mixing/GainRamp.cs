namespace LeviMixer.Dsp.Mixing;

public readonly record struct GainRamp(float Start, float Step)
{
    public static GainRamp Between(float start, float target, int frameCount) =>
        new(start, frameCount == 0 ? 0f : (target - start) / frameCount);

    public float At(int frame) => Start + (Step * frame);
}
