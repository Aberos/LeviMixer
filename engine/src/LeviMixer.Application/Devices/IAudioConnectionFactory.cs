using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Application.Devices;

public interface IAudioConnectionFactory
{
    IAudioConnection Open(AudioConnectionRequest request, IAudioBlockProcessor processor);
}
