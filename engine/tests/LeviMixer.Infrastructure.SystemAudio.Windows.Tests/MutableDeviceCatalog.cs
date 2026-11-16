using System.Collections.Immutable;
using LeviMixer.Application.Devices;

namespace LeviMixer.Infrastructure.SystemAudio.Windows.Tests;

internal sealed class MutableDeviceCatalog : IAudioDeviceCatalog
{
    public ImmutableArray<AudioEndpoint> Inputs { get; set; } = [WindowsDevices.InterfaceInput, WindowsDevices.CableCapture];

    public ImmutableArray<AudioEndpoint> Outputs { get; set; } = [WindowsDevices.InterfaceOutput, WindowsDevices.Monitor, WindowsDevices.CablePlayback];

    public AudioDeviceList ListDevices() => new(Inputs, Outputs);

    public void MakeDefaultOutput(AudioEndpoint output) =>
        Outputs = [.. Outputs.Select(endpoint => endpoint with { IsDefault = endpoint.Id == output.Id })];

    public void RemoveCable()
    {
        Inputs = [.. Inputs.Where(input => !VbCable.IsCableEndpoint(input.Name))];
        Outputs = [.. Outputs.Where(output => !VbCable.IsCableEndpoint(output.Name))];
    }
}
