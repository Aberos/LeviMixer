import type { EngineCommand, EngineReply } from '@mixer/protocol';
import type { EngineStatus } from '../../shared/bridge.ts';
import type { EngineGateway, MessageListener } from './engineGateway.ts';
import type { EngineConnection, EngineLauncher } from './engineLauncher.ts';

type StatusListener = (status: EngineStatus) => void;

export class EngineSupervisor implements EngineGateway {
  readonly #launcher: EngineLauncher;
  readonly #readyTimeoutMilliseconds: number;
  readonly #messageListeners = new Set<MessageListener>();
  readonly #statusListeners = new Set<StatusListener>();
  #connection: EngineConnection | null = null;
  #status: EngineStatus = { state: 'stopped', reason: 'Engine not started.' };

  constructor(launcher: EngineLauncher, readyTimeoutMilliseconds: number) {
    this.#launcher = launcher;
    this.#readyTimeoutMilliseconds = readyTimeoutMilliseconds;
  }

  get status(): EngineStatus {
    return this.#status;
  }

  async start(): Promise<void> {
    this.#setStatus({ state: 'starting' });
    const connection = this.#launcher.launch();
    this.#connection = connection;
    connection.onMessage((message) => this.#messageListeners.forEach((listener) => listener(message)));
    connection.onExit((reason) => this.#handleExit(connection, reason));
    const engineVersion = await this.#waitForReady(connection);
    this.#setStatus({ state: 'running', engineVersion });
  }

  async restart(): Promise<void> {
    await this.stop();
    await this.start();
  }

  async stop(): Promise<void> {
    const connection = this.#connection;
    this.#connection = null;
    if (connection !== null) {
      await connection.shutdown();
      this.#setStatus({ state: 'stopped', reason: 'Engine stopped.' });
    }
  }

  request(command: EngineCommand): Promise<EngineReply> {
    if (this.#connection === null || this.#status.state !== 'running') {
      return Promise.reject(new Error('The audio engine is not running.'));
    }

    return this.#connection.request(command);
  }

  onMessage(listener: MessageListener): () => void {
    this.#messageListeners.add(listener);
    return () => this.#messageListeners.delete(listener);
  }

  onStatus(listener: StatusListener): () => void {
    this.#statusListeners.add(listener);
    return () => this.#statusListeners.delete(listener);
  }

  #waitForReady(connection: EngineConnection): Promise<string> {
    return new Promise((resolve, reject) => {
      const timer = setTimeout(() => {
        unsubscribe();
        reject(new Error('The audio engine did not become ready in time.'));
      }, this.#readyTimeoutMilliseconds);
      const unsubscribe = connection.onMessage((message) => {
        if (message.type === 'ready') {
          clearTimeout(timer);
          unsubscribe();
          resolve(message.payload.engineVersion);
        }
      });
    });
  }

  #handleExit(connection: EngineConnection, reason: string): void {
    if (this.#connection === connection) {
      this.#connection = null;
      this.#setStatus({ state: 'stopped', reason });
    }
  }

  #setStatus(status: EngineStatus): void {
    this.#status = status;
    this.#statusListeners.forEach((listener) => listener(status));
  }
}
