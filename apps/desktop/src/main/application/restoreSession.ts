import type { EngineReply } from '@mixer/protocol';
import { toSessionPayload, withDefaultDevices, type Session } from '../domain/session.ts';
import type { EngineGateway } from './engineGateway.ts';
import type { SessionRepository } from './sessionRepository.ts';

export class RestoreSession {
  readonly #engine: EngineGateway;
  readonly #repository: SessionRepository;

  constructor(engine: EngineGateway, repository: SessionRepository) {
    this.#engine = engine;
    this.#repository = repository;
  }

  async execute(): Promise<EngineReply> {
    const saved = await this.#repository.load();
    if (saved === null) {
      return this.#engine.request({ type: 'startMixing', payload: {} });
    }

    const reply = await this.#apply(saved);
    return isMissingDevice(reply) ? this.#apply(withDefaultDevices(saved)) : reply;
  }

  #apply(session: Session): Promise<EngineReply> {
    return this.#engine.request({ type: 'applySession', payload: toSessionPayload(session) });
  }
}

function isMissingDevice(reply: EngineReply): boolean {
  return reply.type === 'error' && reply.payload.code === 'deviceNotFound';
}
