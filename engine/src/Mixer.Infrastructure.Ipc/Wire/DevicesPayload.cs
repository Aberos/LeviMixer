namespace Mixer.Infrastructure.Ipc.Wire;

internal sealed record DevicesPayload(EndpointWire[] Inputs, EndpointWire[] Outputs);
