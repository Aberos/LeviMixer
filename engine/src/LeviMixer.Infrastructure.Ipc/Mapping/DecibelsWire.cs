using LeviMixer.Domain.Levels;

namespace LeviMixer.Infrastructure.Ipc.Mapping;

internal static class DecibelsWire
{
    public static float? ToWire(Decibels gain) => gain.IsSilent ? null : gain.Value;

    public static Decibels FromWire(float? gain) => gain is { } value ? new Decibels(value) : Decibels.MinusInfinity;
}
