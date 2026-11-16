using LeviMixer.Application.Devices;

namespace LeviMixer.Infrastructure.SystemAudio.Windows;

public sealed class VbCableAwareDeviceCatalog(IAudioDeviceCatalog inner, VbCableSystemAudioCapture capture) : IAudioDeviceCatalog
{
    public AudioDeviceList ListDevices()
    {
        var devices = inner.ListDevices();
        var inputs = devices.Inputs.Where(IsVisible);
        var outputs = devices.Outputs.Where(IsVisible).ToList();
        return new AudioDeviceList([.. inputs], [.. MarkDefault(outputs)]);
    }

    private static bool IsVisible(AudioEndpoint endpoint) => !VbCable.IsCableEndpoint(endpoint.Name);

    private IEnumerable<AudioEndpoint> MarkDefault(List<AudioEndpoint> outputs)
    {
        if (capture.OriginalDefaultOutputName is { } original && outputs.Exists(output => output.Name == original))
        {
            return outputs.Select(output => output with { IsDefault = output.Name == original });
        }

        if (outputs.Count > 0 && !outputs.Exists(output => output.IsDefault))
        {
            return outputs.Select((output, index) => output with { IsDefault = index == 0 });
        }

        return outputs;
    }
}
