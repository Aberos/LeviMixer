import type { EngineCommand, EngineMessage, EngineReply } from '@mixer/protocol';

export type MessageListener = (message: EngineMessage) => void;

export interface EngineGateway {
  request(command: EngineCommand): Promise<EngineReply>;
  onMessage(listener: MessageListener): () => void;
}
