using LeviMixer.Infrastructure.SystemAudio.Linux.Processes;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.Tests;

internal sealed class RecordingCommandRunner(string output = "") : ICommandRunner
{
    public List<string> Commands { get; } = [];

    public CommandOutput Run(string fileName, params string[] arguments)
    {
        Commands.Add(string.Join(' ', [fileName, .. arguments]));
        return new CommandOutput(0, output);
    }
}
