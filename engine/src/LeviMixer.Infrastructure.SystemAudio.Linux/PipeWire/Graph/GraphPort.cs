namespace LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire.Graph;

internal sealed record GraphPort(uint Id, uint NodeId, PortDirection Direction, string Name, uint Order, bool IsMonitor);
