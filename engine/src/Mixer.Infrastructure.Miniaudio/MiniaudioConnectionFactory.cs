using Mixer.Application.Devices;
using Mixer.Dsp.Buffers;

namespace Mixer.Infrastructure.Miniaudio;

public sealed class MiniaudioConnectionFactory(MiniaudioContext context) : IAudioConnectionFactory
{
    public IAudioConnection Open(AudioConnectionRequest request, IAudioBlockProcessor processor)
    {
        ArgumentNullException.ThrowIfNull(request);
        var endpoints = context.Enumerate();
        var devices = new DeviceIdPair(endpoints.FindInput(request.Input.Id).NativeId, endpoints.FindOutput(request.Output.Id).NativeId);
        return MiniaudioDuplexConnection.Open(context, request, devices, processor);
    }
}
