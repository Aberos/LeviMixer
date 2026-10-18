import { BrowserWindow } from 'electron';

export interface WindowContent {
  readonly preloadPath: string;
  readonly rendererUrl: string | undefined;
  readonly rendererFile: string;
}

export function createMainWindow(content: WindowContent): BrowserWindow {
  const window = new BrowserWindow({
    width: 1100,
    height: 720,
    minWidth: 640,
    minHeight: 520,
    backgroundColor: '#15171a',
    title: 'Mixer',
    webPreferences: {
      preload: content.preloadPath,
      contextIsolation: true,
      sandbox: true,
      nodeIntegration: false,
    },
  });

  window.removeMenu();
  void (content.rendererUrl === undefined ? window.loadFile(content.rendererFile) : window.loadURL(content.rendererUrl));
  return window;
}
