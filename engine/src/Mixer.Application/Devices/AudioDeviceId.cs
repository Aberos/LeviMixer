namespace Mixer.Application.Devices;

public readonly record struct AudioDeviceId
{
    public AudioDeviceId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
