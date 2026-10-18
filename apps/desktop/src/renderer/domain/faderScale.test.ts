import { describe, expect, it } from 'vitest';
import { decibelsToPosition, positionToDecibels } from './faderScale.ts';

describe('fader scale', () => {
  it('puts unity gain at three quarters of the travel', () => {
    expect(decibelsToPosition(0)).toBeCloseTo(0.75);
    expect(positionToDecibels(0.75)).toBe(0);
  });

  it('maps the extremes to silence and maximum boost', () => {
    expect(positionToDecibels(0)).toBeNull();
    expect(positionToDecibels(1)).toBe(10);
    expect(decibelsToPosition(null)).toBe(0);
  });

  it('clamps positions outside the travel', () => {
    expect(positionToDecibels(1.4)).toBe(10);
    expect(positionToDecibels(-1)).toBeNull();
  });

  it.each([-60, -20, -7.5, 0, 3, 10])('round-trips %d dB', (decibels) => {
    expect(positionToDecibels(decibelsToPosition(decibels))).toBeCloseTo(decibels, 1);
  });

  it('is monotonic', () => {
    const positions = Array.from({ length: 101 }, (_, index) => index / 100);
    const gains = positions.map((position) => positionToDecibels(position) ?? Number.NEGATIVE_INFINITY);

    expect(gains).toEqual([...gains].sort((first, second) => first - second));
  });
});
