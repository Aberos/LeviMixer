using Mixer.Application.Results;
using Mixer.Application.UseCases;
using Mixer.Infrastructure.Ipc.Mapping;
using Mixer.Infrastructure.Ipc.Transport;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc.Handlers;

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
