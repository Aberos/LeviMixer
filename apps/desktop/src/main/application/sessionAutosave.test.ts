import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { savedSession } from '../testing/fixtures.ts';
import { MemorySessionRepository } from '../testing/memorySessionRepository.ts';
import { SessionAutosave } from './sessionAutosave.ts';

describe('SessionAutosave', () => {
  beforeEach(() => {
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('saves only the latest session after the quiet period', async () => {
    const repository = new MemorySessionRepository();
    const autosave = new SessionAutosave(repository, 500);

    autosave.schedule({ ...savedSession, masterGainDecibels: -1 });
    autosave.schedule({ ...savedSession, masterGainDecibels: -2 });
    await vi.advanceTimersByTimeAsync(500);

    expect(repository.saved.map((session) => session.masterGainDecibels)).toEqual([-2]);
  });

  it('flush saves immediately', async () => {
    const repository = new MemorySessionRepository();
    const autosave = new SessionAutosave(repository, 500);
    autosave.schedule(savedSession);

    await autosave.flush();

    expect(repository.saved).toHaveLength(1);
  });
});
