using Mixer.Dsp.Buffers;

namespace Mixer.Dsp.Tests.Buffers;

public sealed class SpscRingBufferTests
{
    [Fact]
    public void Reads_back_what_was_written_across_the_wrap_point()
    {
        var ring = new SpscRingBuffer(8);
        var scratch = new float[6];
        ring.Write([1, 2, 3, 4, 5, 6]);
        ring.Read(scratch);

        ring.Write([7, 8, 9, 10, 11]);
        var read = new float[5];
        var count = ring.Read(read);

        Assert.Equal(5, count);
        Assert.Equal([7f, 8, 9, 10, 11], read);
    }

    [Fact]
    public void Write_stops_when_full()
    {
        var ring = new SpscRingBuffer(4);

        var written = ring.Write([1, 2, 3, 4, 5, 6]);

        Assert.Equal(4, written);
        Assert.Equal(4, ring.AvailableToRead);
    }

    [Fact]
    public void Skip_discards_oldest_samples()
    {
        var ring = new SpscRingBuffer(8);
        ring.Write([1, 2, 3, 4]);

        ring.Skip(3);
        var read = new float[1];
        ring.Read(read);

        Assert.Equal(4f, read[0]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Capacity_must_be_a_power_of_two(int capacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SpscRingBuffer(capacity));
    }
}
