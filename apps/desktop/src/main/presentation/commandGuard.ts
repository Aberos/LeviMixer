import type { EngineCommand, EngineCommandType } from '@mixer/protocol';

const rendererCommands: Record<Exclude<EngineCommandType, 'shutdown'>, true> = {
  listDevices: true,
  getState: true,
  startMixing: true,
  stopMixing: true,
  applySession: true,
  setChannelGain: true,
  setChannelMute: true,
  setChannelSolo: true,
  setChannelPan: true,
  renameChannel: true,
  setMasterGain: true,
};

export function isRendererCommand(value: unknown): value is EngineCommand {
  return (
    typeof value === 'object' &&
    value !== null &&
    'type' in value &&
    typeof value.type === 'string' &&
    Object.hasOwn(rendererCommands, value.type)
  );
}
