using Mixer.Application.Devices;
using Mixer.Application.Results;
using Mixer.Domain.Channels;
using Mixer.Domain.Mixing;
using Mixer.Dsp.Metering;
using Mixer.Dsp.Mixing;

namespace Mixer.Application.Session;

public sealed class MixerSession : IDisposable
{
    private const int MaxFramesPerBlock = 8_192;

    private readonly IAudioConnectionFactory _connectionFactory;
    private readonly ISystemAudioCapture _systemAudioCapture;
    private readonly Lock _gate = new();
    private MixerState _state = DefaultChannelLayout.ForInterface(0);
    private MixEngine? _engine;
    private IAudioConnection? _connection;
    private IDisposable? _systemAudio;
    private string? _systemAudioError;

    public MixerSession(IAudioConnectionFactory connectionFactory, ISystemAudioCapture systemAudioCapture)
    {
        _connectionFactory = connectionFactory;
        _systemAudioCapture = systemAudioCapture;
    }

    public MixerState State => Status.State;

    public bool IsRunning => Status.IsRunning;

    public MixerStatus Status
    {
        get
        {
            lock (_gate)
            {
                return new MixerStatus(_state, _connection?.Request, _systemAudioError);
            }
        }
    }

    public void Start(AudioConnectionRequest request, MixerState initialState)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(initialState);
        lock (_gate)
        {
            CloseConnection();
            var engine = new MixEngine(new MixEngineCapacity(request.Format.SampleRate, MaxFramesPerBlock, initialState.Channels.Length));
            engine.Apply(MixSnapshot.From(initialState));
            var pipeline = new SystemAudioPipeline(engine, request.Format.BufferFrames, MaxFramesPerBlock);
            StartSystemAudio(pipeline, request.Format.SampleRate);
            OpenConnection(request, pipeline);
            _engine = engine;
            _state = initialState;
        }
    }

    public void Stop()
    {
        lock (_gate)
        {
            CloseConnection();
        }
    }

    public CommandResult Change(Func<MixerState, MixerState> change)
    {
        ArgumentNullException.ThrowIfNull(change);
        lock (_gate)
        {
            _state = change(_state);
            _engine?.Apply(MixSnapshot.From(_state));
            return CommandResult.Success;
        }
    }

    public CommandResult ChangeChannel(ChannelId id, Func<ChannelStrip, ChannelStrip> change)
    {
        lock (_gate)
        {
            return _state.Contains(id)
                ? Change(state => state.UpdateChannel(id, change))
                : CommandResult.Failure(CommandError.UnknownChannel(id.Value));
        }
    }

    public MeterReport? TakeMeterReport()
    {
        lock (_gate)
        {
            return _engine?.TakeMeterReport();
        }
    }

    public void Dispose() => Stop();

    private void StartSystemAudio(SystemAudioPipeline pipeline, int sampleRate)
    {
        try
        {
            _systemAudio = _systemAudioCapture.Start(pipeline.Ring, sampleRate);
            _systemAudioError = null;
        }
        catch (AudioDeviceException exception)
        {
            _systemAudioError = exception.Message;
        }
    }

    private void OpenConnection(AudioConnectionRequest request, SystemAudioPipeline pipeline)
    {
        try
        {
            _connection = _connectionFactory.Open(request, pipeline.Processor);
        }
        catch
        {
            CloseConnection();
            throw;
        }
    }

    private void CloseConnection()
    {
        _connection?.Dispose();
        _systemAudio?.Dispose();
        _connection = null;
        _systemAudio = null;
        _engine = null;
    }
}
