import type { LevelReading } from '@mixer/protocol';
import { useRef } from 'react';
import { advanceMeter, silentDisplay, type MeterDisplay } from '../domain/meterBallistics.ts';
import { meterPosition, meterZone } from '../domain/meterScale.ts';
import { useAnimationFrame } from './useAnimationFrame.ts';

const zoneColors = {
  safe: '#3ddc84',
  hot: '#f5c542',
  clip: '#ff4d4f',
} as const;

const barGap = 2;

export function LevelMeter({ read }: { readonly read: () => readonly LevelReading[] }) {
  const canvas = useRef<HTMLCanvasElement>(null);
  const displays = useRef<MeterDisplay[]>([]);

  useAnimationFrame((elapsed) => {
    const readings = read();
    displays.current = readings.map((reading, index) => advanceMeter(displays.current[index] ?? silentDisplay, reading.peak, elapsed));
    const context = canvas.current?.getContext('2d');
    if (canvas.current !== null && context !== null && context !== undefined) {
      draw(context, canvas.current, displays.current);
    }
  });

  return <canvas ref={canvas} className="meter" width={14} height={240} aria-hidden="true" />;
}

function draw(context: CanvasRenderingContext2D, canvas: HTMLCanvasElement, displays: readonly MeterDisplay[]): void {
  context.clearRect(0, 0, canvas.width, canvas.height);
  const barWidth = (canvas.width - barGap * (displays.length - 1)) / Math.max(1, displays.length);
  displays.forEach((display, index) => {
    const x = index * (barWidth + barGap);
    const height = meterPosition(display.levelDecibels) * canvas.height;
    context.fillStyle = zoneColors[meterZone(display.levelDecibels)];
    context.fillRect(x, canvas.height - height, barWidth, height);
    const peakY = canvas.height - meterPosition(display.peakHoldDecibels) * canvas.height;
    context.fillStyle = zoneColors[meterZone(display.peakHoldDecibels)];
    context.fillRect(x, Math.max(0, peakY - 2), barWidth, 2);
  });
}
