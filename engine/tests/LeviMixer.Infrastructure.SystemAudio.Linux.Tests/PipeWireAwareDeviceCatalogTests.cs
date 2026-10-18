using LeviMixer.Application.Devices;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.Tests;

public sealed class PipeWireAwareDeviceCatalogTests
{
    private static readonly AudioEndpoint Interface = Endpoint("PCM2902 Audio Codec", false);
    private static readonly AudioEndpoint VirtualOutput = Endpoint("LeviMixer PC", true);
    private static readonly AudioEndpoint VirtualMonitor = Endpoint("Monitor of LeviMixer PC", false);

    [Fact]
    public void Hides_the_virtual_sink_and_its_monitor()
    {
        var catalog = new PipeWireAwareDeviceCatalog(new StaticCatalog([Interface, VirtualMonitor], [Interface, VirtualOutput]), new Memory(null));

        var devices = catalog.ListDevices();

        Assert.Equal(["PCM2902 Audio Codec"], devices.Inputs.Select(input => input.Name));
        Assert.Equal(["PCM2902 Audio Codec"], devices.Outputs.Select(output => output.Name));
    }

    [Fact]
    public void Original_default_output_stays_default_after_the_switch()
    {
        var catalog = new PipeWireAwareDeviceCatalog(new StaticCatalog([], [Interface, VirtualOutput]), new Memory("PCM2902 Audio Codec"));

        Assert.Equal(Interface.Name, catalog.ListDevices().DefaultOutput?.Name);
    }

    private static AudioEndpoint Endpoint(string name, bool isDefault) => new(new AudioDeviceId(name), name, 2, isDefault);

    private sealed class StaticCatalog(AudioEndpoint[] inputs, AudioEndpoint[] outputs) : IAudioDeviceCatalog
    {
        public AudioDeviceList ListDevices() => new([.. inputs], [.. outputs]);
    }

    private sealed class Memory(string? original) : IDefaultOutputMemory
    {
        public string? OriginalDefaultOutputName => original;
    }
}
