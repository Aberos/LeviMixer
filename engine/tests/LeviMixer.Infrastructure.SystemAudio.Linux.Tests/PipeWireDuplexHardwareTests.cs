using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;
using LeviMixer.Infrastructure.Miniaudio;
using LeviMixer.Infrastructure.SystemAudio.Linux.Duplex;
using LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire.Graph;
using LeviMixer.Infrastructure.SystemAudio.Linux.Processes;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.Tests;

[Trait("Category", "Hardware")]
public sealed class PipeWireDuplexHardwareTests
{
    [Fact]
    public void Native_duplex_runs_at_the_requested_quantum()
    {
        using var context = new MiniaudioContext();
        var devices = new MiniaudioDeviceCatalog(context).ListDevices();
        var request = new AudioConnectionRequest(devices.DefaultInput!, devices.DefaultOutput!, new StreamFormat(48_000, 128));
        var processor = new CountingProcessor();

        using (PipeWireDuplexConnection.Open(request, processor, new PipeWireGraph(new ProcessCommandRunner())))
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
        }

        Assert.True(processor.Blocks > 10);
        Assert.Equal(128, processor.LastFrameCount);
    }

    private sealed class CountingProcessor : IAudioBlockProcessor
    {
        public int Blocks { get; private set; }

        public int LastFrameCount { get; private set; }

        public void Process(AudioBlock block)
        {
            Blocks++;
            LastFrameCount = block.FrameCount;
            block.Output.Samples.Clear();
        }
    }
}
