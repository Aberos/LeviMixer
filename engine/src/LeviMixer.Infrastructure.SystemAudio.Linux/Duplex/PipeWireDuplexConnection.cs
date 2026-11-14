using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;
using LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire.Graph;
using LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire.Native;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.Duplex;

internal sealed unsafe class PipeWireDuplexConnection : IAudioConnection
{
    public const string NodeName = "levimixer_interface";

    private const int OutputChannelCount = 2;
    private const int MaxFrames = 8_192;
    private const int ClockDurationOffset = 96;
    private static readonly TimeSpan PortWaitTimeout = TimeSpan.FromSeconds(3);
    private static readonly TimeSpan PortPollInterval = TimeSpan.FromMilliseconds(50);
    private static readonly ImmutableArray<string> OutputPortNames = ["out_FL", "out_FR"];

    private readonly IAudioBlockProcessor _processor;
    private readonly int _inputChannelCount;
    private readonly float[] _input;
    private readonly float[] _output = new float[MaxFrames * OutputChannelCount];
    private GCHandle _self;
    private NativeFilter? _filter;

    private PipeWireDuplexConnection(AudioConnectionRequest request, IAudioBlockProcessor processor)
    {
        Request = request;
        _processor = processor;
        _inputChannelCount = request.Input.ChannelCount;
        _input = new float[MaxFrames * _inputChannelCount];
        _self = GCHandle.Alloc(this);
    }

    public AudioConnectionRequest Request { get; }

    public static PipeWireDuplexConnection Open(AudioConnectionRequest request, IAudioBlockProcessor processor, PipeWireGraph graph)
    {
        var devicePorts = InterfacePorts.Find(graph.Read(), request);
        var connection = new PipeWireDuplexConnection(request, processor);
        try
        {
            connection.Start();
            connection.LinkTo(devicePorts, graph);
            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        _filter?.Dispose();
        _filter = null;
        if (_self.IsAllocated)
        {
            _self.Free();
        }
    }

    private void Start()
    {
        var settings = new FilterSettings(NodeName, Request.Format.SampleRate, Request.Format.BufferFrames, InputPortNames(_inputChannelCount), OutputPortNames);
        _filter = NativeFilter.Open(settings, &OnProcess, (void*)GCHandle.ToIntPtr(_self));
    }

    private void LinkTo(InterfacePorts devicePorts, PipeWireGraph graph)
    {
        var (inputs, outputs) = WaitForOwnPorts(graph);
        foreach (var link in devicePorts.LinksTo(inputs, outputs))
        {
            graph.Connect(link);
        }
    }

    private (ImmutableArray<GraphPort> Inputs, ImmutableArray<GraphPort> Outputs) WaitForOwnPorts(PipeWireGraph graph)
    {
        var deadline = DateTime.UtcNow + PortWaitTimeout;
        while (DateTime.UtcNow < deadline)
        {
            var nodeId = _filter!.NodeId;
            if (nodeId != PipeWireLibrary.InvalidId)
            {
                var snapshot = graph.Read();
                var inputs = snapshot.AudioPortsOf(nodeId, PortDirection.Input);
                var outputs = snapshot.AudioPortsOf(nodeId, PortDirection.Output);
                if (inputs.Length == _inputChannelCount && outputs.Length == OutputChannelCount)
                {
                    return (inputs, outputs);
                }
            }

            Thread.Sleep(PortPollInterval);
        }

        throw new AudioDeviceException("The LeviMixer PipeWire ports did not appear in time.");
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnProcess(void* data, void* position)
    {
        if (position == null)
        {
            return;
        }

        var connection = (PipeWireDuplexConnection)GCHandle.FromIntPtr((nint)data).Target!;
        connection.Render((int)*(ulong*)((byte*)position + ClockDurationOffset));
    }

    private void Render(int frames)
    {
        var filter = _filter;
        if (filter is null)
        {
            return;
        }

        if (frames > MaxFrames)
        {
            SilenceOutputs(filter, frames);
            return;
        }

        GatherInputs(filter, frames);
        var output = _output.AsSpan(0, frames * OutputChannelCount);
        _processor.Process(new AudioBlock(new InterleavedInput(_input.AsSpan(0, frames * _inputChannelCount), _inputChannelCount), InterleavedInput.None, new InterleavedOutput(output, OutputChannelCount)));
        ScatterOutputs(filter, output, frames);
    }

    private void GatherInputs(NativeFilter filter, int frames)
    {
        var interleaved = _input.AsSpan(0, frames * _inputChannelCount);
        for (var channel = 0; channel < _inputChannelCount; channel++)
        {
            var source = DspBuffer(filter.InputPorts[channel], frames);
            for (var frame = 0; frame < frames; frame++)
            {
                interleaved[(frame * _inputChannelCount) + channel] = source.IsEmpty ? 0f : source[frame];
            }
        }
    }

    private static void ScatterOutputs(NativeFilter filter, ReadOnlySpan<float> interleaved, int frames)
    {
        for (var channel = 0; channel < OutputChannelCount; channel++)
        {
            var destination = DspBuffer(filter.OutputPorts[channel], frames);
            for (var frame = 0; frame < destination.Length; frame++)
            {
                destination[frame] = interleaved[(frame * OutputChannelCount) + channel];
            }
        }
    }

    private static void SilenceOutputs(NativeFilter filter, int frames)
    {
        foreach (var port in filter.OutputPorts)
        {
            DspBuffer(port, frames).Clear();
        }
    }

    private static Span<float> DspBuffer(nint port, int frames)
    {
        var buffer = PipeWireLibrary.FilterGetDspBuffer((void*)port, (uint)frames);
        return buffer == null ? [] : new Span<float>(buffer, frames);
    }

    private static ImmutableArray<string> InputPortNames(int count) =>
        [.. Enumerable.Range(1, count).Select(index => "in_" + index.ToString(CultureInfo.InvariantCulture))];
}
