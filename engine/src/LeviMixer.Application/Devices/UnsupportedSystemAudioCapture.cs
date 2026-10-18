using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Application.Devices;

public sealed class UnsupportedSystemAudioCapture : ISystemAudioCapture
{
    public IDisposable Start(ISampleWriter destination, int sampleRate) =>
        throw new AudioDeviceException("System audio capture is not available on this platform yet.");
}
