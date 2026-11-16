namespace LeviMixer.Application.Session;

public sealed record SystemAudioFailure(string Code, string Message)
{
    public static SystemAudioFailure DriverMissing(string message) => new("driverMissing", message);

    public static SystemAudioFailure Unavailable(string message) => new("unavailable", message);
}
