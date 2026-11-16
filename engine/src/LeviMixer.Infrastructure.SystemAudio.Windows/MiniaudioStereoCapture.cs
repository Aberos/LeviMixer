using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;
using LeviMixer.Infrastructure.Miniaudio;

namespace LeviMixer.Infrastructure.SystemAudio.Windows;

public sealed class MiniaudioStereoCapture(MiniaudioCaptureFactory captures) : IStereoCapture
{
    public IDisposable Open(AudioDeviceId input, StreamFormat format, ISampleWriter destination) =>
        captures.OpenStereo(input, format, destination);
}
