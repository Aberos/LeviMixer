using System.Text.Json;

namespace Mixer.Infrastructure.Ipc.Wire;

internal sealed record InboundEnvelope(string? Type, string? RequestId, JsonElement? Payload);
