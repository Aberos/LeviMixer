using LeviMixer.Application.Results;
using LeviMixer.Application.UseCases;
using LeviMixer.Infrastructure.Ipc.Handlers;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc;

public sealed class IpcServer
{
    public const int ProtocolVersion = 1;
    private const string ShutdownCommand = "shutdown";

    private static readonly TimeSpan MeterInterval = TimeSpan.FromMilliseconds(33);

    private readonly MixerUseCases _useCases;
    private readonly OutboundChannel _outbound;
    private readonly CommandDispatcher _dispatcher;

    public IpcServer(MixerUseCases useCases, TextWriter output)
    {
        _useCases = useCases;
        _outbound = new OutboundChannel(output);
        _dispatcher = new CommandDispatcher(CreateHandlers(useCases));
    }

    public void Run(TextReader input, string engineVersion)
    {
        ArgumentNullException.ThrowIfNull(input);
        _outbound.Send(OutboundMessage.Create("ready", new ReadyPayload(ProtocolVersion, engineVersion), IpcJsonContext.Default.ReadyPayload));
        using var meters = new MeterPublisher(_useCases.ReadMeters, _outbound, MeterInterval);
        meters.Start();

        while (input.ReadLine() is { } line)
        {
            if (!HandleLine(line))
            {
                break;
            }
        }

        _useCases.StopMixing.Execute();
    }

    private bool HandleLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return true;
        }

        try
        {
            var command = InboundParser.Parse(line);
            if (command.Type == ShutdownCommand)
            {
                _outbound.Send(OutboundMessage.Ack(command.RequestId));
                return false;
            }

            _outbound.SendAll(_dispatcher.Dispatch(command));
        }
        catch (InvalidMessageException exception)
        {
            _outbound.Send(OutboundMessage.Error(new CommandError("invalidMessage", exception.Message), null));
        }

        return true;
    }

    private static IEnumerable<ICommandHandler> CreateHandlers(MixerUseCases useCases) =>
    [
        new ListDevicesHandler(useCases),
        new GetStateHandler(useCases),
        new StartMixingHandler(useCases),
        new StopMixingHandler(useCases),
        new ApplySessionHandler(useCases),
        new SetChannelGainHandler(useCases),
        new SetChannelMuteHandler(useCases),
        new SetChannelSoloHandler(useCases),
        new SetChannelPanHandler(useCases),
        new RenameChannelHandler(useCases),
        new SetMasterGainHandler(useCases),
    ];
}
