using System.Collections.Frozen;
using System.Text.Json;
using LeviMixer.Application.Results;
using LeviMixer.Infrastructure.Ipc.Handlers;
using LeviMixer.Infrastructure.Ipc.Transport;
using LeviMixer.Infrastructure.Ipc.Wire;

namespace LeviMixer.Infrastructure.Ipc;

internal sealed class CommandDispatcher(IEnumerable<ICommandHandler> handlers)
{
    private readonly FrozenDictionary<string, ICommandHandler> _handlers = handlers.ToFrozenDictionary(handler => handler.Type, StringComparer.Ordinal);

    public IReadOnlyList<OutboundEnvelope> Dispatch(InboundCommand command)
    {
        if (!_handlers.TryGetValue(command.Type, out var handler))
        {
            return [OutboundMessage.Error(new CommandError("unknownCommand", $"Command '{command.Type}' is not supported."), command.RequestId)];
        }

        try
        {
            return [.. handler.Handle(command)];
        }
        catch (Exception exception) when (exception is InvalidMessageException or JsonException or ArgumentException)
        {
            return [OutboundMessage.Error(new CommandError("invalidMessage", exception.Message), command.RequestId)];
        }
    }
}
