using LeviMixer.Application.Devices;
using LeviMixer.Infrastructure.Miniaudio;
using LeviMixer.Infrastructure.SystemAudio.Linux;
using LeviMixer.Infrastructure.SystemAudio.Linux.Duplex;
using LeviMixer.Infrastructure.SystemAudio.Linux.Processes;
using LeviMixer.Infrastructure.SystemAudio.Windows;

namespace LeviMixer.Engine.Host.Composition;

internal static class PlatformAudioFactory
{
    public static PlatformAudio Create(MiniaudioContext context)
    {
        var deviceCatalog = new MiniaudioDeviceCatalog(context);
        var connections = new MiniaudioConnectionFactory(context);
        if (OperatingSystem.IsLinux())
        {
            return ForLinux(context, deviceCatalog, connections);
        }

        if (OperatingSystem.IsWindows())
        {
            return ForWindows(context, deviceCatalog, connections);
        }

        return new PlatformAudio(deviceCatalog, connections, new UnsupportedSystemAudioCapture(), null);
    }

    private static PlatformAudio ForLinux(MiniaudioContext context, MiniaudioDeviceCatalog deviceCatalog, MiniaudioConnectionFactory connections)
    {
        var commands = new ProcessCommandRunner();
        var capture = new PipeWireSystemAudioCapture(deviceCatalog, new MiniaudioCaptureFactory(context), commands);
        return new PlatformAudio(new PipeWireAwareDeviceCatalog(deviceCatalog, capture), new PipeWireConnectionFactory(connections, commands), capture, capture);
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static PlatformAudio ForWindows(MiniaudioContext context, MiniaudioDeviceCatalog deviceCatalog, MiniaudioConnectionFactory connections)
    {
        var capture = new VbCableSystemAudioCapture(
            deviceCatalog,
            new MiniaudioStereoCapture(new MiniaudioCaptureFactory(context)),
            new PolicyConfigDefaultOutput(context),
            new DefaultOutputJournal(DefaultOutputJournal.DefaultPath));
        return new PlatformAudio(new VbCableAwareDeviceCatalog(deviceCatalog, capture), connections, capture, capture);
    }
}
