using System.Text.Json.Nodes;
using Mixer.Application.Session;
using Mixer.Application.UseCases;
using Mixer.Testing.Fakes;

namespace Mixer.Infrastructure.Ipc.Tests;

internal sealed class IpcConversation
{
    private readonly List<string> _lines = [];

    public FakeAudioConnectionFactory Connections { get; } = new();

    public FakeSystemAudioCapture SystemAudio { get; } = new();

    public IpcConversation Send(string line)
    {
        _lines.Add(line);
        return this;
    }

    public IReadOnlyList<JsonNode> Run()
    {
        using var session = new MixerSession(Connections, SystemAudio);
        using var output = new StringWriter();
        var server = new IpcServer(new MixerUseCases(new FakeAudioDeviceCatalog(), session), output);

        server.Run(new StringReader(string.Join('\n', _lines)), "0.1.0");

        return [.. output.ToString()
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => JsonNode.Parse(line)!)
            .Where(message => (string?)message["type"] != "meters")];
    }
}
