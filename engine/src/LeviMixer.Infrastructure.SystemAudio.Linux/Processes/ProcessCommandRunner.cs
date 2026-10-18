using System.ComponentModel;
using System.Diagnostics;
using LeviMixer.Application.Devices;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.Processes;

public sealed class ProcessCommandRunner : ICommandRunner
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    public CommandOutput Run(string fileName, params string[] arguments)
    {
        using var process = StartProcess(fileName, arguments);
        var output = process.StandardOutput.ReadToEnd();
        if (!process.WaitForExit(Timeout))
        {
            process.Kill();
            throw new AudioDeviceException($"'{fileName}' did not finish in time.");
        }

        return new CommandOutput(process.ExitCode, output);
    }

    private static Process StartProcess(string fileName, string[] arguments)
    {
        var startInfo = new ProcessStartInfo(fileName, arguments)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        try
        {
            return Process.Start(startInfo) ?? throw new AudioDeviceException($"'{fileName}' could not be started.");
        }
        catch (Win32Exception exception)
        {
            throw new AudioDeviceException($"'{fileName}' is not available. Is PipeWire installed?", exception);
        }
    }
}
