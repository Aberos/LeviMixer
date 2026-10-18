import type { EngineConnection, EngineLauncher } from '../application/engineLauncher.ts';
import type { EngineCommandLine } from './engineCommandLine.ts';
import { EngineClient } from './engineClient.ts';
import { ProcessEngineTransport } from './processEngineTransport.ts';

export class ProcessEngineLauncher implements EngineLauncher {
  readonly #commandLine: EngineCommandLine;
  readonly #shutdownTimeoutMilliseconds: number;

  constructor(commandLine: EngineCommandLine, shutdownTimeoutMilliseconds: number) {
    this.#commandLine = commandLine;
    this.#shutdownTimeoutMilliseconds = shutdownTimeoutMilliseconds;
  }

  launch(): EngineConnection {
    return new EngineClient(new ProcessEngineTransport(this.#commandLine), this.#shutdownTimeoutMilliseconds);
  }
}
