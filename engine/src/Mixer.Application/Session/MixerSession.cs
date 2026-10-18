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
    private readonly Lock _gate = new();
    private MixerState _state = DefaultChannelLayout.ForInterface(0);
    private MixEngine? _engine;
    private IAudioConnection? _connection;

    public MixerSession(IAudioConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public MixerState State
    {
        get
        {
            lock (_gate)
            {
                return _state;
            }
        }
    }

    public bool IsRunning
    {
        get
        {
            lock (_gate)
            {
                return _connection is not null;
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
            _connection = _connectionFactory.Open(request, engine);
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

    private void CloseConnection()
    {
        _connection?.Dispose();
        _connection = null;
        _engine = null;
    }
}
