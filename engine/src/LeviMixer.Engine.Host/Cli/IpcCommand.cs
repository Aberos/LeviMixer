using System.Reflection;
using System.Text;
using LeviMixer.Engine.Host.Composition;
using LeviMixer.Infrastructure.Ipc;

namespace LeviMixer.Engine.Host.Cli;

internal sealed class IpcCommand(EngineComposition engine)
{
    public int Execute()
    {
        using var input = new StreamReader(System.Console.OpenStandardInput(), Encoding.UTF8);
        using var output = new StreamWriter(System.Console.OpenStandardOutput(), new UTF8Encoding(false));
        new IpcServer(engine.UseCases, output).Run(input, EngineVersion);
        return 0;
    }

    private static string EngineVersion =>
        typeof(IpcCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0] ?? "0.0.0";
}
