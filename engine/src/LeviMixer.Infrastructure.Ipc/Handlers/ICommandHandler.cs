using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc.Handlers;

internal interface ICommandHandler
{
    string Type { get; }

    IEnumerable<OutboundEnvelope> Handle(InboundCommand command);
}
