import type { AudioEndpoint, ConnectionState } from '@levimixer/protocol';
import type { ChangeEvent } from 'react';
import { useMixerServices, useMixerState } from './MixerContext.tsx';

const bufferSizes = [64, 128, 256, 512, 1024] as const;

export function TopBar() {
  const { store } = useMixerServices();
  const { devices, mixer, status } = useMixerState();
  const connection = mixer?.connection ?? null;

  const choose = (change: Partial<ConnectionState>) => {
    if (connection !== null) {
      void store.selectDevices({ ...connection, ...change });
    }
  };

  return (
    <header className="top-bar">
      <h1 className="brand">LeviMixer</h1>
      <DeviceSelect label="Entrada" endpoints={devices.inputs} value={connection?.inputDeviceId} onChange={(inputDeviceId) => choose({ inputDeviceId })} />
      <DeviceSelect label="Saída" endpoints={devices.outputs} value={connection?.outputDeviceId} onChange={(outputDeviceId) => choose({ outputDeviceId })} />
      <label className="field">
        <span>Buffer</span>
        <select
          value={connection?.bufferFrames ?? ''}
          disabled={connection === null}
          onChange={(event: ChangeEvent<HTMLSelectElement>) => choose({ bufferFrames: Number(event.target.value) })}
        >
          {bufferSizes.map((size) => (
            <option key={size} value={size}>
              {size} ({((size / (connection?.sampleRate ?? 48_000)) * 1_000).toFixed(1)} ms)
            </option>
          ))}
        </select>
      </label>
      <span className={`engine-pill engine-pill-${status.state}`}>{status.state === 'running' ? `engine ${status.engineVersion}` : status.state}</span>
    </header>
  );
}

interface DeviceSelectProps {
  readonly label: string;
  readonly endpoints: readonly AudioEndpoint[];
  readonly value: string | undefined;
  readonly onChange: (id: string) => void;
}

function DeviceSelect({ label, endpoints, value, onChange }: DeviceSelectProps) {
  return (
    <label className="field field-device">
      <span>{label}</span>
      <select value={value ?? ''} disabled={value === undefined} onChange={(event) => onChange(event.target.value)}>
        {endpoints.map((endpoint) => (
          <option key={endpoint.id} value={endpoint.id}>
            {endpoint.name} · {endpoint.channelCount} ch
          </option>
        ))}
      </select>
    </label>
  );
}
