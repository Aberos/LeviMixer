using System.Globalization;
using System.Text.Json;
using LeviMixer.Application.Devices;
using LeviMixer.Infrastructure.SystemAudio.Linux.Processes;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire.Graph;

internal sealed class PipeWireGraph(ICommandRunner commands)
{
    public GraphSnapshot Read()
    {
        var output = commands.Run("pw-dump");
        if (!output.Succeeded)
        {
            throw new AudioDeviceException("Could not read the PipeWire graph.");
        }

        try
        {
            return GraphSnapshot.Parse(output.StandardOutput);
        }
        catch (JsonException exception)
        {
            throw new AudioDeviceException("The PipeWire graph could not be parsed.", exception);
        }
    }

    public void Connect(PortLink link)
    {
        var output = commands.Run("pw-link", Id(link.From), Id(link.To));
        if (!output.Succeeded)
        {
            throw new AudioDeviceException($"Could not link PipeWire port '{link.From.Name}' to '{link.To.Name}'.");
        }
    }

    private static string Id(GraphPort port) => port.Id.ToString(CultureInfo.InvariantCulture);
}
