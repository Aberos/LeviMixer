import { app, ipcMain, type BrowserWindow } from 'electron';
import { join } from 'node:path';
import { fileURLToPath } from 'node:url';
import { EngineSupervisor } from './application/engineSupervisor.ts';
import { RestoreSession } from './application/restoreSession.ts';
import { SessionAutosave } from './application/sessionAutosave.ts';
import { SessionKeeper } from './application/sessionKeeper.ts';
import { SessionTracker } from './domain/sessionTracker.ts';
import { resolveEngineCommandLine } from './infrastructure/engineCommandLine.ts';
import { JsonSessionRepository } from './infrastructure/jsonSessionRepository.ts';
import { ProcessEngineLauncher } from './infrastructure/processEngineLauncher.ts';
import { createMainWindow } from './presentation/mainWindow.ts';
import { SystemTray } from './presentation/systemTray.ts';
import { registerRendererBridge } from './presentation/rendererBridge.ts';

const readyTimeoutMilliseconds = 15_000;
const shutdownTimeoutMilliseconds = 3_000;
const autosaveDelayMilliseconds = 500;

export class DesktopComposition {
  readonly #supervisor: EngineSupervisor;
  readonly #sessionKeeper: SessionKeeper;
  readonly #restoreSession: RestoreSession;
  #window: BrowserWindow | null = null;
  #tray: SystemTray | null = null;

  constructor() {
    const commandLine = resolveEngineCommandLine({
      isPackaged: app.isPackaged,
      resourcesPath: process.resourcesPath,
      appPath: app.getAppPath(),
      platform: process.platform,
      overridePath: process.env['MIXER_ENGINE_PATH'],
    });
    const repository = new JsonSessionRepository(join(app.getPath('userData'), 'session.json'));
    this.#supervisor = new EngineSupervisor(new ProcessEngineLauncher(commandLine, shutdownTimeoutMilliseconds), readyTimeoutMilliseconds);
    this.#sessionKeeper = new SessionKeeper(new SessionTracker(), new SessionAutosave(repository, autosaveDelayMilliseconds));
    this.#restoreSession = new RestoreSession(this.#supervisor, repository);
    this.#supervisor.onMessage((message) => this.#sessionKeeper.observe(message));
  }

  async start(): Promise<void> {
    registerRendererBridge({
      ipc: ipcMain,
      supervisor: this.#supervisor,
      sessionKeeper: this.#sessionKeeper,
      restartEngine: () => this.#startEngine(),
      target: () => this.#window?.webContents ?? null,
    });
    this.#window = createMainWindow({
      iconPath: resourcePath('icon.png'),
      preloadPath: fileURLToPath(new URL('../preload/preload.cjs', import.meta.url)),
      rendererUrl: process.env['MIXER_RENDERER_URL'],
      rendererFile: fileURLToPath(new URL('../renderer/index.html', import.meta.url)),
    });
    this.#window.on('closed', () => {
      this.#window = null;
    });
    this.#tray = new SystemTray(resourcePath('tray.png'), this.#window, () => app.quit());
    await this.#startEngine();
  }

  showWindow(): void {
    this.#tray?.showWindow();
  }

  async shutdown(): Promise<void> {
    this.#tray?.destroy();
    this.#tray = null;
    await this.#sessionKeeper.flush();
    await this.#supervisor.stop();
  }

  async #startEngine(): Promise<void> {
    try {
      await this.#supervisor.stop();
      await this.#supervisor.start();
      await this.#restoreSession.execute();
    } catch (error) {
      process.stderr.write(`[desktop] ${error instanceof Error ? error.message : String(error)}\n`);
    }
  }
}

function resourcePath(fileName: string): string {
  return join(app.getAppPath(), 'resources', fileName);
}
