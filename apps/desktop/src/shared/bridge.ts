import type { EngineCommand, EngineMessage, EngineReply } from '@levimixer/protocol';

export type EngineStatus =
  | { readonly state: 'starting' }
  | { readonly state: 'running'; readonly engineVersion: string }
  | { readonly state: 'stopped'; readonly reason: string };

export type DriverInstallOutcome = 'installed' | 'openedWebsite';

export type DriverInstallProgress =
  | { readonly stage: 'downloading'; readonly percent: number | null }
  | { readonly stage: 'runningSetup' };

export type BridgeEvent =
  | { readonly kind: 'engine'; readonly message: EngineMessage }
  | { readonly kind: 'status'; readonly status: EngineStatus }
  | { readonly kind: 'driverInstall'; readonly progress: DriverInstallProgress };

export interface MixerBridge {
  request(command: EngineCommand): Promise<EngineReply>;
  engineStatus(): Promise<EngineStatus>;
  restartEngine(): Promise<void>;
  installSystemAudioDriver(): Promise<DriverInstallOutcome>;
  onEvent(listener: (event: BridgeEvent) => void): () => void;
}

export type BridgeChannel = 'mixer:request' | 'mixer:status' | 'mixer:restart' | 'mixer:installDriver' | 'mixer:event';
