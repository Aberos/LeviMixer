using Mixer.Application.Devices;
using Mixer.Application.Session;
using Mixer.Application.Settings;
using Mixer.Domain.Channels;
using Mixer.Dsp.Metering;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc.Mapping;

internal static class WireMapper
{
    public static DevicesPayload ToWire(AudioDeviceList devices) =>
        new([.. devices.Inputs.Select(ToWire)], [.. devices.Outputs.Select(ToWire)]);

    public static MixerStatePayload ToWire(MixerStatus status) =>
        new(status.IsRunning, ToWire(status.Connection), [.. status.State.Channels.Select(ToWire)], DecibelsWire.ToWire(status.State.Master.Gain), status.SystemAudioError);

    public static MetersPayload ToWire(MeterReport report) =>
        new([.. report.Channels.Select(channel => new ChannelMeterWire(channel.ChannelId.Value, channel.Reading.Peak, channel.Reading.Rms))],
            ToWire(report.MasterLeft),
            ToWire(report.MasterRight));

    public static StreamFormat FormatFrom(int? sampleRate, int? bufferFrames) =>
        new(sampleRate ?? StreamFormat.Default.SampleRate, bufferFrames ?? StreamFormat.Default.BufferFrames);

    public static AudioDeviceId? DeviceIdFrom(string? id) => string.IsNullOrWhiteSpace(id) ? null : new AudioDeviceId(id);

    public static ChannelSettings FromWire(ChannelSettingsWire channel) =>
        new(new ChannelId(channel.Id), channel.Name, DecibelsWire.FromWire(channel.GainDecibels), new Pan(channel.Pan), channel.Muted, channel.Soloed);

    private static EndpointWire ToWire(AudioEndpoint endpoint) =>
        new(endpoint.Id.Value, endpoint.Name, endpoint.ChannelCount, endpoint.IsDefault);

    private static ConnectionWire? ToWire(AudioConnectionRequest? connection) =>
        connection is null
            ? null
            : new ConnectionWire(connection.Input.Id.Value, connection.Output.Id.Value, connection.Format.SampleRate, connection.Format.BufferFrames);

    private static ChannelWire ToWire(ChannelStrip channel) => channel.Source switch
    {
        InterfaceInput input => ChannelWireOf(channel, "input", input.Index),
        _ => ChannelWireOf(channel, "system", null),
    };

    private static ChannelWire ChannelWireOf(ChannelStrip channel, string source, int? inputIndex) =>
        new(channel.Id.Value, channel.Name, source, inputIndex, DecibelsWire.ToWire(channel.Gain), channel.Pan.Position, channel.IsMuted, channel.IsSoloed);

    private static LevelWire ToWire(MeterReading reading) => new(reading.Peak, reading.Rms);
}
