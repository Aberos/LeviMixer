using LeviMixer.Domain.Channels;

namespace LeviMixer.Domain.Tests.Channels;

public sealed class PanTests
{
    [Theory]
    [InlineData(-3f, -1f)]
    [InlineData(2f, 1f)]
    [InlineData(0.25f, 0.25f)]
    public void Position_is_clamped_to_hard_left_and_hard_right(float requested, float expected)
    {
        Assert.Equal(expected, new Pan(requested).Position);
    }
}
