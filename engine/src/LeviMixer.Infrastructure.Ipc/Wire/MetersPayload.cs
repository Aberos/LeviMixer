namespace LeviMixer.Infrastructure.Ipc.Wire;

internal sealed record MetersPayload(ChannelMeterWire[] Channels, LevelWire MasterLeft, LevelWire MasterRight);
