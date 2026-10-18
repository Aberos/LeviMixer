using System.Globalization;
using LeviMixer.Application.Devices;
using LeviMixer.Domain.Levels;

namespace LeviMixer.Engine.Host.Cli;

internal sealed record HostOptions(HostCommand Command)
{
    public AudioDeviceId? Input { get; init; }

    public AudioDeviceId? Output { get; init; }

    public int BufferFrames { get; init; } = StreamFormat.Default.BufferFrames;

    public Decibels MasterGain { get; init; } = Decibels.Unity;

    public TimeSpan? Duration { get; init; }

    public static HostOptions Parse(IReadOnlyList<string> args)
    {
        var options = new HostOptions(CommandOf(args.Count > 0 ? args[0] : string.Empty));
        for (var index = 1; index + 1 < args.Count; index += 2)
        {
            options = options.With(args[index], args[index + 1]);
        }

        return options;
    }

    private static HostCommand CommandOf(string name) => name switch
    {
        "devices" => HostCommand.ListDevices,
        "run" => HostCommand.Run,
        "ipc" => HostCommand.Ipc,
        _ => HostCommand.Help,
    };

    private HostOptions With(string option, string value) => option switch
    {
        "--input" => this with { Input = new AudioDeviceId(value) },
        "--output" => this with { Output = new AudioDeviceId(value) },
        "--buffer" => this with { BufferFrames = int.Parse(value, CultureInfo.InvariantCulture) },
        "--master" => this with { MasterGain = ParseDecibels(value) },
        "--seconds" => this with { Duration = TimeSpan.FromSeconds(double.Parse(value, CultureInfo.InvariantCulture)) },
        _ => throw new ArgumentException($"Unknown option '{option}'.", nameof(option)),
    };

    private static Decibels ParseDecibels(string value) =>
        value == "-inf" ? Decibels.MinusInfinity : new Decibels(float.Parse(value, CultureInfo.InvariantCulture));
}
