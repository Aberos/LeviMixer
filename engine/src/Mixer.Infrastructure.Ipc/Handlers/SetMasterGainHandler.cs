using Mixer.Application.UseCases;
using Mixer.Infrastructure.Ipc.Mapping;
using Mixer.Infrastructure.Ipc.Transport;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc.Handlers;

internal sealed class SetMasterGainHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "setMasterGain";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        var payload = command.Read(IpcJsonContext.Default.MasterGainPayload);
        yield return OutboundMessage.Result(useCases.SetMasterGain.Execute(DecibelsWire.FromWire(payload.GainDecibels)), command.RequestId);
    }
}
