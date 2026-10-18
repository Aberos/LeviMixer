using Mixer.Domain.Channels;
using Mixer.Dsp.Routing;

namespace Mixer.Dsp.Mixing;

public readonly record struct StripMix(ChannelId ChannelId, StripRoute Route, StereoGain Gain);
