namespace LeviMixer.Domain.Channels;

public static class ChannelName
{
    public const int MaxLength = 24;

    public static bool IsValid(string? name) => !string.IsNullOrWhiteSpace(name) && name.Trim().Length <= MaxLength;

    public static string Normalize(string name)
    {
        if (!IsValid(name))
        {
            throw new ArgumentException($"Channel name must have between 1 and {MaxLength} visible characters.", nameof(name));
        }

        return name.Trim();
    }
}
