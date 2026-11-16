using LeviMixer.Application.Devices;

namespace LeviMixer.Infrastructure.SystemAudio.Windows;

public interface IDefaultOutputControl
{
    DefaultOutputs Current();

    void PointAllTo(AudioDeviceId output);

    void Restore(DefaultOutputs previous);
}
