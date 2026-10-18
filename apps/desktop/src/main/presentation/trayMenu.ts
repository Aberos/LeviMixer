import type { MenuItemConstructorOptions } from 'electron';

export interface TrayActions {
  readonly toggleWindow: () => void;
  readonly quit: () => void;
}

export function trayMenuTemplate(isWindowVisible: boolean, actions: TrayActions): MenuItemConstructorOptions[] {
  return [
    { label: isWindowVisible ? 'Ocultar Mixer' : 'Mostrar Mixer', click: actions.toggleWindow },
    { type: 'separator' },
    { label: 'Sair', click: actions.quit },
  ];
}
