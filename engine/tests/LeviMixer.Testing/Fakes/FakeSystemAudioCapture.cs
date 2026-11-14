using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Testing.Fakes;

public sealed class FakeSystemAudioCapture : ISystemAudioCapture
{
    private readonly List<FakeSystemAudioFeed> _started = [];

    public string? FailureReason { get; set; }

    public IReadOnlyList<FakeSystemAudioFeed> Started => _started;

    public IDisposable Start(ISampleWriter destination, StreamFormat format)
    {
        if (FailureReason is not null)
        {
            throw new AudioDeviceException(FailureReason);
        }

        var feed = new FakeSystemAudioFeed(destination, format);
        _started.Add(feed);
        return feed;
    }
}
