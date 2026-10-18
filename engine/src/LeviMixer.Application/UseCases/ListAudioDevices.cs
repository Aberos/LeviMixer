using LeviMixer.Application.Devices;

namespace LeviMixer.Application.UseCases;

public sealed class ListAudioDevices(IAudioDeviceCatalog catalog)
{
    public AudioDeviceList Execute() => catalog.ListDevices();
}
