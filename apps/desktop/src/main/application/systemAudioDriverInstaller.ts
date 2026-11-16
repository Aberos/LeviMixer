import type { DriverInstallOutcome, DriverInstallProgress } from '../../shared/bridge.ts';

export type DriverInstallListener = (progress: DriverInstallProgress) => void;

export interface SystemAudioDriverInstaller {
  install(onProgress: DriverInstallListener): Promise<DriverInstallOutcome>;
}
