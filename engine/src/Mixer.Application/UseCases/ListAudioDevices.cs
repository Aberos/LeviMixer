using Mixer.Application.Devices;

namespace Mixer.Application.UseCases;

public sealed class ListAudioDevices(IAudioDeviceCatalog catalog)
{
    public AudioDeviceList Execute() => catalog.ListDevices();
}
