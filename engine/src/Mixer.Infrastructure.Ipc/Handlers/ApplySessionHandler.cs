using Mixer.Application.Settings;
using Mixer.Application.UseCases;
using Mixer.Infrastructure.Ipc.Mapping;
using Mixer.Infrastructure.Ipc.Transport;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc.Handlers;

internal sealed class ApplySessionHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "applySession";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        var payload = command.Read(IpcJsonContext.Default.ApplySessionPayload);
        var settings = new SessionSettings(
            WireMapper.DeviceIdFrom(payload.InputDeviceId),
            WireMapper.DeviceIdFrom(payload.OutputDeviceId),
            WireMapper.FormatFrom(payload.SampleRate, payload.BufferFrames),
            [.. (payload.Channels ?? []).Select(WireMapper.FromWire)],
            DecibelsWire.FromWire(payload.MasterGainDecibels));
        return StateMessages.ResultFollowedByState(useCases, useCases.ApplySession.Execute(settings), command.RequestId);
    }
}
