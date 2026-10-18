using System.Collections.Immutable;
using LeviMixer.Domain.Channels;
using LeviMixer.Domain.Levels;

namespace LeviMixer.Domain.Mixing;

public sealed record MixerState
{
    private MixerState(ImmutableArray<ChannelStrip> channels, MasterBus master)
    {
        Channels = channels;
        Master = master;
    }

    public ImmutableArray<ChannelStrip> Channels { get; }

    public MasterBus Master { get; }

    public bool HasSoloedChannel => Channels.Any(channel => channel.IsSoloed);

    public static MixerState Create(IEnumerable<ChannelStrip> channels, MasterBus master)
    {
        var strips = channels.ToImmutableArray();
        EnsureUniqueIds(strips);
        return new MixerState(strips, master);
    }

    public bool Contains(ChannelId id) => Channels.Any(channel => channel.Id == id);

    public bool IsAudible(ChannelStrip channel) => !channel.IsMuted && (channel.IsSoloed || !HasSoloedChannel);

    public MixerState UpdateChannel(ChannelId id, Func<ChannelStrip, ChannelStrip> change)
    {
        ArgumentNullException.ThrowIfNull(change);
        var index = IndexOf(id);
        return new MixerState(Channels.SetItem(index, change(Channels[index])), Master);
    }

    public MixerState WithMasterGain(Decibels gain) => new(Channels, Master with { Gain = gain });

    private int IndexOf(ChannelId id)
    {
        for (var index = 0; index < Channels.Length; index++)
        {
            if (Channels[index].Id == id)
            {
                return index;
            }
        }

        throw new KeyNotFoundException($"Channel '{id}' does not exist.");
    }

    private static void EnsureUniqueIds(ImmutableArray<ChannelStrip> strips)
    {
        var duplicated = strips.GroupBy(strip => strip.Id).FirstOrDefault(group => group.Count() > 1);
        if (duplicated is not null)
        {
            throw new ArgumentException($"Channel '{duplicated.Key}' appears more than once.", nameof(strips));
        }
    }
}
