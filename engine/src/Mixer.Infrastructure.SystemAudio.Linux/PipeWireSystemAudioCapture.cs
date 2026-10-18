using Mixer.Application.Devices;
using Mixer.Dsp.Buffers;
using Mixer.Infrastructure.Miniaudio;
using Mixer.Infrastructure.SystemAudio.Linux.PipeWire;
using Mixer.Infrastructure.SystemAudio.Linux.Processes;

namespace Mixer.Infrastructure.SystemAudio.Linux;

public sealed class PipeWireSystemAudioCapture : ISystemAudioCapture, IDefaultOutputMemory, IDisposable
{
    private static readonly TimeSpan MonitorWaitTimeout = TimeSpan.FromSeconds(3);
    private static readonly TimeSpan MonitorPollInterval = TimeSpan.FromMilliseconds(100);

    private readonly IAudioDeviceCatalog _devices;
    private readonly MiniaudioCaptureFactory _captures;
    private readonly DefaultSinkSetting _defaultSink;
    private readonly Lock _gate = new();
    private VirtualSinkProcess? _sink;
    private string? _previousDefaultSink;

    public PipeWireSystemAudioCapture(IAudioDeviceCatalog devices, MiniaudioCaptureFactory captures, ICommandRunner commands)
    {
        _devices = devices;
        _captures = captures;
        _defaultSink = new DefaultSinkSetting(commands);
    }

    public string? OriginalDefaultOutputName { get; private set; }

    public IDisposable Start(ISampleWriter destination, int sampleRate)
    {
        var monitor = EnsureVirtualSink();
        return _captures.OpenStereo(monitor.Id, sampleRate, destination);
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_sink is null)
            {
                return;
            }

            _defaultSink.Restore(_previousDefaultSink);
            _sink.Dispose();
            _sink = null;
        }
    }

    private AudioEndpoint EnsureVirtualSink()
    {
        lock (_gate)
        {
            if (_sink is null)
            {
                OriginalDefaultOutputName = _devices.ListDevices().DefaultOutput?.Name;
                _previousDefaultSink = _defaultSink.Read();
                _sink = VirtualSinkProcess.Start();
                _defaultSink.PointTo(VirtualSink.NodeName);
            }

            return WaitForMonitor();
        }
    }

    private AudioEndpoint WaitForMonitor()
    {
        var deadline = DateTime.UtcNow + MonitorWaitTimeout;
        while (DateTime.UtcNow < deadline)
        {
            if (_devices.ListDevices().Inputs.FirstOrDefault(input => VirtualSink.IsMonitor(input.Name)) is { } monitor)
            {
                return monitor;
            }

            Thread.Sleep(MonitorPollInterval);
        }

        throw new AudioDeviceException("The PipeWire virtual sink did not appear in time.");
    }
}
