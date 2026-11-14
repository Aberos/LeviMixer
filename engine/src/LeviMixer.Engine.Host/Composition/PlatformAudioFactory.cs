using LeviMixer.Application.Devices;
using LeviMixer.Infrastructure.Miniaudio;
using LeviMixer.Infrastructure.SystemAudio.Linux;
using LeviMixer.Infrastructure.SystemAudio.Linux.Duplex;
using LeviMixer.Infrastructure.SystemAudio.Linux.Processes;

namespace LeviMixer.Engine.Host.Composition;

internal static class PlatformAudioFactory
{
    public static PlatformAudio Create(MiniaudioContext context)
    {
        var deviceCatalog = new MiniaudioDeviceCatalog(context);
        var connections = new MiniaudioConnectionFactory(context);
        return OperatingSystem.IsLinux()
            ? ForLinux(context, deviceCatalog, connections)
            : new PlatformAudio(deviceCatalog, connections, new UnsupportedSystemAudioCapture(), null);
    }

    private static PlatformAudio ForLinux(MiniaudioContext context, MiniaudioDeviceCatalog deviceCatalog, MiniaudioConnectionFactory connections)
    {
        var commands = new ProcessCommandRunner();
        var capture = new PipeWireSystemAudioCapture(deviceCatalog, new MiniaudioCaptureFactory(context), commands);
        return new PlatformAudio(new PipeWireAwareDeviceCatalog(deviceCatalog, capture), new PipeWireConnectionFactory(connections, commands), capture, capture);
    }
}
