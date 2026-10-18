using System.Text.Json.Serialization;

namespace LeviMixer.Infrastructure.Ipc.Wire;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.Never)]
[JsonSerializable(typeof(InboundEnvelope))]
[JsonSerializable(typeof(OutboundEnvelope))]
[JsonSerializable(typeof(StartMixingPayload))]
[JsonSerializable(typeof(ApplySessionPayload))]
[JsonSerializable(typeof(ChannelGainPayload))]
[JsonSerializable(typeof(ChannelTogglePayload))]
[JsonSerializable(typeof(ChannelPanPayload))]
[JsonSerializable(typeof(ChannelNamePayload))]
[JsonSerializable(typeof(MasterGainPayload))]
[JsonSerializable(typeof(ReadyPayload))]
[JsonSerializable(typeof(ErrorPayload))]
[JsonSerializable(typeof(DevicesPayload))]
[JsonSerializable(typeof(MixerStatePayload))]
[JsonSerializable(typeof(MetersPayload))]
internal sealed partial class IpcJsonContext : JsonSerializerContext;
