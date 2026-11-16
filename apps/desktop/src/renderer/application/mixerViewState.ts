import type { DeviceList, MixerState } from '@levimixer/protocol';
import type { DriverInstallOutcome, EngineStatus } from '../../shared/bridge.ts';

export type DriverInstallState = 'idle' | 'downloading' | 'runningSetup' | DriverInstallOutcome;

export interface MixerViewState {
  readonly status: EngineStatus;
  readonly devices: DeviceList;
  readonly mixer: MixerState | null;
  readonly error: string | null;
  readonly driverInstall: DriverInstallState;
  readonly driverDownloadPercent: number | null;
}

export const initialViewState: MixerViewState = {
  status: { state: 'starting' },
  devices: { inputs: [], outputs: [] },
  mixer: null,
  error: null,
  driverInstall: 'idle',
  driverDownloadPercent: null,
};
