using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Infrastructure.SystemAudio.Windows;

public interface IStereoCapture
{
    IDisposable Open(AudioDeviceId input, StreamFormat format, ISampleWriter destination);
}
