using Mixer.Domain.Channels;
using Mixer.Dsp.Routing;

namespace Mixer.Dsp.Tests.Routing;

public sealed class PanLawTests
{
    [Fact]
    public void Constant_power_center_attenuates_both_sides_by_three_decibels()
    {
        var gain = PanLaw.ConstantPower(Pan.Center);

        Assert.Equal(MathF.Sqrt(0.5f), gain.Left, 0.0001f);
        Assert.Equal(MathF.Sqrt(0.5f), gain.Right, 0.0001f);
    }

    [Fact]
    public void Constant_power_hard_left_sends_everything_left()
    {
        var gain = PanLaw.ConstantPower(new Pan(Pan.HardLeft));

        Assert.Equal(1f, gain.Left, 0.0001f);
        Assert.Equal(0f, gain.Right, 0.0001f);
    }

    [Fact]
    public void Constant_power_keeps_total_power_constant()
    {
        foreach (var position in new[] { -1f, -0.5f, 0f, 0.3f, 1f })
        {
            var gain = PanLaw.ConstantPower(new Pan(position));

            Assert.Equal(1f, (gain.Left * gain.Left) + (gain.Right * gain.Right), 0.0001f);
        }
    }

    [Fact]
    public void Balance_center_leaves_stereo_untouched()
    {
        Assert.Equal(new StereoGain(1f, 1f), PanLaw.Balance(Pan.Center));
    }

    [Fact]
    public void Balance_right_attenuates_only_left()
    {
        Assert.Equal(new StereoGain(0.5f, 1f), PanLaw.Balance(new Pan(0.5f)));
    }
}
