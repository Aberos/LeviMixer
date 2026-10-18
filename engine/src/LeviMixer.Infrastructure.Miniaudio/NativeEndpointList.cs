using System.Collections.Immutable;
using LeviMixer.Application.Devices;

namespace LeviMixer.Infrastructure.Miniaudio;

internal sealed record NativeEndpointList(ImmutableArray<NativeEndpoint> Inputs, ImmutableArray<NativeEndpoint> Outputs)
{
    public AudioDeviceList ToDeviceList() =>
        new([.. Inputs.Select(input => input.Endpoint)], [.. Outputs.Select(output => output.Endpoint)]);

    public NativeEndpoint FindInput(AudioDeviceId id) => Find(Inputs, id);

    public NativeEndpoint FindOutput(AudioDeviceId id) => Find(Outputs, id);

    private static NativeEndpoint Find(ImmutableArray<NativeEndpoint> endpoints, AudioDeviceId id) =>
        endpoints.FirstOrDefault(endpoint => endpoint.Endpoint.Id == id)
        ?? throw new AudioDeviceException($"Audio device '{id}' is no longer available.");
}
