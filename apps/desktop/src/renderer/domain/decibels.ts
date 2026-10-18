import type { GainDecibels } from '@mixer/protocol';

export function formatGain(decibels: GainDecibels): string {
  if (decibels === null) {
    return '-∞';
  }

  const rounded = Math.round(decibels * 10) / 10;
  return `${rounded > 0 ? '+' : ''}${rounded.toFixed(1)}`;
}

export function linearToDecibels(amplitude: number): number {
  return amplitude <= 0 ? Number.NEGATIVE_INFINITY : 20 * Math.log10(amplitude);
}
