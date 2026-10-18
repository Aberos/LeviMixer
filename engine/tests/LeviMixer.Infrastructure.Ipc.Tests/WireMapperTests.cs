using System.Text.Json;
using System.Text.Json.Nodes;
using LeviMixer.Domain.Channels;
using LeviMixer.Dsp.Metering;
using LeviMixer.Infrastructure.Ipc.Mapping;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc.Tests;

public sealed class WireMapperTests
{
    [Fact]
    public void Meter_report_matches_the_protocol_fixture()
    {
        var level = new MeterReading(0.5f, 0.25f);
        var report = new MeterReport(
            [new ChannelMeterReading(new ChannelId("in-1"), level), new ChannelMeterReading(new ChannelId("pc"), MeterReading.Silence)],
            level,
            level);

        var message = OutboundMessage.Create("meters", WireMapper.ToWire(report), IpcJsonContext.Default.MetersPayload);

        var json = JsonNode.Parse(JsonSerializer.Serialize(message, IpcJsonContext.Default.OutboundEnvelope));
        Assert.True(JsonNode.DeepEquals(ProtocolFixtures.Outbound("meters"), json));
    }

    [Fact]
    public void Silent_gain_travels_as_null()
    {
        Assert.Null(DecibelsWire.ToWire(LeviMixer.Domain.Levels.Decibels.MinusInfinity));
        Assert.True(DecibelsWire.FromWire(null).IsSilent);
    }
}
