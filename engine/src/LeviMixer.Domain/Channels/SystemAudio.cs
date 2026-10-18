namespace LeviMixer.Domain.Channels;

public sealed record SystemAudio : ChannelSource
{
    private SystemAudio()
    {
    }

    public static SystemAudio Instance { get; } = new();
}
