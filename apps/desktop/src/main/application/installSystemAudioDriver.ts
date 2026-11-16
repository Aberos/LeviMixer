import type { DriverInstallOutcome } from '../../shared/bridge.ts';
import type { DriverInstallListener, SystemAudioDriverInstaller } from './systemAudioDriverInstaller.ts';

export class InstallSystemAudioDriver {
  readonly #installer: SystemAudioDriverInstaller;
  readonly #restartEngine: () => Promise<void>;
  #running: Promise<DriverInstallOutcome> | null = null;

  constructor(installer: SystemAudioDriverInstaller, restartEngine: () => Promise<void>) {
    this.#installer = installer;
    this.#restartEngine = restartEngine;
  }

  execute(onProgress: DriverInstallListener): Promise<DriverInstallOutcome> {
    this.#running ??= this.#install(onProgress).finally(() => {
      this.#running = null;
    });
    return this.#running;
  }

  async #install(onProgress: DriverInstallListener): Promise<DriverInstallOutcome> {
    const outcome = await this.#installer.install(onProgress);
    if (outcome === 'installed') {
      await this.#restartEngine();
    }

    return outcome;
  }
}
