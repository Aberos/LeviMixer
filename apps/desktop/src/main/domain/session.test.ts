import { describe, expect, it } from 'vitest';
import { runningState, savedSession } from '../testing/fixtures.ts';
import { applyCommand, sessionFromState, withDefaultDevices } from './session.ts';

describe('session', () => {
  it('is captured from a running mixer state', () => {
    expect(sessionFromState(runningState)).toEqual(savedSession);
  });

  it('is not captured while the mixer is stopped', () => {
    expect(sessionFromState({ ...runningState, running: false, connection: null })).toBeNull();
  });

  it('follows channel and master changes', () => {
    const changed = [
      { type: 'setChannelGain', payload: { channelId: 'in-1', gainDecibels: null } },
      { type: 'setChannelMute', payload: { channelId: 'pc', value: true } },
      { type: 'setChannelPan', payload: { channelId: 'in-1', pan: -0.5 } },
      { type: 'renameChannel', payload: { channelId: 'pc', name: 'Spotify' } },
      { type: 'setMasterGain', payload: { gainDecibels: -12 } },
    ] as const;

    const session = changed.reduce(applyCommand, savedSession);

    expect(session.channels).toEqual([
      { id: 'in-1', name: 'Vocal', gainDecibels: null, pan: -0.5, muted: false, soloed: false },
      { id: 'pc', name: 'Spotify', gainDecibels: 0, pan: 0, muted: true, soloed: false },
    ]);
    expect(session.masterGainDecibels).toBe(-12);
  });

  it('ignores commands for unknown channels', () => {
    expect(applyCommand(savedSession, { type: 'setChannelGain', payload: { channelId: 'in-9', gainDecibels: 0 } })).toBe(savedSession);
  });

  it('can fall back to default devices', () => {
    expect(withDefaultDevices(savedSession)).toMatchObject({ inputDeviceId: null, outputDeviceId: null, channels: savedSession.channels });
  });
});
