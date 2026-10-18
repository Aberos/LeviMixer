import type { EngineCommand, EngineMessage, EngineReply } from '@mixer/protocol';
import type { MessageListener } from '../application/engineGateway.ts';
import type { EngineConnection } from '../application/engineLauncher.ts';
import type { EngineTransport } from './engineTransport.ts';

interface PendingRequest {
  readonly resolve: (reply: EngineReply) => void;
  readonly reject: (error: Error) => void;
}

const replyTypes: ReadonlySet<EngineMessage['type']> = new Set(['ack', 'error', 'devices', 'mixerState']);

export class EngineClient implements EngineConnection {
  readonly #transport: EngineTransport;
  readonly #shutdownTimeoutMilliseconds: number;
  readonly #pending = new Map<string, PendingRequest>();
  readonly #messageListeners = new Set<MessageListener>();
  readonly #exitListeners = new Set<(reason: string) => void>();
  #nextRequestId = 1;
  #exited: Promise<void>;

  constructor(transport: EngineTransport, shutdownTimeoutMilliseconds: number) {
    this.#transport = transport;
    this.#shutdownTimeoutMilliseconds = shutdownTimeoutMilliseconds;
    this.#exited = new Promise((resolve) => this.#exitListeners.add(() => resolve()));
    transport.onLine((line) => this.#receive(line));
    transport.onExit((reason) => this.#handleExit(reason));
  }

  request(command: EngineCommand): Promise<EngineReply> {
    const requestId = `r${this.#nextRequestId++}`;
    return new Promise((resolve, reject) => {
      this.#pending.set(requestId, { resolve, reject });
      this.#transport.send(JSON.stringify({ ...command, requestId }));
    });
  }

  onMessage(listener: MessageListener): () => void {
    this.#messageListeners.add(listener);
    return () => this.#messageListeners.delete(listener);
  }

  onExit(listener: (reason: string) => void): () => void {
    this.#exitListeners.add(listener);
    return () => this.#exitListeners.delete(listener);
  }

  async shutdown(): Promise<void> {
    const timeout = new Promise<'timeout'>((resolve) => setTimeout(() => resolve('timeout'), this.#shutdownTimeoutMilliseconds));
    this.#transport.send(JSON.stringify({ type: 'shutdown' }));
    if ((await Promise.race([this.#exited.then(() => 'exited' as const), timeout])) === 'timeout') {
      this.#transport.kill();
      await this.#exited;
    }
  }

  #receive(line: string): void {
    const message = parseMessage(line);
    if (message === null) {
      return;
    }

    this.#resolvePending(message);
    this.#messageListeners.forEach((listener) => listener(message));
  }

  #resolvePending(message: EngineMessage): void {
    if (message.requestId === null || !replyTypes.has(message.type)) {
      return;
    }

    const pending = this.#pending.get(message.requestId);
    this.#pending.delete(message.requestId);
    pending?.resolve(message as EngineReply);
  }

  #handleExit(reason: string): void {
    const error = new Error(`The audio engine exited: ${reason}`);
    this.#pending.forEach((pending) => pending.reject(error));
    this.#pending.clear();
    this.#exitListeners.forEach((listener) => listener(reason));
  }
}

function parseMessage(line: string): EngineMessage | null {
  try {
    const value: unknown = JSON.parse(line);
    return typeof value === 'object' && value !== null && 'type' in value ? (value as EngineMessage) : null;
  } catch {
    return null;
  }
}
