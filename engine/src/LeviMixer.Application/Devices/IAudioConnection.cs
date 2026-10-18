namespace LeviMixer.Application.Devices;

public interface IAudioConnection : IDisposable
{
    AudioConnectionRequest Request { get; }
}
