namespace LeviMixer.Dsp.Buffers;

public readonly ref struct MonoSource
{
    public MonoSource(InterleavedInput input, int channel)
    {
        Input = input;
        Channel = channel;
    }

    public InterleavedInput Input { get; }

    public int Channel { get; }

    public bool Covers(int frameCount) => Input.Covers(frameCount, Channel);

    public float Sample(int frame) => Input.Sample(frame, Channel);
}
