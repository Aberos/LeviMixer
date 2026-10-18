using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace LeviMixer.Infrastructure.Ipc.Transport;

internal sealed record InboundCommand(string Type, string? RequestId, JsonElement? Payload)
{
    public TPayload Read<TPayload>(JsonTypeInfo<TPayload> typeInfo)
    {
        if (Payload is not { } payload)
        {
            throw new InvalidMessageException($"Command '{Type}' requires a payload.");
        }

        return payload.Deserialize(typeInfo) ?? throw new InvalidMessageException($"Command '{Type}' has an empty payload.");
    }
}
