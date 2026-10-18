using Mixer.Application.Results;
using Mixer.Application.UseCases;
using Mixer.Infrastructure.Ipc.Transport;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc.Handlers;

internal sealed class StopMixingHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "stopMixing";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        useCases.StopMixing.Execute();
        return StateMessages.ResultFollowedByState(useCases, CommandResult.Success, command.RequestId);
    }
}
