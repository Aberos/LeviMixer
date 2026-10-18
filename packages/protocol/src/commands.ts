import type { ChannelId, DeviceId, GainDecibels } from './values.ts';

export interface DeviceSelection {
  readonly inputDeviceId?: DeviceId | null;
  readonly outputDeviceId?: DeviceId | null;
  readonly sampleRate?: number;
  readonly bufferFrames?: number;
}

export interface ChannelSettings {
  readonly id: ChannelId;
  readonly name: string;
  readonly gainDecibels: GainDecibels;
  readonly pan: number;
  readonly muted: boolean;
  readonly soloed: boolean;
}

export interface SessionPayload extends DeviceSelection {
  readonly channels: readonly ChannelSettings[];
  readonly masterGainDecibels: GainDecibels;
}

export type EngineCommand =
  | { readonly type: 'listDevices' }
  | { readonly type: 'getState' }
  | { readonly type: 'startMixing'; readonly payload: DeviceSelection }
  | { readonly type: 'stopMixing' }
  | { readonly type: 'applySession'; readonly payload: SessionPayload }
  | { readonly type: 'setChannelGain'; readonly payload: { readonly channelId: ChannelId; readonly gainDecibels: GainDecibels } }
  | { readonly type: 'setChannelMute'; readonly payload: { readonly channelId: ChannelId; readonly value: boolean } }
  | { readonly type: 'setChannelSolo'; readonly payload: { readonly channelId: ChannelId; readonly value: boolean } }
  | { readonly type: 'setChannelPan'; readonly payload: { readonly channelId: ChannelId; readonly pan: number } }
  | { readonly type: 'renameChannel'; readonly payload: { readonly channelId: ChannelId; readonly name: string } }
  | { readonly type: 'setMasterGain'; readonly payload: { readonly gainDecibels: GainDecibels } }
  | { readonly type: 'shutdown' };

export type EngineCommandType = EngineCommand['type'];

export type CommandEnvelope = EngineCommand & { readonly requestId?: string };
