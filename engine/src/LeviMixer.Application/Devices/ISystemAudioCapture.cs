using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Application.Devices;

public interface ISystemAudioCapture
{
    IDisposable Start(ISampleWriter destination, StreamFormat format);
}
