using System.Text.Json;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc;

internal static class InboundParser
{
    public static InboundCommand Parse(string line)
    {
        InboundEnvelope? envelope;
        try
        {
            envelope = JsonSerializer.Deserialize(line, IpcJsonContext.Default.InboundEnvelope);
        }
        catch (JsonException exception)
        {
            throw new InvalidMessageException($"Message is not valid JSON: {exception.Message}", exception);
        }

        return envelope is { Type: { Length: > 0 } type }
            ? new InboundCommand(type, envelope.RequestId, envelope.Payload)
            : throw new InvalidMessageException("Message must have a non-empty 'type'.");
    }
}
