using System.Numerics;

namespace LeviMixer.Dsp.Buffers;

public sealed class SpscRingBuffer : ISampleWriter
{
    private readonly float[] _samples;
    private readonly int _mask;
    private long _writePosition;
    private long _readPosition;

    public SpscRingBuffer(int capacity)
    {
        if (capacity <= 0 || !BitOperations.IsPow2(capacity))
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Capacity must be a positive power of two.");
        }

        _samples = new float[capacity];
        _mask = capacity - 1;
    }

    public int Capacity => _samples.Length;

    public int AvailableToRead => (int)(Volatile.Read(ref _writePosition) - Volatile.Read(ref _readPosition));

    public int Write(ReadOnlySpan<float> samples)
    {
        var writePosition = Volatile.Read(ref _writePosition);
        var free = Capacity - (int)(writePosition - Volatile.Read(ref _readPosition));
        var count = Math.Min(free, samples.Length);
        CopyIn(samples[..count], (int)(writePosition & _mask));
        Volatile.Write(ref _writePosition, writePosition + count);
        return count;
    }

    public int Read(Span<float> destination)
    {
        var readPosition = Volatile.Read(ref _readPosition);
        var count = Math.Min(AvailableToRead, destination.Length);
        CopyOut(destination[..count], (int)(readPosition & _mask));
        Volatile.Write(ref _readPosition, readPosition + count);
        return count;
    }

    public void Skip(int count)
    {
        var skipped = Math.Min(count, AvailableToRead);
        Volatile.Write(ref _readPosition, Volatile.Read(ref _readPosition) + skipped);
    }

    private void CopyIn(ReadOnlySpan<float> samples, int start)
    {
        var firstPart = Math.Min(samples.Length, Capacity - start);
        samples[..firstPart].CopyTo(_samples.AsSpan(start));
        samples[firstPart..].CopyTo(_samples);
    }

    private void CopyOut(Span<float> destination, int start)
    {
        var firstPart = Math.Min(destination.Length, Capacity - start);
        _samples.AsSpan(start, firstPart).CopyTo(destination);
        _samples.AsSpan(0, destination.Length - firstPart).CopyTo(destination[firstPart..]);
    }
}
