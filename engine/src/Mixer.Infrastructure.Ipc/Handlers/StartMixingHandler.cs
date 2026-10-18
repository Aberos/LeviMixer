using Mixer.Application.UseCases;
using Mixer.Infrastructure.Ipc.Mapping;
using Mixer.Infrastructure.Ipc.Transport;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc.Handlers;

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
