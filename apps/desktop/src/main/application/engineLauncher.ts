import type { EngineGateway } from './engineGateway.ts';

export interface EngineConnection extends EngineGateway {
  onExit(listener: (reason: string) => void): () => void;
  shutdown(): Promise<void>;
}

export interface EngineLauncher {
  launch(): EngineConnection;
}
