using System.Text.Json;

namespace Mixer.Infrastructure.Ipc.Wire;

internal sealed record OutboundEnvelope(string Type, string? RequestId, JsonElement? Payload);
