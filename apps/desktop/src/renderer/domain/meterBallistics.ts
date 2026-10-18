import { linearToDecibels } from './decibels.ts';
import { meterFloorDecibels } from './meterScale.ts';

export interface MeterDisplay {
  readonly levelDecibels: number;
  readonly peakHoldDecibels: number;
  readonly peakHoldAgeMilliseconds: number;
}

const releaseDecibelsPerSecond = 24;
const peakHoldMilliseconds = 1_200;

export const silentDisplay: MeterDisplay = {
  levelDecibels: meterFloorDecibels,
  peakHoldDecibels: meterFloorDecibels,
  peakHoldAgeMilliseconds: 0,
};

export function advanceMeter(previous: MeterDisplay, peakAmplitude: number, elapsedMilliseconds: number): MeterDisplay {
  const incoming = Math.max(meterFloorDecibels, linearToDecibels(peakAmplitude));
  const released = previous.levelDecibels - (releaseDecibelsPerSecond * elapsedMilliseconds) / 1_000;
  const levelDecibels = Math.max(incoming, released, meterFloorDecibels);
  return { levelDecibels, ...nextPeakHold(previous, levelDecibels, elapsedMilliseconds) };
}

function nextPeakHold(previous: MeterDisplay, levelDecibels: number, elapsedMilliseconds: number): Omit<MeterDisplay, 'levelDecibels'> {
  const age = previous.peakHoldAgeMilliseconds + elapsedMilliseconds;
  if (levelDecibels >= previous.peakHoldDecibels || age > peakHoldMilliseconds) {
    return { peakHoldDecibels: levelDecibels, peakHoldAgeMilliseconds: 0 };
  }

  return { peakHoldDecibels: previous.peakHoldDecibels, peakHoldAgeMilliseconds: age };
}
