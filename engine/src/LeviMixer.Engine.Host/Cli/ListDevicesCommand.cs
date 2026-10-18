using LeviMixer.Engine.Host.Composition;
using LeviMixer.Engine.Host.Console;

namespace LeviMixer.Engine.Host.Cli;

internal sealed class ListDevicesCommand(EngineComposition engine)
{
    public int Execute()
    {
        DeviceListPrinter.Print(engine.UseCases.ListAudioDevices.Execute(), System.Console.Out);
        return 0;
    }
}
