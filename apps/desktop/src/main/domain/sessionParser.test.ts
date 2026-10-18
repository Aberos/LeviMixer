import { describe, expect, it } from 'vitest';
import { savedSession } from '../testing/fixtures.ts';
import { parseSession } from './sessionParser.ts';

describe('parseSession', () => {
  it('accepts a saved session', () => {
    expect(parseSession(JSON.parse(JSON.stringify(savedSession)))).toEqual(savedSession);
  });

  it.each([
    ['null', null],
    ['an unknown schema version', { ...savedSession, schemaVersion: 2 }],
    ['a channel without name', { ...savedSession, channels: [{ id: 'in-1', gainDecibels: 0, pan: 0, muted: false, soloed: false }] }],
    ['a zero buffer', { ...savedSession, bufferFrames: 0 }],
    ['an infinite gain', { ...savedSession, masterGainDecibels: 'loud' }],
  ])('rejects %s', (_, value) => {
    expect(parseSession(value)).toBeNull();
  });
});
