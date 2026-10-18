import { spawn } from 'node:child_process';
import { fileURLToPath } from 'node:url';
import { createServer } from 'vite';

const appRoot = fileURLToPath(new URL('..', import.meta.url));

function run(command, args) {
  return new Promise((resolve, reject) => {
    const child = spawn(command, args, { cwd: appRoot, stdio: 'inherit' });
    child.on('exit', (code) => (code === 0 ? resolve() : reject(new Error(`${command} ${args.join(' ')} exited with ${code}`))));
  });
}

await run('npx', ['tsc', '-p', 'tsconfig.main.json']);
await run('npx', ['tsc', '-p', 'tsconfig.preload.json']);

const server = await createServer({ configFile: 'vite.config.ts', root: 'src/renderer' });
await server.listen();
const rendererUrl = server.resolvedUrls?.local[0] ?? 'http://localhost:5173/';

const { ELECTRON_RUN_AS_NODE: _inheritedFromEditor, ...environment } = process.env;
const electron = spawn('npx', ['electron', '.'], {
  cwd: appRoot,
  stdio: 'inherit',
  env: { ...environment, LEVIMIXER_RENDERER_URL: rendererUrl },
});

electron.on('exit', async (code) => {
  await server.close();
  process.exit(code ?? 0);
});
