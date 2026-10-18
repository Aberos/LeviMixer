using LeviMixer.Application.Devices;

namespace LeviMixer.Infrastructure.Miniaudio;

public sealed class MiniaudioDeviceCatalog(MiniaudioContext context) : IAudioDeviceCatalog
{
    public AudioDeviceList ListDevices() => context.Enumerate().ToDeviceList();
}
