import type { EngineCommand, EngineMessage, EngineReply } from '@mixer/protocol';

export type EngineStatus =
  | { readonly state: 'starting' }
  | { readonly state: 'running'; readonly engineVersion: string }
  | { readonly state: 'stopped'; readonly reason: string };

export type BridgeEvent =
  | { readonly kind: 'engine'; readonly message: EngineMessage }
  | { readonly kind: 'status'; readonly status: EngineStatus };

export interface MixerBridge {
  request(command: EngineCommand): Promise<EngineReply>;
  engineStatus(): Promise<EngineStatus>;
  restartEngine(): Promise<void>;
  onEvent(listener: (event: BridgeEvent) => void): () => void;
}

export type BridgeChannel = 'mixer:request' | 'mixer:status' | 'mixer:restart' | 'mixer:event';
