using Mixer.Application.UseCases;
using Mixer.Domain.Channels;
using Mixer.Infrastructure.Ipc.Transport;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc.Handlers;

internal sealed class RenameChannelHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "renameChannel";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        var payload = command.Read(IpcJsonContext.Default.ChannelNamePayload);
        var result = useCases.RenameChannel.Execute(new ChannelId(payload.ChannelId), payload.Name);
        return StateMessages.ResultFollowedByState(useCases, result, command.RequestId);
    }
}
