import { mkdir, readFile, rename, writeFile } from 'node:fs/promises';
import { dirname } from 'node:path';
import type { SessionRepository } from '../application/sessionRepository.ts';
import type { Session } from '../domain/session.ts';
import { parseSession } from '../domain/sessionParser.ts';

export class JsonSessionRepository implements SessionRepository {
  readonly #path: string;

  constructor(path: string) {
    this.#path = path;
  }

  async load(): Promise<Session | null> {
    try {
      return parseSession(JSON.parse(await readFile(this.#path, 'utf8')));
    } catch {
      return null;
    }
  }

  async save(session: Session): Promise<void> {
    const temporaryPath = `${this.#path}.tmp`;
    await mkdir(dirname(this.#path), { recursive: true });
    await writeFile(temporaryPath, JSON.stringify(session, null, 2), 'utf8');
    await rename(temporaryPath, this.#path);
  }
}
