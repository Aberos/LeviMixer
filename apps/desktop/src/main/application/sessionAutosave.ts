import type { Session } from '../domain/session.ts';
import type { SessionRepository } from './sessionRepository.ts';

export class SessionAutosave {
  readonly #repository: SessionRepository;
  readonly #delayMilliseconds: number;
  #pending: Session | null = null;
  #timer: ReturnType<typeof setTimeout> | null = null;

  constructor(repository: SessionRepository, delayMilliseconds: number) {
    this.#repository = repository;
    this.#delayMilliseconds = delayMilliseconds;
  }

  schedule(session: Session): void {
    this.#pending = session;
    if (this.#timer !== null) {
      clearTimeout(this.#timer);
    }

    this.#timer = setTimeout(() => void this.flush(), this.#delayMilliseconds);
  }

  async flush(): Promise<void> {
    if (this.#timer !== null) {
      clearTimeout(this.#timer);
      this.#timer = null;
    }

    const session = this.#pending;
    this.#pending = null;
    if (session !== null) {
      await this.#repository.save(session);
    }
  }
}
