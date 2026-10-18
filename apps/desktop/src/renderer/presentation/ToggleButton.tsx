interface ToggleButtonProps {
  readonly label: string;
  readonly title: string;
  readonly variant: 'mute' | 'solo';
  readonly active: boolean;
  readonly onToggle: () => void;
}

export function ToggleButton({ label, title, variant, active, onToggle }: ToggleButtonProps) {
  return (
    <button type="button" className={`toggle toggle-${variant}${active ? ' is-active' : ''}`} aria-pressed={active} title={title} aria-label={title} onClick={onToggle}>
      {label}
    </button>
  );
}
