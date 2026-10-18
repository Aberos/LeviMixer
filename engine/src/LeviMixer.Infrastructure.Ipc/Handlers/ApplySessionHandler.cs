using LeviMixer.Application.Settings;
using LeviMixer.Application.UseCases;
using LeviMixer.Infrastructure.Ipc.Mapping;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc.Handlers;

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
