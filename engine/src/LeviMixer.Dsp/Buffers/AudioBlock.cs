namespace LeviMixer.Dsp.Buffers;

public readonly ref struct AudioBlock
{
    public AudioBlock(InterleavedInput interfaceInput, InterleavedInput systemAudio, InterleavedOutput output)
    {
        InterfaceInput = interfaceInput;
        SystemAudio = systemAudio;
        Output = output;
    }

    public InterleavedInput InterfaceInput { get; }

    public InterleavedInput SystemAudio { get; }

    public InterleavedOutput Output { get; }

    public int FrameCount => Output.FrameCount;
}
