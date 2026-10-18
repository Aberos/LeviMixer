using LeviMixer.Application.UseCases;
using LeviMixer.Infrastructure.Ipc.Mapping;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc.Handlers;

internal sealed class StartMixingHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "startMixing";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        var payload = command.Read(IpcJsonContext.Default.StartMixingPayload);
        var request = new StartMixingRequest(
            WireMapper.DeviceIdFrom(payload.InputDeviceId),
            WireMapper.DeviceIdFrom(payload.OutputDeviceId),
            WireMapper.FormatFrom(payload.SampleRate, payload.BufferFrames));
        return StateMessages.ResultFollowedByState(useCases, useCases.StartMixing.Execute(request), command.RequestId);
    }
}
