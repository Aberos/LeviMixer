import { describe, expect, it } from 'vitest';
import { advanceMeter, silentDisplay } from './meterBallistics.ts';
import { meterPosition, meterZone } from './meterScale.ts';

describe('meter ballistics', () => {
  it('attacks instantly', () => {
    expect(advanceMeter(silentDisplay, 1, 16).levelDecibels).toBe(0);
  });

  it('releases gradually', () => {
    const loud = advanceMeter(silentDisplay, 1, 16);

    expect(advanceMeter(loud, 0, 500).levelDecibels).toBeCloseTo(-12, 5);
  });

  it('holds the peak before letting it fall', () => {
    const loud = advanceMeter(silentDisplay, 1, 16);
    const held = advanceMeter(loud, 0, 1_000);
    const released = advanceMeter(held, 0, 500);

    expect(held.peakHoldDecibels).toBe(0);
    expect(released.peakHoldDecibels).toBeLessThan(0);
  });
});

describe('meter scale', () => {
  it('spans from the floor to the ceiling', () => {
    expect(meterPosition(-80)).toBe(0);
    expect(meterPosition(6)).toBe(1);
    expect(meterPosition(Number.NEGATIVE_INFINITY)).toBe(0);
  });

  it('colours by zone', () => {
    expect([meterZone(-20), meterZone(-3), meterZone(0.5)]).toEqual(['safe', 'hot', 'clip']);
  });
});
