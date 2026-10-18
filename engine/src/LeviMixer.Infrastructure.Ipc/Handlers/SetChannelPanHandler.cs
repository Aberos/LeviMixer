using LeviMixer.Application.UseCases;
using LeviMixer.Domain.Channels;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc.Handlers;

internal sealed class SetChannelPanHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "setChannelPan";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        var payload = command.Read(IpcJsonContext.Default.ChannelPanPayload);
        yield return OutboundMessage.Result(useCases.SetChannelPan.Execute(new ChannelId(payload.ChannelId), new Pan(payload.Pan)), command.RequestId);
    }
}
