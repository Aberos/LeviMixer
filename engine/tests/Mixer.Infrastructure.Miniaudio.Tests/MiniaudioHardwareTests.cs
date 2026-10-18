using Mixer.Application.Devices;

namespace Mixer.Infrastructure.Miniaudio.Tests;

[Trait("Category", "Hardware")]
public sealed class MiniaudioHardwareTests
{
    [Fact]
    public void Lists_default_input_and_output_with_channels()
    {
        using var context = new MiniaudioContext();

        var devices = new MiniaudioDeviceCatalog(context).ListDevices();

        Assert.NotNull(devices.DefaultInput);
        Assert.NotNull(devices.DefaultOutput);
        Assert.All(devices.Inputs, input => Assert.True(input.ChannelCount > 0));
    }

    [Fact]
    public void Duplex_connection_delivers_blocks_of_the_requested_size()
    {
        using var context = new MiniaudioContext();
        var devices = new MiniaudioDeviceCatalog(context).ListDevices();
        var request = new AudioConnectionRequest(devices.DefaultInput!, devices.DefaultOutput!, new StreamFormat(48_000, 256));
        var processor = new CountingProcessor();

        using (new MiniaudioConnectionFactory(context).Open(request, processor))
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
        }

        Assert.True(processor.Blocks > 10);
        Assert.Equal(256, processor.LastFrameCount);
    }
}
