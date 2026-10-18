using System.Runtime.InteropServices;
using Hexa.NET.MiniAudio;
using Mixer.Application.Devices;

namespace Mixer.Infrastructure.Miniaudio;

internal static class MiniaudioResult
{
    public static void EnsureSuccess(MaResult result, string operation)
    {
        if (result != MaResult.Success)
        {
            throw new AudioDeviceException($"{operation} failed: {Describe(result)}.");
        }
    }

    private static unsafe string Describe(MaResult result) =>
        Marshal.PtrToStringUTF8((nint)MiniAudio.ResultDescription(result)) ?? result.ToString();
}
