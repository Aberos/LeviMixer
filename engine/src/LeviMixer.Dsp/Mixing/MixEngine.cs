using System.Collections.Immutable;
using LeviMixer.Dsp.Buffers;
using LeviMixer.Dsp.Metering;
using LeviMixer.Dsp.Routing;

namespace LeviMixer.Dsp.Mixing;

public sealed class MixEngine : IAudioBlockProcessor
{
    private readonly float[] _busLeft;
    private readonly float[] _busRight;
    private readonly StereoGain[] _appliedStripGains;
    private readonly LevelMeter[] _stripMeters;
    private readonly LevelMeter _masterLeftMeter;
    private readonly LevelMeter _masterRightMeter;
    private float _appliedMasterGain;
    private MixSnapshot _snapshot = MixSnapshot.Silent;

    public MixEngine(MixEngineCapacity capacity)
    {
        ArgumentNullException.ThrowIfNull(capacity);
        Capacity = capacity;
        _busLeft = new float[capacity.MaxFrames];
        _busRight = new float[capacity.MaxFrames];
        _appliedStripGains = new StereoGain[capacity.MaxStrips];
        _stripMeters = [.. Enumerable.Range(0, capacity.MaxStrips).Select(_ => new LevelMeter(capacity.SampleRate))];
        _masterLeftMeter = new LevelMeter(capacity.SampleRate);
        _masterRightMeter = new LevelMeter(capacity.SampleRate);
    }

    public MixEngineCapacity Capacity { get; }

    public void Apply(MixSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        if (snapshot.Strips.Length > Capacity.MaxStrips)
        {
            throw new ArgumentException($"Snapshot has {snapshot.Strips.Length} strips but capacity is {Capacity.MaxStrips}.", nameof(snapshot));
        }

        Volatile.Write(ref _snapshot, snapshot);
    }

    public void Process(AudioBlock block)
    {
        var snapshot = Volatile.Read(ref _snapshot);
        var frameCount = Math.Min(block.FrameCount, Capacity.MaxFrames);
        var bus = new StereoBus(_busLeft.AsSpan(0, frameCount), _busRight.AsSpan(0, frameCount));
        bus.Clear();
        MixStrips(block, snapshot, bus);
        WriteMaster(block.Output, snapshot.Master.Value, bus);
    }

    public MeterReport TakeMeterReport()
    {
        var strips = Volatile.Read(ref _snapshot).Strips;
        var channels = strips
            .Select((strip, index) => new ChannelMeterReading(strip.ChannelId, _stripMeters[index].TakeReading()))
            .ToImmutableArray();
        return new MeterReport(channels, _masterLeftMeter.TakeReading(), _masterRightMeter.TakeReading());
    }

    private void MixStrips(AudioBlock block, MixSnapshot snapshot, StereoBus bus)
    {
        for (var index = 0; index < snapshot.Strips.Length; index++)
        {
            var strip = snapshot.Strips[index];
            var writer = new StripWriter(bus, StereoRamp.Between(_appliedStripGains[index], strip.Gain, bus.FrameCount));
            MixSource(block, strip.Route, ref writer);
            writer.Level.RecordInto(_stripMeters[index], bus.FrameCount);
            _appliedStripGains[index] = strip.Gain;
        }
    }

    private static void MixSource(AudioBlock block, StripRoute route, ref StripWriter writer)
    {
        if (route.Kind == StripSourceKind.SystemStereo)
        {
            MixStereo(block.SystemAudio, ref writer);
            return;
        }

        MixMono(new MonoSource(block.InterfaceInput, route.InterfaceChannel), ref writer);
    }

    private static void MixMono(MonoSource source, ref StripWriter writer)
    {
        if (!source.Covers(writer.FrameCount))
        {
            return;
        }

        for (var frame = 0; frame < writer.FrameCount; frame++)
        {
            var sample = source.Sample(frame);
            writer.Write(frame, sample, sample);
        }
    }

    private static void MixStereo(InterleavedInput source, ref StripWriter writer)
    {
        if (!source.Covers(writer.FrameCount, 1))
        {
            return;
        }

        for (var frame = 0; frame < writer.FrameCount; frame++)
        {
            writer.Write(frame, source.Sample(frame, 0), source.Sample(frame, 1));
        }
    }

    private void WriteMaster(InterleavedOutput output, float targetGain, StereoBus bus)
    {
        output.Samples.Clear();
        var ramp = GainRamp.Between(_appliedMasterGain, targetGain, bus.FrameCount);
        var leftLevel = default(BlockLevel);
        var rightLevel = default(BlockLevel);

        for (var frame = 0; frame < bus.FrameCount; frame++)
        {
            var gain = ramp.At(frame);
            var left = bus.Left[frame] * gain;
            var right = bus.Right[frame] * gain;
            WriteFrame(output, frame, left, right);
            leftLevel.Add(left);
            rightLevel.Add(right);
        }

        leftLevel.RecordInto(_masterLeftMeter, bus.FrameCount);
        rightLevel.RecordInto(_masterRightMeter, bus.FrameCount);
        _appliedMasterGain = targetGain;
    }

    private static void WriteFrame(InterleavedOutput output, int frame, float left, float right)
    {
        var offset = frame * output.ChannelCount;
        output.Samples[offset] = left;
        if (output.ChannelCount > 1)
        {
            output.Samples[offset + 1] = right;
        }
    }
}
