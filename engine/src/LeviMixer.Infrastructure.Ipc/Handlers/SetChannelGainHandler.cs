using LeviMixer.Application.UseCases;
using LeviMixer.Domain.Channels;
using LeviMixer.Infrastructure.Ipc.Mapping;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc.Handlers;

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
