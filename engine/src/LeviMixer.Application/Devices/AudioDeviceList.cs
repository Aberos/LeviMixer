using System.Collections.Immutable;

namespace LeviMixer.Application.Devices;

public sealed record AudioDeviceList(ImmutableArray<AudioEndpoint> Inputs, ImmutableArray<AudioEndpoint> Outputs)
{
    public AudioEndpoint? FindInput(AudioDeviceId id) => Inputs.FirstOrDefault(input => input.Id == id);

    public AudioEndpoint? FindOutput(AudioDeviceId id) => Outputs.FirstOrDefault(output => output.Id == id);

    public AudioEndpoint? DefaultInput => Inputs.FirstOrDefault(input => input.IsDefault);

    public AudioEndpoint? DefaultOutput => Outputs.FirstOrDefault(output => output.IsDefault);
}
