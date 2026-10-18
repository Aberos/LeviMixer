using Mixer.Application.Devices;
using Mixer.Infrastructure.Miniaudio;
using Mixer.Infrastructure.SystemAudio.Linux;
using Mixer.Infrastructure.SystemAudio.Linux.Processes;

namespace Mixer.Engine.Host.Composition;

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
