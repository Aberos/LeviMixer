namespace LeviMixer.Dsp.Routing;

public readonly record struct StripRoute(StripSourceKind Kind, int InterfaceChannel)
{
    public static StripRoute FromInterface(int channel) => new(StripSourceKind.InterfaceMono, channel);

    public static StripRoute FromSystemAudio() => new(StripSourceKind.SystemStereo, 0);
}
