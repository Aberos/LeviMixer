using LeviMixer.Application.Devices;
using LeviMixer.Infrastructure.Miniaudio;
using LeviMixer.Infrastructure.SystemAudio.Linux;
using LeviMixer.Infrastructure.SystemAudio.Linux.Processes;

namespace LeviMixer.Engine.Host.Composition;

internal static class PlatformAudioFactory
{
    public static PlatformAudio Create(MiniaudioContext context)
    {
        var deviceCatalog = new MiniaudioDeviceCatalog(context);
        return OperatingSystem.IsLinux()
            ? ForLinux(context, deviceCatalog)
            : new PlatformAudio(deviceCatalog, new UnsupportedSystemAudioCapture(), null);
    }

    private static PlatformAudio ForLinux(MiniaudioContext context, MiniaudioDeviceCatalog deviceCatalog)
    {
        var capture = new PipeWireSystemAudioCapture(deviceCatalog, new MiniaudioCaptureFactory(context), new ProcessCommandRunner());
        return new PlatformAudio(new PipeWireAwareDeviceCatalog(deviceCatalog, capture), capture, capture);
    }
}
