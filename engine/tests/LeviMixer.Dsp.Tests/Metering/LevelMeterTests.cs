using LeviMixer.Dsp.Metering;

namespace LeviMixer.Dsp.Tests.Metering;

public sealed class LevelMeterTests
{
    [Fact]
    public void Peak_holds_the_highest_value_until_read()
    {
        var meter = new LevelMeter(48_000);

        meter.Record(0.3f, 0f, 128);
        meter.Record(0.8f, 0f, 128);
        meter.Record(0.5f, 0f, 128);

        Assert.Equal(0.8f, meter.TakeReading().Peak);
        Assert.Equal(0f, meter.TakeReading().Peak);
    }

    [Fact]
    public void Rms_converges_to_the_signal_level()
    {
        var meter = new LevelMeter(48_000);

        for (var block = 0; block < 2_000; block++)
        {
            meter.Record(1f, 0.25f, 128);
        }

        Assert.Equal(0.5f, meter.TakeReading().Rms, 0.001f);
    }
}
