using Mixer.Application.Devices;
using Mixer.Infrastructure.SystemAudio.Linux.PipeWire;

namespace Mixer.Infrastructure.SystemAudio.Linux;

public sealed class PipeWireAwareDeviceCatalog(IAudioDeviceCatalog inner, IDefaultOutputMemory defaultOutput) : IAudioDeviceCatalog
{
    public AudioDeviceList ListDevices()
    {
        var devices = inner.ListDevices();
        var inputs = devices.Inputs.Where(IsVisible);
        var outputs = devices.Outputs.Where(IsVisible).Select(MarkOriginalDefault);
        return new AudioDeviceList([.. inputs], [.. outputs]);
    }

    private static bool IsVisible(AudioEndpoint endpoint) => !VirtualSink.IsVirtualEndpoint(endpoint.Name);

    private AudioEndpoint MarkOriginalDefault(AudioEndpoint endpoint) =>
        defaultOutput.OriginalDefaultOutputName is { } original ? endpoint with { IsDefault = endpoint.Name == original } : endpoint;
}
