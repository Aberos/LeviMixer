namespace LeviMixer.Infrastructure.SystemAudio.Windows;

internal static class VbCable
{
    public const string DownloadUrl = "https://vb-audio.com/Cable/";

    private const string PlaybackName = "CABLE Input";
    private const string CaptureName = "CABLE Output";
    private const string DriverName = "VB-Audio Virtual Cable";

    public static bool IsPlayback(string endpointName) => IsCableEndpoint(endpointName) && endpointName.StartsWith(PlaybackName, StringComparison.Ordinal);

    public static bool IsCapture(string endpointName) => IsCableEndpoint(endpointName) && endpointName.StartsWith(CaptureName, StringComparison.Ordinal);

    public static bool IsCableEndpoint(string endpointName) => endpointName.Contains(DriverName, StringComparison.Ordinal);
}
