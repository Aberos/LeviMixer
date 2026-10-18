using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Mixer.Application.Results;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc.Transport;

internal static class OutboundMessage
{
    public static OutboundEnvelope Create<TPayload>(string type, TPayload payload, JsonTypeInfo<TPayload> typeInfo, string? requestId = null) =>
        new(type, requestId, JsonSerializer.SerializeToElement(payload, typeInfo));

    public static OutboundEnvelope Ack(string? requestId) => new("ack", requestId, null);

    public static OutboundEnvelope Error(CommandError error, string? requestId) =>
        Create("error", new ErrorPayload(error.Code, error.Message), IpcJsonContext.Default.ErrorPayload, requestId);

    public static OutboundEnvelope Result(CommandResult result, string? requestId) =>
        result.Error is { } error ? Error(error, requestId) : Ack(requestId);
}
