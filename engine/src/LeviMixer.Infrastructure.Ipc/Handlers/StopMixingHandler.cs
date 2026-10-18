using LeviMixer.Application.Results;
using LeviMixer.Application.UseCases;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc.Handlers;

internal sealed class StopMixingHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "stopMixing";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        useCases.StopMixing.Execute();
        return StateMessages.ResultFollowedByState(useCases, CommandResult.Success, command.RequestId);
    }
}
