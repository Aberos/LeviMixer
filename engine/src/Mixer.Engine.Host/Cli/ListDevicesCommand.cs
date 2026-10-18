using Mixer.Engine.Host.Composition;
using Mixer.Engine.Host.Console;

namespace Mixer.Engine.Host.Cli;

internal sealed class ListDevicesCommand(EngineComposition engine)
{
    public int Execute()
    {
        DeviceListPrinter.Print(engine.UseCases.ListAudioDevices.Execute(), System.Console.Out);
        return 0;
    }
}
