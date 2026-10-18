using Mixer.Application.UseCases;
using Mixer.Infrastructure.Ipc.Mapping;
using Mixer.Infrastructure.Ipc.Transport;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc.Handlers;

internal sealed class ListDevicesHandler(MixerUseCases useCases) : ICommandHandler
{
    public string Type => "listDevices";

    public IEnumerable<OutboundEnvelope> Handle(InboundCommand command)
    {
        var devices = WireMapper.ToWire(useCases.ListAudioDevices.Execute());
        yield return OutboundMessage.Create("devices", devices, IpcJsonContext.Default.DevicesPayload, command.RequestId);
    }
}
