using System.Runtime.Versioning;
using LeviMixer.Application.Devices;
using LeviMixer.Infrastructure.Miniaudio;
using LeviMixer.Infrastructure.SystemAudio.Windows.Native;

namespace LeviMixer.Infrastructure.SystemAudio.Windows;

[SupportedOSPlatform("windows")]
public sealed class PolicyConfigDefaultOutput(MiniaudioContext context) : IDefaultOutputControl
{
    private const string WasapiEndpointPrefix = "{0.0.0.";

    public DefaultOutputs Current() => new(
        WindowsAudioEndpoints.DefaultOutputOf(WindowsAudioEndpoints.ConsoleRole),
        WindowsAudioEndpoints.DefaultOutputOf(WindowsAudioEndpoints.MultimediaRole),
        WindowsAudioEndpoints.DefaultOutputOf(WindowsAudioEndpoints.CommunicationsRole));

    public void PointAllTo(AudioDeviceId output)
    {
        var endpointId = context.WasapiOutputIdOf(output);
        if (!endpointId.StartsWith(WasapiEndpointPrefix, StringComparison.Ordinal))
        {
            throw new AudioDeviceException("Switching the default output requires the WASAPI audio backend.");
        }

        Restore(new DefaultOutputs(endpointId, endpointId, endpointId));
    }

    public void Restore(DefaultOutputs previous)
    {
        ArgumentNullException.ThrowIfNull(previous);
        SetIfKnown(previous.Console, WindowsAudioEndpoints.ConsoleRole);
        SetIfKnown(previous.Multimedia, WindowsAudioEndpoints.MultimediaRole);
        SetIfKnown(previous.Communications, WindowsAudioEndpoints.CommunicationsRole);
    }

    private static void SetIfKnown(string? endpointId, int role)
    {
        if (endpointId is not null)
        {
            WindowsAudioEndpoints.SetDefaultOutput(endpointId, role);
        }
    }
}
