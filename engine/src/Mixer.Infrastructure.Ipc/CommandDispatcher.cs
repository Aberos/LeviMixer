using System.Collections.Frozen;
using System.Text.Json;
using Mixer.Application.Results;
using Mixer.Infrastructure.Ipc.Handlers;
using Mixer.Infrastructure.Ipc.Transport;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc;

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
