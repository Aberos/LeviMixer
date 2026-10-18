using System.Runtime.InteropServices;
using Hexa.NET.MiniAudio;

namespace Mixer.Infrastructure.Miniaudio;

internal sealed unsafe class NativeDuplexDevice : IDisposable
{
    private const int NativeDeviceSize = 16 * 1024;

    private MaDevice* _device;
    private MaDeviceId* _deviceIds;

    public NativeDuplexDevice(DuplexDevicePair devices)
    {
        _device = (MaDevice*)NativeMemory.AllocZeroed(NativeDeviceSize);
        _deviceIds = (MaDeviceId*)NativeMemory.Alloc((nuint)(2 * sizeof(MaDeviceId)));
        _deviceIds[0] = devices.Capture;
        _deviceIds[1] = devices.Playback;
    }

    public MaDevice* Device => _device;

    public MaDeviceId* CaptureId => _deviceIds;

    public MaDeviceId* PlaybackId => _deviceIds + 1;

    public void Dispose()
    {
        NativeMemory.Free(_device);
        NativeMemory.Free(_deviceIds);
        _device = null;
        _deviceIds = null;
    }
}
