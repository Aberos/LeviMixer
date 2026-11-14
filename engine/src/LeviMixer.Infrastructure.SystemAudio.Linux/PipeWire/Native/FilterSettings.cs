using System.Collections.Immutable;
using System.Globalization;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire.Native;

internal sealed record FilterSettings(string NodeName, int SampleRate, int BufferFrames, ImmutableArray<string> InputPorts, ImmutableArray<string> OutputPorts)
{
    public string NodeProperties => string.Create(
        CultureInfo.InvariantCulture,
        $"media.type=Audio media.category=Duplex media.role=DSP node.name={NodeName} node.description=LeviMixer node.latency={BufferFrames}/{SampleRate} node.rate=1/{SampleRate}");

    public static string PortProperties(string portName) => $"format.dsp=\"32 bit float mono audio\" port.name={portName}";
}
