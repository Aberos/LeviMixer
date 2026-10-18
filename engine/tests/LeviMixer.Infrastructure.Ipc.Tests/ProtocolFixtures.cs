using System.Text.Json.Nodes;

namespace LeviMixer.Infrastructure.Ipc.Tests;

internal static class ProtocolFixtures
{
    private static readonly Lazy<string> Root = new(FindRoot);

    public static string Inbound(string name) => ReadLine(Path.Combine(Root.Value, "inbound", $"{name}.json"));

    public static JsonNode Outbound(string name) => JsonNode.Parse(File.ReadAllText(Path.Combine(Root.Value, "outbound", $"{name}.json")))!;

    public static IEnumerable<string> InboundNames() =>
        Directory.EnumerateFiles(Path.Combine(Root.Value, "inbound"), "*.json").Select(Path.GetFileNameWithoutExtension)!;

    private static string ReadLine(string path) => JsonNode.Parse(File.ReadAllText(path))!.ToJsonString();

    private static string FindRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            var candidate = Path.Combine(directory.FullName, "packages", "protocol", "fixtures");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new DirectoryNotFoundException("packages/protocol/fixtures was not found above the test directory.");
    }
}
