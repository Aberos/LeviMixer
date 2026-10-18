import type { EngineCommand, EngineMessage } from '@mixer/protocol';
import type { SessionTracker } from '../domain/sessionTracker.ts';
import type { SessionAutosave } from './sessionAutosave.ts';

export class SessionKeeper {
  readonly #tracker: SessionTracker;
  readonly #autosave: SessionAutosave;

  constructor(tracker: SessionTracker, autosave: SessionAutosave) {
    this.#tracker = tracker;
    this.#autosave = autosave;
  }

  observe(message: EngineMessage): void {
    if (this.#tracker.observe(message)) {
      this.#saveCurrent();
    }
  }

  record(command: EngineCommand): void {
    if (this.#tracker.record(command)) {
      this.#saveCurrent();
    }
  }

  flush(): Promise<void> {
    return this.#autosave.flush();
  }

  #saveCurrent(): void {
    const session = this.#tracker.current;
    if (session !== null) {
      this.#autosave.schedule(session);
    }
  }
}
