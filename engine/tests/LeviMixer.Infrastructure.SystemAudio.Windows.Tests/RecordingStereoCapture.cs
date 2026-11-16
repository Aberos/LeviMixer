using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Infrastructure.SystemAudio.Windows.Tests;

internal sealed class RecordingStereoCapture : IStereoCapture
{
    public List<AudioDeviceId> Opened { get; } = [];

    public IDisposable Open(AudioDeviceId input, StreamFormat format, ISampleWriter destination)
    {
        Opened.Add(input);
        return new MemoryStream();
    }
}
