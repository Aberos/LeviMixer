using LeviMixer.Domain.Channels;

namespace LeviMixer.Domain.Mixing;

public static class DefaultChannelLayout
{
    public static ChannelId SystemAudioChannelId { get; } = new("pc");

    public static ChannelId InputChannelId(int inputIndex) => new($"in-{inputIndex + 1}");

    public static MixerState ForInterface(int inputChannelCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(inputChannelCount);
        var inputs = Enumerable.Range(0, inputChannelCount).Select(CreateInputStrip);
        return MixerState.Create(inputs.Append(CreateSystemAudioStrip()), MasterBus.AtUnity);
    }

    private static ChannelStrip CreateInputStrip(int inputIndex) =>
        new(InputChannelId(inputIndex), $"Input {inputIndex + 1}", new InterfaceInput(inputIndex));

    private static ChannelStrip CreateSystemAudioStrip() =>
        new(SystemAudioChannelId, "PC", SystemAudio.Instance);
}
