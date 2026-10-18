using LeviMixer.Dsp.Buffers;
using LeviMixer.Dsp.Resampling;

namespace LeviMixer.Dsp.Tests.Resampling;

public sealed class DriftCompensatingReaderTests
{
    private const int Block = 64;
    private const int Target = 256;

    [Fact]
    public void Outputs_silence_until_the_target_fill_is_reached()
    {
        var (ring, reader) = Create();
        ring.Write(Stereo(Target - 1, 0.5f));

        var output = ReadBlock(reader);

        Assert.All(output, sample => Assert.Equal(0f, sample));
        Assert.False(reader.IsPrimed);
    }

    [Fact]
    public void Constant_signal_passes_through_unchanged_at_target_fill()
    {
        var (ring, reader) = Create();
        ring.Write(Stereo(Target + Block, 0.5f));
        ReadBlock(reader);

        var output = ReadBlock(reader);

        Assert.All(output, sample => Assert.Equal(0.5f, sample, 0.0001f));
    }

    [Fact]
    public void Ramp_signal_stays_continuous_across_blocks()
    {
        var (ring, reader) = Create();
        ring.Write(Ramp(Target * 3));
        var lefts = Enumerable.Range(0, 6).SelectMany(_ => Left(ReadBlock(reader))).Skip(1).ToArray();

        var steps = lefts.Zip(lefts.Skip(1), (first, second) => second - first).ToArray();

        Assert.All(steps, step => Assert.InRange(step, 0.5f, 1.5f));
    }

    [Fact]
    public void Underrun_outputs_silence_and_waits_to_prime_again()
    {
        var (ring, reader) = Create();
        ring.Write(Stereo(Target, 0.5f));
        for (var block = 0; block < Target / Block; block++)
        {
            ReadBlock(reader);
        }

        var output = ReadBlock(reader);

        Assert.All(output, sample => Assert.Equal(0f, sample));
        Assert.False(reader.IsPrimed);
    }

    [Fact]
    public void Excess_latency_is_dropped_back_to_target()
    {
        var (ring, reader) = Create(ringFrames: 4096);
        ring.Write(Stereo(Target * 8, 0.5f));

        ReadBlock(reader);

        Assert.InRange(ring.AvailableToRead / 2, Target - Block - 2, Target);
    }

    [Theory]
    [InlineData(1.001, 1.0005, 1.0015)]
    [InlineData(0.999, 0.9985, 0.9995)]
    public void Tracks_producer_clock_drift_while_keeping_fill_stable(double producerRate, double minRatio, double maxRatio)
    {
        var (ring, reader) = Create(ringFrames: 4096);
        var producedFrames = 0d;
        var fills = new List<int>();

        for (var block = 0; block < 20_000; block++)
        {
            producedFrames += Block * producerRate;
            var whole = (int)producedFrames;
            producedFrames -= whole;
            ring.Write(Stereo(whole, 0.1f));
            ReadBlock(reader);
            if (block > 5_000)
            {
                fills.Add(ring.AvailableToRead / 2);
            }
        }

        Assert.InRange(fills.Min(), Target / 2, Target * 2);
        Assert.InRange(fills.Max(), Target / 2, Target * 2);
        Assert.True(reader.IsPrimed);
        Assert.InRange(reader.Ratio, minRatio, maxRatio);
    }

    [Fact]
    public void Reading_does_not_allocate()
    {
        var (ring, reader) = Create(ringFrames: 4096);
        var source = Stereo(Block, 0.2f);
        var output = new float[Block * 2];
        ring.Write(Stereo(Target, 0.2f));
        reader.ReadInto(output);

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var block = 0; block < 1_000; block++)
        {
            ring.Write(source);
            reader.ReadInto(output);
        }

        Assert.Equal(before, GC.GetAllocatedBytesForCurrentThread());
    }

    private static (SpscRingBuffer Ring, DriftCompensatingReader Reader) Create(int ringFrames = 1024)
    {
        var ring = new SpscRingBuffer(ringFrames * 2);
        return (ring, new DriftCompensatingReader(ring, new DriftControl(Target, Block)));
    }

    private static float[] ReadBlock(DriftCompensatingReader reader)
    {
        var output = new float[Block * 2];
        reader.ReadInto(output);
        return output;
    }

    private static float[] Stereo(int frames, float value) => Enumerable.Repeat(value, frames * 2).ToArray();

    private static float[] Ramp(int frames) => Enumerable.Range(0, frames).SelectMany(frame => new[] { (float)frame, (float)frame }).ToArray();

    private static IEnumerable<float> Left(float[] stereo) => stereo.Where((_, index) => index % 2 == 0);
}
