import { join, resolve } from 'node:path';

export interface EngineCommandLine {
  readonly command: string;
  readonly args: readonly string[];
}

export interface EngineLocation {
  readonly isPackaged: boolean;
  readonly resourcesPath: string;
  readonly appPath: string;
  readonly platform: NodeJS.Platform;
  readonly overridePath: string | undefined;
}

const ipcArgument = 'ipc';

export function resolveEngineCommandLine(location: EngineLocation): EngineCommandLine {
  if (location.overridePath !== undefined) {
    return commandFor(location.overridePath);
  }

  if (location.isPackaged) {
    const executable = location.platform === 'win32' ? 'mixer-engine.exe' : 'mixer-engine';
    return commandFor(join(location.resourcesPath, 'engine', executable));
  }

  return commandFor(resolve(location.appPath, '..', '..', 'engine', 'src', 'Mixer.Engine.Host', 'bin', 'Debug', 'net10.0', 'mixer-engine.dll'));
}

function commandFor(path: string): EngineCommandLine {
  return path.endsWith('.dll') ? { command: 'dotnet', args: [path, ipcArgument] } : { command: path, args: [ipcArgument] };
}
