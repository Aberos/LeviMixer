namespace LeviMixer.Infrastructure.SystemAudio.Linux.Processes;

public sealed record CommandOutput(int ExitCode, string StandardOutput)
{
    public bool Succeeded => ExitCode == 0;
}
