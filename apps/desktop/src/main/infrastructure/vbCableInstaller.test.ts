import { existsSync } from 'node:fs';
import { mkdtemp, readdir, rm } from 'node:fs/promises';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import type { DriverInstallProgress } from '../../shared/bridge.ts';
import { installScript, VbCableInstaller, vbCableDownloadUrl, vbCableWebsite, type VbCableInstallerDependencies } from './vbCableInstaller.ts';

describe('VbCableInstaller', () => {
  let temporaryRoot = '';
  const downloads: string[] = [];
  const scripts: string[] = [];
  const opened: string[] = [];
  const progress: DriverInstallProgress[] = [];
  const recordProgress = (step: DriverInstallProgress): void => {
    progress.push(step);
  };

  beforeEach(async () => {
    temporaryRoot = await mkdtemp(join(tmpdir(), 'levimixer-installer-test-'));
    downloads.length = 0;
    scripts.length = 0;
    opened.length = 0;
    progress.length = 0;
  });

  afterEach(async () => {
    await rm(temporaryRoot, { recursive: true, force: true });
  });

  function installer(overrides: Partial<VbCableInstallerDependencies> = {}): VbCableInstaller {
    return new VbCableInstaller({
      download: (url, onProgress) => {
        onProgress(0.5);
        downloads.push(url);
        return Promise.resolve(new Uint8Array([80, 75]));
      },
      runPowerShell: (script) => {
        scripts.push(script);
        return Promise.resolve(0);
      },
      openExternal: (url) => {
        opened.push(url);
        return Promise.resolve();
      },
      architecture: 'x64',
      temporaryRoot,
      ...overrides,
    });
  }

  it('downloads the official package and runs the 64-bit setup elevated', async () => {
    expect(await installer().install(recordProgress)).toBe('installed');

    expect(downloads).toEqual([vbCableDownloadUrl]);
    expect(scripts[0]).toContain('VBCABLE_Setup_x64.exe');
    expect(scripts[0]).toContain('-Verb RunAs -Wait');
  });

  it('reports download percentage before handing over to the setup', async () => {
    await installer().install(recordProgress);

    expect(progress).toEqual([
      { stage: 'downloading', percent: null },
      { stage: 'downloading', percent: 50 },
      { stage: 'runningSetup' },
    ]);
  });

  it('removes the downloaded files afterwards', async () => {
    await installer().install(recordProgress);

    expect(await readdir(temporaryRoot)).toEqual([]);
  });

  it('opens the website when the download fails', async () => {
    const outcome = await installer({ download: () => Promise.resolve(null) }).install(recordProgress);

    expect(outcome).toBe('openedWebsite');
    expect(opened).toEqual([vbCableWebsite]);
    expect(scripts).toEqual([]);
  });

  it('opens the website on architectures without a setup in the package', async () => {
    const outcome = await installer({ architecture: 'arm64' }).install(recordProgress);

    expect(outcome).toBe('openedWebsite');
    expect(downloads).toEqual([]);
  });

  it('fails when the setup was cancelled', async () => {
    await expect(installer({ runPowerShell: () => Promise.resolve(1) }).install(recordProgress)).rejects.toThrow('cancelada');
    expect(existsSync(temporaryRoot) && (await readdir(temporaryRoot)).length).toBe(0);
  });
});

describe('installScript', () => {
  it('quotes paths so apostrophes in the user folder cannot break the command', () => {
    const script = installScript("C:\\Users\\D'Artagnan\\pack.zip", 'C:\\tmp\\setup', 'VBCABLE_Setup_x64.exe');

    expect(script).toContain("-LiteralPath 'C:\\Users\\D''Artagnan\\pack.zip'");
  });
});
