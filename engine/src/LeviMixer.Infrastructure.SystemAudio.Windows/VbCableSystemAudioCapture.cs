using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Infrastructure.SystemAudio.Windows;

public sealed class VbCableSystemAudioCapture : ISystemAudioCapture, IDisposable
{
    private readonly IAudioDeviceCatalog _devices;
    private readonly IStereoCapture _capture;
    private readonly IDefaultOutputControl _defaultOutput;
    private readonly DefaultOutputJournal _journal;
    private readonly Lock _gate = new();
    private DefaultOutputs? _previous;

    public VbCableSystemAudioCapture(IAudioDeviceCatalog devices, IStereoCapture capture, IDefaultOutputControl defaultOutput, DefaultOutputJournal journal)
    {
        _devices = devices;
        _capture = capture;
        _defaultOutput = defaultOutput;
        _journal = journal;
        RecoverInterruptedSession();
    }

    public string? OriginalDefaultOutputName { get; private set; }

    public IDisposable Start(ISampleWriter destination, StreamFormat format)
    {
        var cableCapture = RouteSystemAudioThroughCable();
        return _capture.Open(cableCapture.Id, format, destination);
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_previous is null)
            {
                return;
            }

            var previous = _previous;
            _previous = null;
            TryRestore(previous);
        }
    }

    private AudioEndpoint RouteSystemAudioThroughCable()
    {
        lock (_gate)
        {
            var devices = _devices.ListDevices();
            var cablePlayback = devices.Outputs.FirstOrDefault(output => VbCable.IsPlayback(output.Name));
            var cableCapture = devices.Inputs.FirstOrDefault(input => VbCable.IsCapture(input.Name));
            if (cablePlayback is null || cableCapture is null)
            {
                throw new MissingAudioDriverException($"Install VB-Cable ({VbCable.DownloadUrl}) to use the PC channel on Windows.");
            }

            if (_previous is null)
            {
                PointDefaultOutputTo(cablePlayback, devices.DefaultOutput);
            }

            return cableCapture;
        }
    }

    private void PointDefaultOutputTo(AudioEndpoint cablePlayback, AudioEndpoint? currentDefault)
    {
        if (currentDefault is not null && !VbCable.IsCableEndpoint(currentDefault.Name))
        {
            OriginalDefaultOutputName = currentDefault.Name;
        }

        var previous = _defaultOutput.Current();
        _journal.Save(previous);
        try
        {
            _defaultOutput.PointAllTo(cablePlayback.Id);
        }
        catch
        {
            _journal.Clear();
            throw;
        }

        _previous = previous;
    }

    private void RecoverInterruptedSession()
    {
        if (_journal.Pending() is { IsEmpty: false } pending)
        {
            TryRestore(pending);
        }
    }

    private void TryRestore(DefaultOutputs previous)
    {
        try
        {
            _defaultOutput.Restore(previous);
            _journal.Clear();
        }
        catch (AudioDeviceException)
        {
        }
    }
}
