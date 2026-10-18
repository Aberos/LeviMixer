using LeviMixer.Domain.Levels;

namespace LeviMixer.Domain.Tests.Levels;

public sealed class DecibelsTests
{
    [Theory]
    [InlineData(0f, 1f)]
    [InlineData(-6.0206f, 0.5f)]
    [InlineData(-20f, 0.1f)]
    [InlineData(6.0206f, 2f)]
    public void Converts_decibels_to_linear_amplitude(float decibels, float expectedLinear)
    {
        var linear = new Decibels(decibels).ToLinear();

        Assert.Equal(expectedLinear, linear.Value, 0.0001f);
    }

    [Fact]
    public void Minus_infinity_is_silent()
    {
        Assert.True(Decibels.MinusInfinity.IsSilent);
        Assert.Equal(LinearGain.Silence, Decibels.MinusInfinity.ToLinear());
    }

    [Fact]
    public void Zero_amplitude_converts_to_minus_infinity()
    {
        Assert.Equal(Decibels.MinusInfinity, Decibels.FromLinear(0f));
    }

    [Fact]
    public void Rejects_nan()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Decibels(float.NaN));
    }

    [Fact]
    public void Fader_range_limits_boost_to_maximum()
    {
        Assert.Equal(FaderRange.Maximum, FaderRange.Clamp(new Decibels(40f)));
        Assert.Equal(new Decibels(-12f), FaderRange.Clamp(new Decibels(-12f)));
    }
}
