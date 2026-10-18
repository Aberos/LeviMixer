namespace LeviMixer.Application.Results;

public sealed record CommandResult
{
    private CommandResult(CommandError? error)
    {
        Error = error;
    }

    public static CommandResult Success { get; } = new((CommandError?)null);

    public CommandError? Error { get; }

    public bool IsSuccess => Error is null;

    public static CommandResult Failure(CommandError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new CommandResult(error);
    }
}
