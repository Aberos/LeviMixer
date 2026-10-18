using System.Text.Json;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc.Transport;

internal sealed class OutboundChannel(TextWriter writer)
{
    private readonly Lock _gate = new();

    public void Send(OutboundEnvelope message)
    {
        var line = JsonSerializer.Serialize(message, IpcJsonContext.Default.OutboundEnvelope);
        lock (_gate)
        {
            writer.WriteLine(line);
            writer.Flush();
        }
    }

    public void SendAll(IEnumerable<OutboundEnvelope> messages)
    {
        foreach (var message in messages)
        {
            Send(message);
        }
    }
}
