using Mixer.Application.Devices;

namespace Mixer.Infrastructure.Miniaudio;

public sealed class MiniaudioDeviceCatalog(MiniaudioContext context) : IAudioDeviceCatalog
{
    public AudioDeviceList ListDevices() => context.Enumerate().ToDeviceList();
}
