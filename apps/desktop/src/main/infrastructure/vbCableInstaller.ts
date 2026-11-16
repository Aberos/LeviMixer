import { mkdtemp, rm, writeFile } from 'node:fs/promises';
import { join } from 'node:path';
import type { DriverInstallOutcome } from '../../shared/bridge.ts';
import type { DriverInstallListener, SystemAudioDriverInstaller } from '../application/systemAudioDriverInstaller.ts';
import type { DownloadProgress } from './httpDownload.ts';

export const vbCableDownloadUrl = 'https://download.vb-audio.com/Download_CABLE/VBCABLE_Driver_Pack46.zip';
export const vbCableWebsite = 'https://vb-audio.com/Cable/';

const setupFiles: Partial<Record<NodeJS.Architecture, string>> = {
  x64: 'VBCABLE_Setup_x64.exe',
  ia32: 'VBCABLE_Setup.exe',
};

export interface VbCableInstallerDependencies {
  readonly download: (url: string, onProgress: DownloadProgress) => Promise<Uint8Array | null>;
  readonly runPowerShell: (script: string) => Promise<number>;
  readonly openExternal: (url: string) => Promise<void>;
  readonly architecture: NodeJS.Architecture;
  readonly temporaryRoot: string;
}

export class VbCableInstaller implements SystemAudioDriverInstaller {
  readonly #dependencies: VbCableInstallerDependencies;

  constructor(dependencies: VbCableInstallerDependencies) {
    this.#dependencies = dependencies;
  }

  async install(onProgress: DriverInstallListener): Promise<DriverInstallOutcome> {
    const setupFile = setupFiles[this.#dependencies.architecture];
    const archive = setupFile === undefined ? null : await this.#download(onProgress);
    if (setupFile === undefined || archive === null) {
      await this.#dependencies.openExternal(vbCableWebsite);
      return 'openedWebsite';
    }

    const workDirectory = await mkdtemp(join(this.#dependencies.temporaryRoot, 'levimixer-vbcable-'));
    try {
      const archivePath = join(workDirectory, 'VBCABLE_Driver_Pack.zip');
      await writeFile(archivePath, archive);
      onProgress({ stage: 'runningSetup' });
      const exitCode = await this.#dependencies.runPowerShell(installScript(archivePath, join(workDirectory, 'setup'), setupFile));
      if (exitCode !== 0) {
        throw new Error('A instalação do VB-Cable foi cancelada ou não terminou.');
      }

      return 'installed';
    } finally {
      await rm(workDirectory, { recursive: true, force: true });
    }
  }

  #download(onProgress: DriverInstallListener): Promise<Uint8Array | null> {
    onProgress({ stage: 'downloading', percent: null });
    return this.#dependencies.download(vbCableDownloadUrl, (fraction) =>
      onProgress({ stage: 'downloading', percent: fraction === null ? null : Math.round(fraction * 100) }),
    );
  }
}

export function installScript(archivePath: string, extractDirectory: string, setupFile: string): string {
  return [
    "$ErrorActionPreference = 'Stop'",
    `Expand-Archive -LiteralPath ${quoted(archivePath)} -DestinationPath ${quoted(extractDirectory)} -Force`,
    `Start-Process -FilePath ${quoted(join(extractDirectory, setupFile))} -Verb RunAs -Wait`,
  ].join('; ');
}

function quoted(value: string): string {
  return `'${value.replaceAll("'", "''")}'`;
}
