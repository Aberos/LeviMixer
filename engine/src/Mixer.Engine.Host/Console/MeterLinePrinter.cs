using System.Globalization;
using Mixer.Domain.Levels;
using Mixer.Dsp.Metering;

namespace Mixer.Engine.Host.Console;

internal static class MeterLinePrinter
{
    public static void Print(MeterReport? report, TextWriter writer)
    {
        if (report is null)
        {
            return;
        }

        var channels = report.Channels.Select(channel => $"{channel.ChannelId}={Format(channel.Reading)}");
        writer.WriteLine($"{string.Join("  ", channels)}  | master L={Format(report.MasterLeft)} R={Format(report.MasterRight)}");
    }

    private static string Format(MeterReading reading)
    {
        var peak = Decibels.FromLinear(reading.Peak);
        return peak.IsSilent ? "  -inf" : peak.Value.ToString("+0.0;-0.0", CultureInfo.InvariantCulture).PadLeft(6);
    }
}
