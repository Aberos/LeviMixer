namespace LeviMixer.Domain.Channels;

public sealed record InterfaceInput : ChannelSource
{
    public InterfaceInput(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        Index = index;
    }

    public int Index { get; }
}
