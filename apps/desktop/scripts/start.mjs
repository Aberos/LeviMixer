import { spawn } from 'node:child_process';
import { fileURLToPath } from 'node:url';

const appRoot = fileURLToPath(new URL('..', import.meta.url));
const { ELECTRON_RUN_AS_NODE: _inheritedFromEditor, ...environment } = process.env;

const electron = spawn('npx', ['electron', '.', ...process.argv.slice(2)], { cwd: appRoot, stdio: 'inherit', env: environment });
electron.on('exit', (code) => process.exit(code ?? 0));
