namespace LeviMixer.Infrastructure.Ipc.Wire;

internal sealed record ReadyPayload(int ProtocolVersion, string EngineVersion);
