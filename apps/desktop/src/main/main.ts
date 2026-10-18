import { app } from 'electron';
import { DesktopComposition } from './composition.ts';

let isQuitting = false;

if (!app.requestSingleInstanceLock()) {
  app.quit();
} else {
  app.whenReady().then(() => {
    const composition = new DesktopComposition();

    app.on('second-instance', () => composition.showWindow());
    app.on('window-all-closed', () => app.quit());
    app.on('before-quit', (event) => {
      if (isQuitting) {
        return;
      }

      event.preventDefault();
      isQuitting = true;
      void composition.shutdown().finally(() => app.quit());
    });

    return composition.start();
  }).catch((error: unknown) => {
    process.stderr.write(`[desktop] ${error instanceof Error ? error.stack : String(error)}\n`);
    app.exit(1);
  });
}
