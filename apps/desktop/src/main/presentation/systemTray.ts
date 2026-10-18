import { Menu, Tray, app, nativeImage, type BrowserWindow, type Event } from 'electron';
import { trayMenuTemplate } from './trayMenu.ts';

export class SystemTray {
  readonly #tray: Tray;
  readonly #window: BrowserWindow;
  readonly #quit: () => void;
  #isQuitting = false;

  constructor(iconPath: string, window: BrowserWindow, quit: () => void) {
    this.#window = window;
    this.#quit = quit;
    this.#tray = new Tray(nativeImage.createFromPath(iconPath));
    this.#tray.setToolTip('LeviMixer');
    this.#tray.on('click', () => this.toggleWindow());
    window.on('minimize', () => window.hide());
    window.on('close', (event) => this.#hideInsteadOfClosing(event));
    app.on('before-quit', () => {
      this.#isQuitting = true;
    });
    window.on('show', () => this.#refreshMenu());
    window.on('hide', () => this.#refreshMenu());
    this.#refreshMenu();
  }

  showWindow(): void {
    if (this.#window.isMinimized()) {
      this.#window.restore();
    }

    this.#window.show();
    this.#window.focus();
  }

  toggleWindow(): void {
    if (this.#window.isVisible()) {
      this.#window.hide();
      return;
    }

    this.showWindow();
  }

  destroy(): void {
    this.#tray.destroy();
  }

  #hideInsteadOfClosing(event: Event): void {
    if (this.#isQuitting) {
      return;
    }

    event.preventDefault();
    this.#window.hide();
  }

  #refreshMenu(): void {
    const actions = { toggleWindow: () => this.toggleWindow(), quit: this.#quit };
    this.#tray.setContextMenu(Menu.buildFromTemplate(trayMenuTemplate(this.#window.isVisible(), actions)));
  }
}
