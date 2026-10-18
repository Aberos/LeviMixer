namespace Mixer.Domain.Channels;

public readonly record struct ChannelId
{
    public ChannelId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
