using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Infrastructure.Miniaudio;

public sealed class MiniaudioCaptureFactory(MiniaudioContext context)
{
    public IDisposable OpenStereo(AudioDeviceId input, StreamFormat format, ISampleWriter destination)
    {
        var device = context.Enumerate().FindInput(input).NativeId;
        return MiniaudioCaptureConnection.Open(context, device, format, destination);
    }
}
