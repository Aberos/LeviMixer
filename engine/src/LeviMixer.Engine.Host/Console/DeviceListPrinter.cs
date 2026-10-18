using LeviMixer.Application.Devices;

namespace LeviMixer.Engine.Host.Console;

internal static class DeviceListPrinter
{
    public static void Print(AudioDeviceList devices, TextWriter writer)
    {
        PrintSection("Inputs", devices.Inputs, writer);
        PrintSection("Outputs", devices.Outputs, writer);
    }

    private static void PrintSection(string title, IEnumerable<AudioEndpoint> endpoints, TextWriter writer)
    {
        writer.WriteLine(title);
        foreach (var endpoint in endpoints)
        {
            var marker = endpoint.IsDefault ? "*" : " ";
            writer.WriteLine($" {marker} [{endpoint.ChannelCount} ch] {endpoint.Id}");
        }
    }
}
