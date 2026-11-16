using LeviMixer.Application.Devices;

namespace LeviMixer.Infrastructure.SystemAudio.Windows.Tests;

internal sealed class RecordingDefaultOutputControl(MutableDeviceCatalog catalog) : IDefaultOutputControl
{
    public DefaultOutputs Defaults { get; set; } = WindowsDevices.InterfaceAsDefault;

    public List<AudioDeviceId> PointedTo { get; } = [];

    public List<DefaultOutputs> Restored { get; } = [];

    public bool FailsToSwitch { get; set; }

    public DefaultOutputs Current() => Defaults;

    public void PointAllTo(AudioDeviceId output)
    {
        if (FailsToSwitch)
        {
            throw new AudioDeviceException("Changing the default output failed.");
        }

        PointedTo.Add(output);
        catalog.MakeDefaultOutput(catalog.Outputs.Single(endpoint => endpoint.Id == output));
    }

    public void Restore(DefaultOutputs previous)
    {
        Restored.Add(previous);
        Defaults = previous;
    }
}
