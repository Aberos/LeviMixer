import { describe, expect, it, vi } from 'vitest';
import { trayMenuTemplate } from './trayMenu.ts';

const noActions = { toggleWindow: () => undefined, quit: () => undefined };

describe('trayMenuTemplate', () => {
  it('offers to hide a visible window', () => {
    expect(trayMenuTemplate(true, noActions)[0]?.label).toBe('Ocultar Mixer');
  });

  it('offers to show a hidden window', () => {
    expect(trayMenuTemplate(false, noActions)[0]?.label).toBe('Mostrar Mixer');
  });

  it('ends with a quit entry wired to the quit action', () => {
    const quit = vi.fn();
    const template = trayMenuTemplate(true, { ...noActions, quit });
    const quitItem = template.at(-1);

    quitItem?.click?.({} as never, undefined, {} as never);

    expect(quitItem?.label).toBe('Sair');
    expect(quit).toHaveBeenCalledOnce();
  });
});
