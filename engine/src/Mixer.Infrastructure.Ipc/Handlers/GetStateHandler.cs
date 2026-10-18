using Mixer.Application.UseCases;
using Mixer.Infrastructure.Ipc.Transport;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc.Handlers;

internal sealed class GetStateHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "getState";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        yield return StateMessages.MixerState(useCases, command.RequestId);
    }
}
