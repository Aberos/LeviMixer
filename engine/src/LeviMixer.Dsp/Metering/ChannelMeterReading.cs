using LeviMixer.Domain.Channels;

namespace LeviMixer.Dsp.Metering;

public readonly record struct ChannelMeterReading(ChannelId ChannelId, MeterReading Reading);
