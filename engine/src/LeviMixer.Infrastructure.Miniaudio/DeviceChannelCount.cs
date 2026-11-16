using Hexa.NET.MiniAudio;

namespace LeviMixer.Infrastructure.Miniaudio;

internal static class DeviceChannelCount
{
    public const uint HardwareFormatFlag = 1u << 1;

    public static uint Of(ReadOnlySpan<MaDeviceInfo.NativeDataFormat> formats)
    {
        var hardware = 0u;
        var any = 0u;
        foreach (var format in formats)
        {
            any = Math.Max(any, format.Channels);
            if ((format.Flags & HardwareFormatFlag) != 0)
            {
                hardware = Math.Max(hardware, format.Channels);
            }
        }

        return hardware != 0 ? hardware : any;
    }
}
