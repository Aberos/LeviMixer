using LeviMixer.Application.UseCases;
using LeviMixer.Domain.Channels;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc.Handlers;

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
