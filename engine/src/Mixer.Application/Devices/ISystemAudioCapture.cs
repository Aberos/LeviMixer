using Mixer.Dsp.Buffers;

namespace Mixer.Application.Devices;

public interface ISystemAudioCapture
{
    IDisposable Start(ISampleWriter destination, int sampleRate);
}
