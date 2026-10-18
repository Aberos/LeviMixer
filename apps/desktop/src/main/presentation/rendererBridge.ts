import type { IpcMain, WebContents } from 'electron';
import type { EngineReply } from '@mixer/protocol';
import type { BridgeChannel, BridgeEvent, EngineStatus } from '../../shared/bridge.ts';
import type { EngineSupervisor } from '../application/engineSupervisor.ts';
import type { SessionKeeper } from '../application/sessionKeeper.ts';
import { isRendererCommand } from './commandGuard.ts';

export interface RendererBridgeDependencies {
  readonly ipc: IpcMain;
  readonly supervisor: EngineSupervisor;
  readonly sessionKeeper: SessionKeeper;
  readonly restartEngine: () => Promise<void>;
  readonly target: () => WebContents | null;
}

const channels = {
  request: 'mixer:request',
  status: 'mixer:status',
  restart: 'mixer:restart',
  event: 'mixer:event',
} as const satisfies Record<string, BridgeChannel>;

export function registerRendererBridge(dependencies: RendererBridgeDependencies): void {
  const { ipc, supervisor, sessionKeeper, restartEngine, target } = dependencies;
  const publish = (event: BridgeEvent): void => target()?.send(channels.event, event);

  ipc.handle(channels.request, (_event, command: unknown): Promise<EngineReply> => {
    if (!isRendererCommand(command)) {
      return Promise.reject(new Error('Unsupported command.'));
    }

    sessionKeeper.record(command);
    return supervisor.request(command);
  });
  ipc.handle(channels.status, (): EngineStatus => supervisor.status);
  ipc.handle(channels.restart, () => restartEngine());

  supervisor.onMessage((message) => publish({ kind: 'engine', message }));
  supervisor.onStatus((status) => publish({ kind: 'status', status }));
}
