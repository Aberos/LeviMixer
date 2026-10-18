using Mixer.Application.UseCases;
using Mixer.Domain.Channels;
using Mixer.Infrastructure.Ipc.Mapping;
using Mixer.Infrastructure.Ipc.Transport;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc.Handlers;

internal sealed class SetChannelGainHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "setChannelGain";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        var payload = command.Read(IpcJsonContext.Default.ChannelGainPayload);
        var result = useCases.SetChannelGain.Execute(new ChannelId(payload.ChannelId), DecibelsWire.FromWire(payload.GainDecibels));
        yield return OutboundMessage.Result(result, command.RequestId);
    }
}
