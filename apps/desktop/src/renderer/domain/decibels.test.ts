import { describe, expect, it } from 'vitest';
import { formatGain, linearToDecibels } from './decibels.ts';

describe('decibels', () => {
  it.each([
    [null, '-∞'],
    [0, '0.0'],
    [3.04, '+3.0'],
    [-12.46, '-12.5'],
  ])('formats %s as %s', (gain, text) => {
    expect(formatGain(gain)).toBe(text);
  });

  it('converts amplitude to decibels', () => {
    expect(linearToDecibels(1)).toBe(0);
    expect(linearToDecibels(0.5)).toBeCloseTo(-6.02, 2);
    expect(linearToDecibels(0)).toBe(Number.NEGATIVE_INFINITY);
  });
});
