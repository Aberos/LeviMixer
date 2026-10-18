import { describe, expect, it } from 'vitest';
import { isRendererCommand } from './commandGuard.ts';

describe('isRendererCommand', () => {
  it('accepts mixer commands', () => {
    expect(isRendererCommand({ type: 'setChannelGain', payload: { channelId: 'in-1', gainDecibels: 0 } })).toBe(true);
  });

  it.each([null, 'getState', { type: 'shutdown' }, { type: 'toString' }, { type: 42 }])('rejects %j', (value) => {
    expect(isRendererCommand(value)).toBe(false);
  });
});
