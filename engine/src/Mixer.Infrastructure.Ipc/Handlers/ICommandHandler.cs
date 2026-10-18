using Mixer.Infrastructure.Ipc.Transport;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc.Handlers;

internal interface ICommandHandler
{
    string Type { get; }

    IEnumerable<OutboundEnvelope> Handle(InboundCommand command);
}
