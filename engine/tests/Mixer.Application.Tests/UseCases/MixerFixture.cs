using Mixer.Application.Devices;
using Mixer.Application.Session;
using Mixer.Testing.Fakes;
using Mixer.Application.UseCases;

namespace Mixer.Application.Tests.UseCases;

internal sealed class MixerFixture : IDisposable
{
    public MixerFixture()
    {
        Session = new MixerSession(Connections, SystemAudio);
        StartMixing = new StartMixing(Catalog, Session);
    }

    public FakeAudioDeviceCatalog Catalog { get; } = new();

    public FakeAudioConnectionFactory Connections { get; } = new();

    public FakeSystemAudioCapture SystemAudio { get; } = new();

    public MixerSession Session { get; }

    public StartMixing StartMixing { get; }

    public MixerFixture StartWithDefaults()
    {
        StartMixing.Execute(new StartMixingRequest(null, null, StreamFormat.Default));
        return this;
    }

    public float SettledLeftOutputFor(float inputSample)
    {
        Connections.Current.Render(inputSample, 32);
        return Connections.Current.Render(inputSample, 32)[^2];
    }

    public void Dispose() => Session.Dispose();
}
