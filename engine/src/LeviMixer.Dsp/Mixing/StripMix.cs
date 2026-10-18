using LeviMixer.Domain.Channels;
using LeviMixer.Dsp.Routing;

namespace LeviMixer.Dsp.Mixing;

public readonly record struct StripMix(ChannelId ChannelId, StripRoute Route, StereoGain Gain);
