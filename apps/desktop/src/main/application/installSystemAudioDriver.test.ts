import { describe, expect, it } from 'vitest';
import type { DriverInstallOutcome } from '../../shared/bridge.ts';
import { InstallSystemAudioDriver } from './installSystemAudioDriver.ts';
import type { DriverInstallListener, SystemAudioDriverInstaller } from './systemAudioDriverInstaller.ts';

const ignoreProgress: DriverInstallListener = () => undefined;

class ScriptedInstaller implements SystemAudioDriverInstaller {
  readonly #outcome: DriverInstallOutcome;
  runs = 0;

  constructor(outcome: DriverInstallOutcome) {
    this.#outcome = outcome;
  }

  install(onProgress: DriverInstallListener): Promise<DriverInstallOutcome> {
    onProgress({ stage: 'runningSetup' });
    this.runs++;
    return Promise.resolve(this.#outcome);
  }
}

describe('InstallSystemAudioDriver', () => {
  it('restarts the engine after installing so the PC channel picks up the cable', async () => {
    let restarts = 0;
    const install = new InstallSystemAudioDriver(new ScriptedInstaller('installed'), () => {
      restarts++;
      return Promise.resolve();
    });

    expect(await install.execute(ignoreProgress)).toBe('installed');
    expect(restarts).toBe(1);
  });

  it('leaves the engine alone when only the website was opened', async () => {
    let restarts = 0;
    const install = new InstallSystemAudioDriver(new ScriptedInstaller('openedWebsite'), () => {
      restarts++;
      return Promise.resolve();
    });

    expect(await install.execute(ignoreProgress)).toBe('openedWebsite');
    expect(restarts).toBe(0);
  });

  it('runs a single install when asked twice at once', async () => {
    const installer = new ScriptedInstaller('installed');
    const install = new InstallSystemAudioDriver(installer, () => Promise.resolve());

    await Promise.all([install.execute(ignoreProgress), install.execute(ignoreProgress)]);

    expect(installer.runs).toBe(1);
  });
});
