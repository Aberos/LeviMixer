using LeviMixer.Application.UseCases;
using LeviMixer.Infrastructure.Ipc.Mapping;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc.Handlers;

internal sealed class SetMasterGainHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "setMasterGain";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        var payload = command.Read(IpcJsonContext.Default.MasterGainPayload);
        yield return OutboundMessage.Result(useCases.SetMasterGain.Execute(DecibelsWire.FromWire(payload.GainDecibels)), command.RequestId);
    }
}
