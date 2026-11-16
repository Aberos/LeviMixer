using LeviMixer.Application.Devices;

namespace LeviMixer.Infrastructure.SystemAudio.Windows.Tests;

internal static class WindowsDevices
{
    public static AudioEndpoint InterfaceInput { get; } = new(new AudioDeviceId("Microfone (USB Audio CODEC )"), "Microfone (USB Audio CODEC )", 2, true);

    public static AudioEndpoint InterfaceOutput { get; } = new(new AudioDeviceId("Alto-falantes (USB Audio CODEC )"), "Alto-falantes (USB Audio CODEC )", 2, true);

    public static AudioEndpoint Monitor { get; } = new(new AudioDeviceId("Tempest (NVIDIA High Definition Audio)"), "Tempest (NVIDIA High Definition Audio)", 2, false);

    public static AudioEndpoint CableCapture { get; } = new(new AudioDeviceId("CABLE Output (VB-Audio Virtual Cable)"), "CABLE Output (VB-Audio Virtual Cable)", 2, false);

    public static AudioEndpoint CablePlayback { get; } = new(new AudioDeviceId("CABLE Input (VB-Audio Virtual Cable)"), "CABLE Input (VB-Audio Virtual Cable)", 2, false);

    public static DefaultOutputs InterfaceAsDefault { get; } = new("{0.0.0.00000000}.{interface}", "{0.0.0.00000000}.{interface}", "{0.0.0.00000000}.{headset}");
}
