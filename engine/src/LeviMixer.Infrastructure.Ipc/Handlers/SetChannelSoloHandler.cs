using LeviMixer.Application.UseCases;
using LeviMixer.Domain.Channels;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc.Handlers;

internal sealed class SetChannelSoloHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "setChannelSolo";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        var payload = command.Read(IpcJsonContext.Default.ChannelTogglePayload);
        yield return OutboundMessage.Result(useCases.SetChannelSolo.Execute(new ChannelId(payload.ChannelId), payload.Value), command.RequestId);
    }
}
