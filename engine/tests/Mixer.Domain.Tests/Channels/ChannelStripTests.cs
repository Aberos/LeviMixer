using Mixer.Domain.Channels;
using Mixer.Domain.Levels;

namespace Mixer.Domain.Tests.Channels;

public sealed class ChannelStripTests
{
    [Fact]
    public void New_strip_starts_at_unity_centered_and_unmuted()
    {
        var strip = new ChannelStrip(new ChannelId("in-1"), "Mic", new InterfaceInput(0));

        Assert.Equal(Decibels.Unity, strip.Gain);
        Assert.Equal(Pan.Center, strip.Pan);
        Assert.False(strip.IsMuted);
        Assert.False(strip.IsSoloed);
    }

    [Fact]
    public void Gain_above_fader_maximum_is_clamped()
    {
        var strip = new ChannelStrip(new ChannelId("in-1"), "Mic", new InterfaceInput(0)) { Gain = new Decibels(30f) };

        Assert.Equal(FaderRange.Maximum, strip.Gain);
    }

    [Fact]
    public void Name_is_trimmed()
    {
        var strip = new ChannelStrip(new ChannelId("in-1"), "  Vocal  ", new InterfaceInput(0));

        Assert.Equal("Vocal", strip.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("A name that is way too long for a strip")]
    public void Invalid_names_are_rejected(string name)
    {
        var strip = new ChannelStrip(new ChannelId("in-1"), "Mic", new InterfaceInput(0));

        Assert.Throws<ArgumentException>(() => strip with { Name = name });
    }
}
