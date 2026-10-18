using System.Collections.Immutable;
using Mixer.Application.Devices;

namespace Mixer.Testing.Fakes;

public sealed class FakeAudioDeviceCatalog : IAudioDeviceCatalog
{
    public static AudioEndpoint TwoChannelInterfaceInput { get; } = new(new AudioDeviceId("umc22-in"), "UMC22", 2, true);

    public static AudioEndpoint FourChannelInterfaceInput { get; } = new(new AudioDeviceId("big-in"), "Big Interface", 4, false);

    public static AudioEndpoint StereoOutput { get; } = new(new AudioDeviceId("umc22-out"), "UMC22", 2, true);

    public AudioDeviceList ListDevices() =>
        new([TwoChannelInterfaceInput, FourChannelInterfaceInput], ImmutableArray.Create(StereoOutput));
}
