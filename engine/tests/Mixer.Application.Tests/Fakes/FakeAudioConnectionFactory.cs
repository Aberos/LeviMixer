using Mixer.Application.Devices;
using Mixer.Dsp.Buffers;

namespace Mixer.Application.Tests.Fakes;

internal sealed class FakeAudioConnectionFactory : IAudioConnectionFactory
{
    private readonly List<FakeAudioConnection> _opened = [];

    public string? FailureReason { get; set; }

    public IReadOnlyList<FakeAudioConnection> Opened => _opened;

    public FakeAudioConnection Current => _opened[^1];

    public IAudioConnection Open(AudioConnectionRequest request, IAudioBlockProcessor processor)
    {
        if (FailureReason is not null)
        {
            throw new AudioDeviceException(FailureReason);
        }

        var connection = new FakeAudioConnection(request, processor);
        _opened.Add(connection);
        return connection;
    }
}
