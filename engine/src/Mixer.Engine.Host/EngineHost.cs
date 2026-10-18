using System.Runtime;
using Mixer.Engine.Host.Cli;
using Mixer.Engine.Host.Composition;

namespace Mixer.Engine.Host;

internal static class EngineHost
{
    public static int Run(string[] args)
    {
        GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        var options = HostOptions.Parse(args);
        using var engine = new EngineComposition();
        return options.Command switch
        {
            HostCommand.ListDevices => new ListDevicesCommand(engine).Execute(),
            HostCommand.Run => new RunMixerCommand(engine, options).Execute(),
            HostCommand.Ipc => new IpcCommand(engine).Execute(),
            _ => Usage.Print(),
        };
    }
}
