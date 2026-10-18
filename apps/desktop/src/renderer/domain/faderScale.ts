import type { GainDecibels } from '@mixer/protocol';

interface ScalePoint {
  readonly position: number;
  readonly decibels: number;
}

const scale: readonly ScalePoint[] = [
  { position: 0.02, decibels: -70 },
  { position: 0.1, decibels: -50 },
  { position: 0.25, decibels: -30 },
  { position: 0.4, decibels: -20 },
  { position: 0.55, decibels: -10 },
  { position: 0.65, decibels: -5 },
  { position: 0.75, decibels: 0 },
  { position: 0.875, decibels: 5 },
  { position: 1, decibels: 10 },
];

const silentPosition = 0;
const firstPoint = scale[0] as ScalePoint;
const lastPoint = scale[scale.length - 1] as ScalePoint;

export const faderMarks: readonly number[] = [10, 5, 0, -5, -10, -20, -30, -50];

export function positionToDecibels(position: number): GainDecibels {
  const clamped = clamp(position, silentPosition, 1);
  if (clamped < firstPoint.position) {
    return null;
  }

  const [lower, upper] = segmentAround(clamped, (point) => point.position);
  return roundToTenth(interpolate(clamped, lower.position, upper.position, lower.decibels, upper.decibels));
}

export function decibelsToPosition(decibels: GainDecibels): number {
  if (decibels === null || decibels < firstPoint.decibels) {
    return silentPosition;
  }

  const clamped = Math.min(decibels, lastPoint.decibels);
  const [lower, upper] = segmentAround(clamped, (point) => point.decibels);
  return interpolate(clamped, lower.decibels, upper.decibels, lower.position, upper.position);
}

function segmentAround(value: number, axis: (point: ScalePoint) => number): readonly [ScalePoint, ScalePoint] {
  for (let index = 1; index < scale.length; index++) {
    const upper = scale[index] as ScalePoint;
    if (value <= axis(upper)) {
      return [scale[index - 1] as ScalePoint, upper];
    }
  }

  return [scale[scale.length - 2] as ScalePoint, lastPoint];
}

function interpolate(value: number, fromStart: number, fromEnd: number, toStart: number, toEnd: number): number {
  return toStart + ((value - fromStart) / (fromEnd - fromStart)) * (toEnd - toStart);
}

function clamp(value: number, minimum: number, maximum: number): number {
  return Math.min(maximum, Math.max(minimum, value));
}

function roundToTenth(value: number): number {
  return Math.round(value * 10) / 10;
}
