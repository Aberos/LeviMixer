using Mixer.Application.Devices;
using Mixer.Application.UseCases;
using Mixer.Engine.Host.Composition;
using Mixer.Engine.Host.Console;

namespace Mixer.Engine.Host.Cli;

internal sealed class RunMixerCommand(EngineComposition engine, HostOptions options)
{
    private static readonly TimeSpan MeterInterval = TimeSpan.FromMilliseconds(250);

    public int Execute()
    {
        var format = new StreamFormat(StreamFormat.Default.SampleRate, options.BufferFrames);
        var started = engine.UseCases.StartMixing.Execute(new StartMixingRequest(options.Input, options.Output, format));
        if (started.Error is { } error)
        {
            System.Console.Error.WriteLine($"{error.Code}: {error.Message}");
            return 2;
        }

        engine.UseCases.SetMasterGain.Execute(options.MasterGain);
        PrintMetersUntilStopped();
        engine.UseCases.StopMixing.Execute();
        return 0;
    }

    private void PrintMetersUntilStopped()
    {
        using var stop = new CancellationTokenSource();
        System.Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            stop.Cancel();
        };

        if (options.Duration is { } duration)
        {
            stop.CancelAfter(duration);
        }

        while (!stop.Token.WaitHandle.WaitOne(MeterInterval))
        {
            MeterLinePrinter.Print(engine.UseCases.ReadMeters.Execute(), System.Console.Out);
        }
    }
}
