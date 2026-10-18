import type { EngineCommand, EngineReply } from '@mixer/protocol';
import type { EngineGateway, MessageListener } from '../application/engineGateway.ts';

export class FakeEngineGateway implements EngineGateway {
  readonly requests: EngineCommand[] = [];
  readonly #replies: EngineReply[] = [];

  replyWith(...replies: EngineReply[]): this {
    this.#replies.push(...replies);
    return this;
  }

  request(command: EngineCommand): Promise<EngineReply> {
    this.requests.push(command);
    return Promise.resolve(this.#replies.shift() ?? { type: 'ack', requestId: null, payload: null });
  }

  onMessage(_listener: MessageListener): () => void {
    return () => undefined;
  }
}
