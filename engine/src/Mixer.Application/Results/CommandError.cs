namespace Mixer.Application.Results;

public sealed record CommandError(string Code, string Message)
{
    public static CommandError UnknownChannel(string channelId) => new("unknownChannel", $"Channel '{channelId}' does not exist.");

    public static CommandError DeviceNotFound(string deviceId) => new("deviceNotFound", $"Audio device '{deviceId}' was not found.");

    public static CommandError DeviceUnavailable(string reason) => new("deviceUnavailable", reason);

    public static CommandError NotRunning() => new("notRunning", "The mixer is not running.");
}
