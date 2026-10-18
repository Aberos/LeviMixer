using Mixer.Domain.Channels;

namespace Mixer.Dsp.Metering;

public readonly record struct ChannelMeterReading(ChannelId ChannelId, MeterReading Reading);
