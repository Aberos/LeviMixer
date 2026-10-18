using LeviMixer.Application.Results;
using LeviMixer.Application.UseCases;
using LeviMixer.Infrastructure.Ipc.Mapping;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc.Handlers;

internal static class StateMessages
{
    public static OutboundEnvelope MixerState(MixerUseCases useCases, string? requestId = null) =>
        OutboundMessage.Create("mixerState", WireMapper.ToWire(useCases.GetMixerStatus.Execute()), IpcJsonContext.Default.MixerStatePayload, requestId);

    public static IEnumerable<OutboundEnvelope> ResultFollowedByState(MixerUseCases useCases, CommandResult result, string? requestId)
    {
        yield return OutboundMessage.Result(result, requestId);
        if (result.IsSuccess)
        {
            yield return MixerState(useCases);
        }
    }
}
