using Mixer.Application.UseCases;
using Mixer.Domain.Channels;
using Mixer.Infrastructure.Ipc.Transport;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc.Handlers;

internal sealed class SetChannelMuteHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "setChannelMute";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        var payload = command.Read(IpcJsonContext.Default.ChannelTogglePayload);
        yield return OutboundMessage.Result(useCases.SetChannelMute.Execute(new ChannelId(payload.ChannelId), payload.Value), command.RequestId);
    }
}
