using Mixer.Application.UseCases;
using Mixer.Domain.Channels;
using Mixer.Infrastructure.Ipc.Transport;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc.Handlers;

internal sealed class SetChannelPanHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "setChannelPan";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        var payload = command.Read(IpcJsonContext.Default.ChannelPanPayload);
        yield return OutboundMessage.Result(useCases.SetChannelPan.Execute(new ChannelId(payload.ChannelId), new Pan(payload.Pan)), command.RequestId);
    }
}
