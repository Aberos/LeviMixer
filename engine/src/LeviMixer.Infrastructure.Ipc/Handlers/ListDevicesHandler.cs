using LeviMixer.Application.UseCases;
using LeviMixer.Infrastructure.Ipc.Mapping;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc.Handlers;

internal sealed class ListDevicesHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "listDevices";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        var devices = WireMapper.ToWire(useCases.ListAudioDevices.Execute());
        yield return OutboundMessage.Create("devices", devices, IpcJsonContext.Default.DevicesPayload, command.RequestId);
    }
}
