using Hexa.NET.MiniAudio;

namespace LeviMixer.Infrastructure.Miniaudio.Tests;

public sealed class DeviceChannelCountTests
{
    [Fact]
    public void Mono_hardware_format_wins_over_an_upmixed_stereo_mix_format()
    {
        MaDeviceInfo.NativeDataFormat[] formats = [Format(1, DeviceChannelCount.HardwareFormatFlag), Format(2, 0)];

        Assert.Equal(1u, DeviceChannelCount.Of(formats));
    }

    [Fact]
    public void Stereo_hardware_format_exposes_both_inputs()
    {
        MaDeviceInfo.NativeDataFormat[] formats = [Format(2, DeviceChannelCount.HardwareFormatFlag), Format(2, 0)];

        Assert.Equal(2u, DeviceChannelCount.Of(formats));
    }

    [Fact]
    public void Uses_the_widest_format_when_no_hardware_format_is_reported()
    {
        MaDeviceInfo.NativeDataFormat[] formats = [Format(1, 0), Format(4, 0)];

        Assert.Equal(4u, DeviceChannelCount.Of(formats));
    }

    [Fact]
    public void Reports_zero_when_the_device_has_no_formats()
    {
        Assert.Equal(0u, DeviceChannelCount.Of([]));
    }

    private static MaDeviceInfo.NativeDataFormat Format(uint channels, uint flags) =>
        new() { Format = MaFormat.F32, Channels = channels, SampleRate = 48_000, Flags = flags };
}
