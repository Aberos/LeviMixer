using System.Text.Json;

namespace LeviMixer.Infrastructure.Ipc.Wire;

internal sealed record OutboundEnvelope(string Type, string? RequestId, JsonElement? Payload);
