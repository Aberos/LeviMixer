namespace Mixer.Infrastructure.SystemAudio.Linux.Processes;

public interface ICommandRunner
{
    CommandOutput Run(string fileName, params string[] arguments);
}
