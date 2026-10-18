import type { EngineCommand, EngineMessage } from '@levimixer/protocol';
import { applyCommand, sessionFromState, type Session } from './session.ts';

export class SessionTracker {
  #current: Session | null = null;

  get current(): Session | null {
    return this.#current;
  }

  observe(message: EngineMessage): boolean {
    if (message.type !== 'mixerState') {
      return false;
    }

    const session = sessionFromState(message.payload);
    if (session === null) {
      return false;
    }

    this.#current = session;
    return true;
  }

  record(command: EngineCommand): boolean {
    if (this.#current === null) {
      return false;
    }

    const session = applyCommand(this.#current, command);
    const changed = session !== this.#current;
    this.#current = session;
    return changed;
  }
}
