namespace LeviMixer.Application.Devices;

public sealed class MissingAudioDriverException : Exception
{
    public MissingAudioDriverException()
    {
    }

    public MissingAudioDriverException(string message)
        : base(message)
    {
    }

    public MissingAudioDriverException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
