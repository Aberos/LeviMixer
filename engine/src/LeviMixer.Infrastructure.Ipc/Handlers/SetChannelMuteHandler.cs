using LeviMixer.Application.UseCases;
using LeviMixer.Domain.Channels;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc.Handlers;

internal sealed class SetChannelMuteHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "setChannelMute";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        var payload = command.Read(IpcJsonContext.Default.ChannelTogglePayload);
        yield return OutboundMessage.Result(useCases.SetChannelMute.Execute(new ChannelId(payload.ChannelId), payload.Value), command.RequestId);
    }
}
