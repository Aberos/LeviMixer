using System.Collections.Immutable;
using Mixer.Domain.Channels;
using Mixer.Domain.Levels;
using Mixer.Domain.Mixing;
using Mixer.Dsp.Routing;

namespace Mixer.Dsp.Mixing;

public sealed class MixSnapshot
{
    private MixSnapshot(ImmutableArray<StripMix> strips, LinearGain master)
    {
        Strips = strips;
        Master = master;
    }

    public static MixSnapshot Silent { get; } = new([], LinearGain.Silence);

    public ImmutableArray<StripMix> Strips { get; }

    public LinearGain Master { get; }

    public static MixSnapshot From(MixerState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        var strips = state.Channels.Select(channel => ToStripMix(state, channel)).ToImmutableArray();
        return new MixSnapshot(strips, state.Master.Gain.ToLinear());
    }

    private static StripMix ToStripMix(MixerState state, ChannelStrip channel)
    {
        var route = RouteOf(channel.Source);
        var gain = state.IsAudible(channel) ? PanGainOf(route, channel.Pan).Scale(channel.Gain.ToLinear()) : StereoGain.Silence;
        return new StripMix(channel.Id, route, gain);
    }

    private static StripRoute RouteOf(ChannelSource source) => source switch
    {
        InterfaceInput input => StripRoute.FromInterface(input.Index),
        SystemAudio => StripRoute.FromSystemAudio(),
        _ => throw new ArgumentOutOfRangeException(nameof(source), source, "Unsupported channel source."),
    };

    private static StereoGain PanGainOf(StripRoute route, Pan pan) =>
        route.Kind == StripSourceKind.SystemStereo ? PanLaw.Balance(pan) : PanLaw.ConstantPower(pan);
}
