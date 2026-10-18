import type { SessionRepository } from '../application/sessionRepository.ts';
import type { Session } from '../domain/session.ts';

export class MemorySessionRepository implements SessionRepository {
  readonly saved: Session[] = [];
  #stored: Session | null;

  constructor(stored: Session | null = null) {
    this.#stored = stored;
  }

  load(): Promise<Session | null> {
    return Promise.resolve(this.#stored);
  }

  save(session: Session): Promise<void> {
    this.saved.push(session);
    this.#stored = session;
    return Promise.resolve();
  }
}
