using System.Text.RegularExpressions;
using LeviMixer.Infrastructure.SystemAudio.Linux.Processes;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire;

internal sealed partial class DefaultSinkSetting(ICommandRunner commands)
{
    private const string Tool = "pw-metadata";
    private const string Key = "default.configured.audio.sink";
    private const string JsonType = "Spa:String:JSON";

    public string? Read()
    {
        var output = commands.Run(Tool, "0", Key);
        var match = ValuePattern().Match(output.StandardOutput);
        return match.Success && !match.Groups["value"].Value.Contains(VirtualSink.NodeName, StringComparison.Ordinal)
            ? match.Groups["value"].Value
            : null;
    }

    public void PointTo(string nodeName) => commands.Run(Tool, "0", Key, $$"""{"name":"{{nodeName}}"}""", JsonType);

    public void Restore(string? previousValue)
    {
        if (previousValue is null)
        {
            commands.Run(Tool, "-d", "0", Key);
            return;
        }

        commands.Run(Tool, "0", Key, previousValue, JsonType);
    }

    [GeneratedRegex(@"key:'default\.configured\.audio\.sink' value:'(?<value>.*?)' type:")]
    private static partial Regex ValuePattern();
}
