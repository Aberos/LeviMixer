using System.ComponentModel;
using System.Diagnostics;
using Mixer.Application.Devices;

namespace Mixer.Infrastructure.SystemAudio.Linux.PipeWire;

internal sealed class VirtualSinkProcess : IDisposable
{
    private const string KeepAliveScript =
        "pw-cli -m create-node adapter \"$1\" >/dev/null 2>&1 & child=$!; cat >/dev/null; kill $child 2>/dev/null; wait $child";

    private static readonly TimeSpan ExitTimeout = TimeSpan.FromSeconds(2);

    private readonly Process _process;

    private VirtualSinkProcess(Process process)
    {
        _process = process;
    }

    public static VirtualSinkProcess Start()
    {
        var startInfo = new ProcessStartInfo("sh", ["-c", KeepAliveScript, "sh", VirtualSink.NodeProperties])
        {
            RedirectStandardInput = true,
            UseShellExecute = false,
        };

        try
        {
            return new VirtualSinkProcess(Process.Start(startInfo) ?? throw new AudioDeviceException("Could not start the PipeWire virtual sink."));
        }
        catch (Win32Exception exception)
        {
            throw new AudioDeviceException("Could not start the PipeWire virtual sink.", exception);
        }
    }

    public void Dispose()
    {
        _process.StandardInput.Close();
        if (!_process.WaitForExit(ExitTimeout))
        {
            _process.Kill(entireProcessTree: true);
        }

        _process.Dispose();
    }
}
