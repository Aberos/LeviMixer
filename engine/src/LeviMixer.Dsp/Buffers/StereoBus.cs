namespace LeviMixer.Dsp.Buffers;

public readonly ref struct StereoBus
{
    public StereoBus(Span<float> left, Span<float> right)
    {
        if (left.Length != right.Length)
        {
            throw new ArgumentException("Left and right buses must have the same length.", nameof(right));
        }

        Left = left;
        Right = right;
    }

    public Span<float> Left { get; }

    public Span<float> Right { get; }

    public int FrameCount => Left.Length;

    public void Clear()
    {
        Left.Clear();
        Right.Clear();
    }

    public void Add(int frame, float left, float right)
    {
        Left[frame] += left;
        Right[frame] += right;
    }
}
