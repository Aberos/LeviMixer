namespace LeviMixer.Infrastructure.SystemAudio.Windows.Tests;

public sealed class VbCableTests
{
    [Theory]
    [InlineData("CABLE Input (VB-Audio Virtual Cable)")]
    [InlineData("CABLE In 16ch (VB-Audio Virtual Cable)")]
    [InlineData("CABLE Output (VB-Audio Virtual Cable)")]
    public void Every_cable_endpoint_is_recognised(string name)
    {
        Assert.True(VbCable.IsCableEndpoint(name));
    }

    [Fact]
    public void The_sixteen_channel_input_is_not_used_as_the_default_playback()
    {
        Assert.False(VbCable.IsPlayback("CABLE In 16ch (VB-Audio Virtual Cable)"));
        Assert.True(VbCable.IsPlayback("CABLE Input (VB-Audio Virtual Cable)"));
    }

    [Fact]
    public void Regular_devices_are_not_cable_endpoints()
    {
        Assert.False(VbCable.IsCableEndpoint("Alto-falantes (USB Audio CODEC )"));
    }
}
