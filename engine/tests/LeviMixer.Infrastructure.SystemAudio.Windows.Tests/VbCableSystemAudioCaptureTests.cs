using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Infrastructure.SystemAudio.Windows.Tests;

public sealed class VbCableSystemAudioCaptureTests : IDisposable
{
    private readonly MutableDeviceCatalog _catalog = new();
    private readonly RecordingStereoCapture _capture = new();
    private readonly TemporaryJournal _journal = new();
    private readonly RecordingDefaultOutputControl _defaultOutput;

    public VbCableSystemAudioCaptureTests()
    {
        _defaultOutput = new RecordingDefaultOutputControl(_catalog);
    }

    [Fact]
    public void Captures_the_cable_output()
    {
        using var systemAudio = CreateCapture();

        systemAudio.Start(new SpscRingBuffer(1_024), StreamFormat.Default);

        Assert.Equal([WindowsDevices.CableCapture.Id], _capture.Opened);
    }

    [Fact]
    public void Points_the_default_output_to_the_cable_and_remembers_the_original()
    {
        using var systemAudio = CreateCapture();

        systemAudio.Start(new SpscRingBuffer(1_024), StreamFormat.Default);

        Assert.Equal([WindowsDevices.CablePlayback.Id], _defaultOutput.PointedTo);
        Assert.Equal(WindowsDevices.InterfaceOutput.Name, systemAudio.OriginalDefaultOutputName);
        Assert.Equal(WindowsDevices.InterfaceAsDefault, _journal.Journal.Pending());
    }

    [Fact]
    public void Switches_the_default_output_only_once_across_restarts()
    {
        using var systemAudio = CreateCapture();

        systemAudio.Start(new SpscRingBuffer(1_024), StreamFormat.Default);
        systemAudio.Start(new SpscRingBuffer(1_024), StreamFormat.Default);

        Assert.Single(_defaultOutput.PointedTo);
        Assert.Equal(WindowsDevices.InterfaceOutput.Name, systemAudio.OriginalDefaultOutputName);
    }

    [Fact]
    public void Disposing_restores_the_previous_default_and_clears_the_journal()
    {
        var systemAudio = CreateCapture();
        systemAudio.Start(new SpscRingBuffer(1_024), StreamFormat.Default);

        systemAudio.Dispose();

        Assert.Equal([WindowsDevices.InterfaceAsDefault], _defaultOutput.Restored);
        Assert.Null(_journal.Journal.Pending());
    }

    [Fact]
    public void Disposing_without_starting_leaves_the_default_alone()
    {
        CreateCapture().Dispose();

        Assert.Empty(_defaultOutput.Restored);
    }

    [Fact]
    public void Restores_the_default_left_behind_by_a_crashed_engine()
    {
        _journal.Journal.Save(WindowsDevices.InterfaceAsDefault);

        using var systemAudio = CreateCapture();

        Assert.Equal([WindowsDevices.InterfaceAsDefault], _defaultOutput.Restored);
        Assert.Null(_journal.Journal.Pending());
    }

    [Fact]
    public void Missing_cable_explains_how_to_install_it()
    {
        _catalog.RemoveCable();
        using var systemAudio = CreateCapture();

        var error = Assert.Throws<MissingAudioDriverException>(() => systemAudio.Start(new SpscRingBuffer(1_024), StreamFormat.Default));

        Assert.Contains("VB-Cable", error.Message, StringComparison.Ordinal);
        Assert.Empty(_defaultOutput.PointedTo);
    }

    [Fact]
    public void Failed_switch_does_not_leave_a_journal_behind()
    {
        _defaultOutput.FailsToSwitch = true;
        using var systemAudio = CreateCapture();

        Assert.Throws<AudioDeviceException>(() => systemAudio.Start(new SpscRingBuffer(1_024), StreamFormat.Default));

        Assert.Null(_journal.Journal.Pending());
    }

    public void Dispose() => _journal.Dispose();

    private VbCableSystemAudioCapture CreateCapture() => new(_catalog, _capture, _defaultOutput, _journal.Journal);
}
