interface PanControlProps {
  readonly value: number;
  readonly label: string;
  readonly onChange: (pan: number) => void;
}

const centered = 0;

export function PanControl({ value, label, onChange }: PanControlProps) {
  return (
    <div className="pan">
      <input
        type="range"
        className="pan-slider"
        min={-1}
        max={1}
        step={0.05}
        value={value}
        aria-label={label}
        aria-valuetext={describePan(value)}
        onChange={(event) => onChange(Number(event.target.value))}
        onDoubleClick={() => onChange(centered)}
      />
      <span className="pan-readout">{describePan(value)}</span>
    </div>
  );
}

function describePan(value: number): string {
  if (Math.abs(value) < 0.025) {
    return 'C';
  }

  const percent = Math.round(Math.abs(value) * 100);
  return value < 0 ? `L${percent}` : `R${percent}`;
}
