using System.Text.Json.Nodes;

namespace Mixer.Infrastructure.Ipc.Tests;

public sealed class IpcServerTests
{
    [Fact]
    public void Announces_readiness_first()
    {
        var messages = new IpcConversation().Run();

        Assert.True(JsonNode.DeepEquals(ProtocolFixtures.Outbound("ready"), messages[0]));
    }

    [Fact]
    public void Lists_devices()
    {
        var messages = new IpcConversation().Send(ProtocolFixtures.Inbound("listDevices")).Run();

        Assert.True(JsonNode.DeepEquals(ProtocolFixtures.Outbound("devices"), messages[1]));
    }

    [Fact]
    public void Starting_acknowledges_and_publishes_state()
    {
        var messages = new IpcConversation().Send(ProtocolFixtures.Inbound("startMixing")).Run();

        Assert.Equal("ack", (string?)messages[1]["type"]);
        Assert.Equal("r2", (string?)messages[1]["requestId"]);
        Assert.True(JsonNode.DeepEquals(ProtocolFixtures.Outbound("mixerState"), messages[2]));
    }

    [Fact]
    public void Unknown_channel_is_answered_with_error()
    {
        var messages = new IpcConversation()
            .Send(ProtocolFixtures.Inbound("startMixing"))
            .Send("""{ "type": "setChannelGain", "requestId": "r4", "payload": { "channelId": "in-99", "gainDecibels": 0 } }""")
            .Run();

        Assert.True(JsonNode.DeepEquals(ProtocolFixtures.Outbound("error"), messages[3]));
    }

    [Fact]
    public void Applying_a_session_restores_channel_settings()
    {
        var messages = new IpcConversation().Send(ProtocolFixtures.Inbound("applySession")).Run();

        var state = messages[2]["payload"]!;
        Assert.Equal("Vocal", (string?)state["channels"]![0]!["name"]);
        Assert.Null(state["channels"]![2]!["gainDecibels"]);
        Assert.Equal(-6f, (float?)state["masterGainDecibels"]);
    }

    [Fact]
    public void Invalid_json_is_reported_and_the_server_keeps_running()
    {
        var messages = new IpcConversation().Send("{ not json").Send(ProtocolFixtures.Inbound("getState")).Run();

        Assert.Equal("invalidMessage", (string?)messages[1]["payload"]!["code"]);
        Assert.Equal("mixerState", (string?)messages[2]["type"]);
    }

    [Fact]
    public void Unknown_command_is_reported()
    {
        var messages = new IpcConversation().Send("""{ "type": "explode", "requestId": "x" }""").Run();

        Assert.Equal("unknownCommand", (string?)messages[1]["payload"]!["code"]);
    }

    [Fact]
    public void Missing_payload_is_reported_as_invalid()
    {
        var messages = new IpcConversation().Send("""{ "type": "setMasterGain", "requestId": "x" }""").Run();

        Assert.Equal("invalidMessage", (string?)messages[1]["payload"]!["code"]);
    }

    [Fact]
    public void Shutdown_acknowledges_stops_mixing_and_ignores_later_lines()
    {
        var conversation = new IpcConversation()
            .Send(ProtocolFixtures.Inbound("startMixing"))
            .Send(ProtocolFixtures.Inbound("shutdown"))
            .Send(ProtocolFixtures.Inbound("getState"));

        var messages = conversation.Run();

        Assert.Equal("ack", (string?)messages[^1]["type"]);
        Assert.Equal("r13", (string?)messages[^1]["requestId"]);
        Assert.True(conversation.Connections.Current.IsClosed);
    }

    [Fact]
    public void Every_inbound_fixture_is_understood()
    {
        foreach (var name in ProtocolFixtures.InboundNames().Where(name => name != "shutdown"))
        {
            var messages = new IpcConversation().Send(ProtocolFixtures.Inbound("startMixing")).Send(ProtocolFixtures.Inbound(name)).Run();

            Assert.DoesNotContain(messages, message => (string?)message["type"] == "error");
        }
    }
}
