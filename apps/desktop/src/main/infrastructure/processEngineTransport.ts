import { spawn, type ChildProcessWithoutNullStreams } from 'node:child_process';
import type { EngineCommandLine } from './engineCommandLine.ts';
import type { EngineTransport } from './engineTransport.ts';
import { LineSplitter } from './lineSplitter.ts';

export class ProcessEngineTransport implements EngineTransport {
  readonly #process: ChildProcessWithoutNullStreams;
  readonly #lines = new LineSplitter();
  readonly #lineListeners: Array<(line: string) => void> = [];
  readonly #exitListeners: Array<(reason: string) => void> = [];

  constructor(commandLine: EngineCommandLine) {
    this.#process = spawn(commandLine.command, [...commandLine.args], { stdio: ['pipe', 'pipe', 'pipe'] });
    this.#process.stdout.setEncoding('utf8');
    this.#process.stdout.on('data', (chunk: string) => this.#lines.push(chunk).forEach((line) => this.#lineListeners.forEach((listener) => listener(line))));
    this.#process.stderr.setEncoding('utf8');
    this.#process.stderr.on('data', (chunk: string) => process.stderr.write(`[engine] ${chunk}`));
    this.#process.on('error', (error) => this.#exit(error.message));
    this.#process.on('exit', (code, signal) => this.#exit(signal === null ? `exit code ${code ?? 'unknown'}` : `signal ${signal}`));
  }

  send(line: string): void {
    if (this.#process.stdin.writable) {
      this.#process.stdin.write(`${line}\n`);
    }
  }

  onLine(listener: (line: string) => void): void {
    this.#lineListeners.push(listener);
  }

  onExit(listener: (reason: string) => void): void {
    this.#exitListeners.push(listener);
  }

  kill(): void {
    this.#process.kill('SIGTERM');
  }

  #exit(reason: string): void {
    const listeners = this.#exitListeners.splice(0);
    listeners.forEach((listener) => listener(reason));
  }
}
