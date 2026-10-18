using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Infrastructure.Miniaudio;

public sealed class MiniaudioCaptureFactory(MiniaudioContext context)
{
    public IDisposable OpenStereo(AudioDeviceId input, int sampleRate, ISampleWriter destination)
    {
        var device = context.Enumerate().FindInput(input).NativeId;
        return MiniaudioCaptureConnection.Open(context, device, sampleRate, destination);
    }
}
