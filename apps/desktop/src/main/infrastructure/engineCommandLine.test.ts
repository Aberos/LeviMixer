import { describe, expect, it } from 'vitest';
import { resolveEngineCommandLine, type EngineLocation } from './engineCommandLine.ts';

const development: EngineLocation = {
  isPackaged: false,
  resourcesPath: '/opt/levimixer/resources',
  appPath: '/repo/apps/desktop',
  platform: 'linux',
  overridePath: undefined,
};

describe('resolveEngineCommandLine', () => {
  it('runs the debug build through dotnet during development', () => {
    expect(resolveEngineCommandLine(development)).toEqual({
      command: 'dotnet',
      args: ['/repo/engine/src/LeviMixer.Engine.Host/bin/Debug/net10.0/levimixer-engine.dll', 'ipc'],
    });
  });

  it('runs the bundled native executable when packaged', () => {
    expect(resolveEngineCommandLine({ ...development, isPackaged: true, platform: 'win32' })).toEqual({
      command: '/opt/levimixer/resources/engine/levimixer-engine.exe',
      args: ['ipc'],
    });
  });

  it('honours an explicit override', () => {
    expect(resolveEngineCommandLine({ ...development, overridePath: '/tmp/levimixer-engine' })).toEqual({ command: '/tmp/levimixer-engine', args: ['ipc'] });
  });
});
