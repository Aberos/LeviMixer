using Mixer.Dsp.Buffers;

namespace Mixer.Application.Devices;

public interface IAudioConnectionFactory
{
    IAudioConnection Open(AudioConnectionRequest request, IAudioBlockProcessor processor);
}
