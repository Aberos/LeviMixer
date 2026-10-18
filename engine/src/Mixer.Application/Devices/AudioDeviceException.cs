namespace Mixer.Application.Devices;

public sealed class AudioDeviceException : Exception
{
    public AudioDeviceException()
    {
    }

    public AudioDeviceException(string message)
        : base(message)
    {
    }

    public AudioDeviceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
