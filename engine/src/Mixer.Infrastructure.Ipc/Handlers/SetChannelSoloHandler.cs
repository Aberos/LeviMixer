using Mixer.Application.UseCases;
using Mixer.Domain.Channels;
using Mixer.Infrastructure.Ipc.Transport;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc.Handlers;

internal sealed class SetChannelSoloHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "setChannelSolo";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        var payload = command.Read(IpcJsonContext.Default.ChannelTogglePayload);
        yield return OutboundMessage.Result(useCases.SetChannelSolo.Execute(new ChannelId(payload.ChannelId), payload.Value), command.RequestId);
    }
}
