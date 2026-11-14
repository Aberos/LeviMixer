using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;
using LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire.Graph;
using LeviMixer.Infrastructure.SystemAudio.Linux.Processes;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.Duplex;

public sealed class PipeWireConnectionFactory(IAudioConnectionFactory fallback, ICommandRunner commands) : IAudioConnectionFactory
{
    public IAudioConnection Open(AudioConnectionRequest request, IAudioBlockProcessor processor)
    {
        ArgumentNullException.ThrowIfNull(request);
        try
        {
            return PipeWireDuplexConnection.Open(request, processor, new PipeWireGraph(commands));
        }
        catch (Exception exception) when (exception is AudioDeviceException or DllNotFoundException or EntryPointNotFoundException)
        {
            return fallback.Open(request, processor);
        }
    }
}
