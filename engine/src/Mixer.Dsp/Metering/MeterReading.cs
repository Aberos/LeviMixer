namespace Mixer.Dsp.Metering;

public readonly record struct MeterReading(float Peak, float Rms)
{
    public static MeterReading Silence { get; } = new(0f, 0f);
}
