import { describe, expect, it } from 'vitest';
import { resolveEngineCommandLine, type EngineLocation } from './engineCommandLine.ts';

const development: EngineLocation = {
  isPackaged: false,
  resourcesPath: '/opt/mixer/resources',
  appPath: '/repo/apps/desktop',
  platform: 'linux',
  overridePath: undefined,
};

describe('resolveEngineCommandLine', () => {
  it('runs the debug build through dotnet during development', () => {
    expect(resolveEngineCommandLine(development)).toEqual({
      command: 'dotnet',
      args: ['/repo/engine/src/Mixer.Engine.Host/bin/Debug/net10.0/mixer-engine.dll', 'ipc'],
    });
  });

  it('runs the bundled native executable when packaged', () => {
    expect(resolveEngineCommandLine({ ...development, isPackaged: true, platform: 'win32' })).toEqual({
      command: '/opt/mixer/resources/engine/mixer-engine.exe',
      args: ['ipc'],
    });
  });

  it('honours an explicit override', () => {
    expect(resolveEngineCommandLine({ ...development, overridePath: '/tmp/mixer-engine' })).toEqual({ command: '/tmp/mixer-engine', args: ['ipc'] });
  });
});
