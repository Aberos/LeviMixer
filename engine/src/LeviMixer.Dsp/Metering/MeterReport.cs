using System.Collections.Immutable;

namespace LeviMixer.Dsp.Metering;

public sealed record MeterReport(ImmutableArray<ChannelMeterReading> Channels, MeterReading MasterLeft, MeterReading MasterRight);
