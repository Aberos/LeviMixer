import type { GainDecibels } from '@mixer/protocol';
import { useRef, type KeyboardEvent, type PointerEvent, type WheelEvent } from 'react';
import { formatGain } from '../domain/decibels.ts';
import { decibelsToPosition, faderMarks, positionToDecibels } from '../domain/faderScale.ts';

interface FaderProps {
  readonly value: GainDecibels;
  readonly label: string;
  readonly onChange: (gain: GainDecibels) => void;
}

const keyboardStep = 0.01;
const wheelStep = 0.01;
const unityGain = 0;

export function Fader({ value, label, onChange }: FaderProps) {
  const track = useRef<HTMLDivElement>(null);
  const position = decibelsToPosition(value);

  const moveTo = (nextPosition: number) => onChange(positionToDecibels(nextPosition));

  const positionAt = (clientY: number): number => {
    const bounds = track.current?.getBoundingClientRect();
    return bounds === undefined || bounds.height === 0 ? position : 1 - (clientY - bounds.top) / bounds.height;
  };

  const handlePointerDown = (event: PointerEvent<HTMLDivElement>) => {
    event.currentTarget.setPointerCapture(event.pointerId);
    moveTo(positionAt(event.clientY));
  };

  const handlePointerMove = (event: PointerEvent<HTMLDivElement>) => {
    if (event.currentTarget.hasPointerCapture(event.pointerId)) {
      moveTo(positionAt(event.clientY));
    }
  };

  const handleWheel = (event: WheelEvent<HTMLDivElement>) => moveTo(position + (event.deltaY < 0 ? wheelStep : -wheelStep));

  const handleKey = (event: KeyboardEvent<HTMLDivElement>) => {
    const next = keyboardTarget(event.key, position);
    if (next !== null) {
      event.preventDefault();
      moveTo(next);
    }
  };

  return (
    <div className="fader">
      <div
        ref={track}
        className="fader-track"
        role="slider"
        tabIndex={0}
        aria-label={label}
        aria-orientation="vertical"
        aria-valuemin={0}
        aria-valuemax={1}
        aria-valuenow={Number(position.toFixed(3))}
        aria-valuetext={`${formatGain(value)} dB`}
        onPointerDown={handlePointerDown}
        onPointerMove={handlePointerMove}
        onWheel={handleWheel}
        onKeyDown={handleKey}
        onDoubleClick={() => onChange(unityGain)}
      >
        {faderMarks.map((mark) => (
          <span key={mark} className={`fader-mark${mark === unityGain ? ' is-unity' : ''}`} style={{ bottom: `${decibelsToPosition(mark) * 100}%` }}>
            {mark > 0 ? `+${mark}` : mark}
          </span>
        ))}
        <span className="fader-cap" style={{ bottom: `${position * 100}%` }} />
      </div>
      <output className="fader-readout">{formatGain(value)}</output>
    </div>
  );
}

function keyboardTarget(key: string, position: number): number | null {
  switch (key) {
    case 'ArrowUp':
    case 'ArrowRight':
      return position + keyboardStep;
    case 'ArrowDown':
    case 'ArrowLeft':
      return position - keyboardStep;
    case 'Home':
      return 1;
    case 'End':
      return 0;
    default:
      return null;
  }
}
