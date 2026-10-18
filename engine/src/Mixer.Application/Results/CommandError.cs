namespace Mixer.Application.Results;

public sealed record CommandError(string Code, string Message)
{
    public static CommandError UnknownChannel(string channelId) => new("unknownChannel", $"Channel '{channelId}' does not exist.");

    public static CommandError InvalidChannelName(int maxLength) => new("invalidChannelName", $"Channel names must have between 1 and {maxLength} characters.");

    public static CommandError DeviceNotFound(string deviceId) => new("deviceNotFound", $"Audio device '{deviceId}' was not found.");

    public static CommandError DeviceUnavailable(string reason) => new("deviceUnavailable", reason);
}
