import type { Session } from '../domain/session.ts';

export interface SessionRepository {
  load(): Promise<Session | null>;
  save(session: Session): Promise<void>;
}
