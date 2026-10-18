import type { MixerState } from '@levimixer/protocol';
import type { Session } from '../domain/session.ts';

export const runningState: MixerState = {
  running: true,
  connection: { inputDeviceId: 'umc22-in', outputDeviceId: 'umc22-out', sampleRate: 48_000, bufferFrames: 128 },
  channels: [
    { id: 'in-1', name: 'Vocal', source: 'input', inputIndex: 0, gainDecibels: -3, pan: 0, muted: false, soloed: false },
    { id: 'pc', name: 'PC', source: 'system', inputIndex: null, gainDecibels: 0, pan: 0, muted: false, soloed: false },
  ],
  masterGainDecibels: -6,
  systemAudioError: null,
};

export const savedSession: Session = {
  schemaVersion: 1,
  inputDeviceId: 'umc22-in',
  outputDeviceId: 'umc22-out',
  sampleRate: 48_000,
  bufferFrames: 128,
  channels: [
    { id: 'in-1', name: 'Vocal', gainDecibels: -3, pan: 0, muted: false, soloed: false },
    { id: 'pc', name: 'PC', gainDecibels: 0, pan: 0, muted: false, soloed: false },
  ],
  masterGainDecibels: -6,
};
