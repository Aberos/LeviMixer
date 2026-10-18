import type { EngineTransport } from '../infrastructure/engineTransport.ts';

export class FakeEngineTransport implements EngineTransport {
  readonly sent: Array<Record<string, unknown>> = [];
  readonly #lineListeners: Array<(line: string) => void> = [];
  readonly #exitListeners: Array<(reason: string) => void> = [];
  killed = false;
  exitOnShutdown = true;

  send(line: string): void {
    const message = JSON.parse(line) as Record<string, unknown>;
    this.sent.push(message);
    if (message['type'] === 'shutdown' && this.exitOnShutdown) {
      this.exit('exit code 0');
    }
  }

  onLine(listener: (line: string) => void): void {
    this.#lineListeners.push(listener);
  }

  onExit(listener: (reason: string) => void): void {
    this.#exitListeners.push(listener);
  }

  kill(): void {
    this.killed = true;
    this.exit('signal SIGTERM');
  }

  emit(message: object): void {
    this.#lineListeners.forEach((listener) => listener(JSON.stringify(message)));
  }

  exit(reason: string): void {
    this.#exitListeners.splice(0).forEach((listener) => listener(reason));
  }
}
