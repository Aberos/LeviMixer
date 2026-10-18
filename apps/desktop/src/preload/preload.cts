import electron = require('electron');
import type { IpcRendererEvent } from 'electron';
import type { BridgeChannel, BridgeEvent, MixerBridge } from '../shared/bridge.ts';

const { contextBridge, ipcRenderer } = electron;

const channel = <Name extends BridgeChannel>(name: Name): Name => name;

const bridge: MixerBridge = {
  request: (command) => ipcRenderer.invoke(channel('mixer:request'), command),
  engineStatus: () => ipcRenderer.invoke(channel('mixer:status')),
  restartEngine: () => ipcRenderer.invoke(channel('mixer:restart')),
  onEvent: (listener) => {
    const handler = (_event: IpcRendererEvent, bridgeEvent: BridgeEvent): void => listener(bridgeEvent);
    ipcRenderer.on(channel('mixer:event'), handler);
    return () => {
      ipcRenderer.removeListener(channel('mixer:event'), handler);
    };
  },
};

contextBridge.exposeInMainWorld('mixer', bridge);
