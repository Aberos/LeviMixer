using System.Collections.Immutable;

namespace Mixer.Dsp.Metering;

public sealed record MeterReport(ImmutableArray<ChannelMeterReading> Channels, MeterReading MasterLeft, MeterReading MasterRight);
