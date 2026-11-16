using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Infrastructure.SystemAudio.Windows.Tests;

public sealed class VbCableAwareDeviceCatalogTests : IDisposable
{
    private readonly MutableDeviceCatalog _inner = new();
    private readonly TemporaryJournal _journal = new();
    private readonly VbCableSystemAudioCapture _capture;

    public VbCableAwareDeviceCatalogTests()
    {
        _capture = new VbCableSystemAudioCapture(_inner, new RecordingStereoCapture(), new RecordingDefaultOutputControl(_inner), _journal.Journal);
    }

    [Fact]
    public void Hides_the_cable_endpoints()
    {
        var devices = new VbCableAwareDeviceCatalog(_inner, _capture).ListDevices();

        Assert.DoesNotContain(devices.Inputs, input => VbCable.IsCableEndpoint(input.Name));
        Assert.DoesNotContain(devices.Outputs, output => VbCable.IsCableEndpoint(output.Name));
    }

    [Fact]
    public void Keeps_the_original_output_as_default_while_the_cable_is_the_system_default()
    {
        _capture.Start(new SpscRingBuffer(1_024), StreamFormat.Default);

        var devices = new VbCableAwareDeviceCatalog(_inner, _capture).ListDevices();

        Assert.Equal(WindowsDevices.InterfaceOutput.Id, devices.DefaultOutput?.Id);
    }

    [Fact]
    public void Falls_back_to_the_first_output_when_the_cable_was_already_the_default()
    {
        _inner.MakeDefaultOutput(WindowsDevices.CablePlayback);

        var devices = new VbCableAwareDeviceCatalog(_inner, _capture).ListDevices();

        Assert.Equal(WindowsDevices.InterfaceOutput.Id, devices.DefaultOutput?.Id);
        Assert.Single(devices.Outputs, output => output.IsDefault);
    }

    public void Dispose()
    {
        _capture.Dispose();
        _journal.Dispose();
    }
}
