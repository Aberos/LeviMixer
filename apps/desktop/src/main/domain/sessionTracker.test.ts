import { describe, expect, it } from 'vitest';
import { runningState } from '../testing/fixtures.ts';
import { SessionTracker } from './sessionTracker.ts';

describe('SessionTracker', () => {
  it('ignores commands until the first mixer state arrives', () => {
    const tracker = new SessionTracker();

    expect(tracker.record({ type: 'setMasterGain', payload: { gainDecibels: 0 } })).toBe(false);
    expect(tracker.current).toBeNull();
  });

  it('records changes after observing the mixer state', () => {
    const tracker = new SessionTracker();
    tracker.observe({ type: 'mixerState', requestId: null, payload: runningState });

    const changed = tracker.record({ type: 'setMasterGain', payload: { gainDecibels: 3 } });

    expect(changed).toBe(true);
    expect(tracker.current?.masterGainDecibels).toBe(3);
  });

  it('reports no change for commands that do not affect the session', () => {
    const tracker = new SessionTracker();
    tracker.observe({ type: 'mixerState', requestId: null, payload: runningState });

    expect(tracker.record({ type: 'listDevices' })).toBe(false);
  });
});
