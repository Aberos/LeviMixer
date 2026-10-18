export const meterFloorDecibels = -60;
export const meterCeilingDecibels = 6;
export const meterMarks: readonly number[] = [6, 0, -6, -12, -24, -40, -60];

export function meterPosition(decibels: number): number {
  if (!Number.isFinite(decibels) || decibels <= meterFloorDecibels) {
    return 0;
  }

  return Math.min(1, (decibels - meterFloorDecibels) / (meterCeilingDecibels - meterFloorDecibels));
}

export type MeterZone = 'safe' | 'hot' | 'clip';

export function meterZone(decibels: number): MeterZone {
  if (decibels >= 0) {
    return 'clip';
  }

  return decibels >= -6 ? 'hot' : 'safe';
}
