import { spawn } from 'node:child_process';
import { rm } from 'node:fs/promises';
import { arch, platform } from 'node:os';
import { fileURLToPath } from 'node:url';

const appRoot = fileURLToPath(new URL('..', import.meta.url));
const engineProject = fileURLToPath(new URL('../../../engine/src/LeviMixer.Engine.Host/LeviMixer.Engine.Host.csproj', import.meta.url));
const engineOutput = fileURLToPath(new URL('../build/engine', import.meta.url));

const runtimeIdentifiers = {
  linux: 'linux-x64',
  win32: 'win-x64',
  darwin: arch() === 'arm64' ? 'osx-arm64' : 'osx-x64',
};

const builderTargets = {
  linux: '--linux',
  win32: '--win',
  darwin: '--mac',
};

const usesWindowsShell = platform() === 'win32';

function run(command, args) {
  return new Promise((resolve, reject) => {
    const child = spawn(command, args, { cwd: appRoot, stdio: 'inherit', shell: usesWindowsShell });
    child.on('exit', (code) => (code === 0 ? resolve() : reject(new Error(`${command} ${args.join(' ')} exited with ${code}`))));
  });
}

const runtimeIdentifier = runtimeIdentifiers[platform()];
if (runtimeIdentifier === undefined) {
  throw new Error(`Unsupported platform: ${platform()}`);
}

await rm(engineOutput, { recursive: true, force: true });
await run('dotnet', [
  'publish', engineProject,
  '--configuration', 'Release',
  '--runtime', runtimeIdentifier,
  '--self-contained', 'true',
  '--output', engineOutput,
  '-p:PublishSingleFile=true',
  '-p:DebugType=none',
]);
await run('npm', ['run', 'build']);
await run('npx', ['electron-builder', '--config', 'electron-builder.yml', builderTargets[platform()], '--publish', 'never']);
