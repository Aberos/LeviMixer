using LeviMixer.Application.UseCases;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc.Handlers;

internal sealed class GetStateHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "getState";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        yield return StateMessages.MixerState(useCases, command.RequestId);
    }
}
