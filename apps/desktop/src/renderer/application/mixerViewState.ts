import type { DeviceList, MixerState } from '@mixer/protocol';
import type { EngineStatus } from '../../shared/bridge.ts';

export interface MixerViewState {
  readonly status: EngineStatus;
  readonly devices: DeviceList;
  readonly mixer: MixerState | null;
  readonly error: string | null;
}

export const initialViewState: MixerViewState = {
  status: { state: 'starting' },
  devices: { inputs: [], outputs: [] },
  mixer: null,
  error: null,
};
