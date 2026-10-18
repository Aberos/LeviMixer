import type { ChannelId, ChannelSource, DeviceId, GainDecibels } from './values.ts';

export interface AudioEndpoint {
  readonly id: DeviceId;
  readonly name: string;
  readonly channelCount: number;
  readonly isDefault: boolean;
}

export interface DeviceList {
  readonly inputs: readonly AudioEndpoint[];
  readonly outputs: readonly AudioEndpoint[];
}

export interface ConnectionState {
  readonly inputDeviceId: DeviceId;
  readonly outputDeviceId: DeviceId;
  readonly sampleRate: number;
  readonly bufferFrames: number;
}

export interface ChannelState {
  readonly id: ChannelId;
  readonly name: string;
  readonly source: ChannelSource;
  readonly inputIndex: number | null;
  readonly gainDecibels: GainDecibels;
  readonly pan: number;
  readonly muted: boolean;
  readonly soloed: boolean;
}

export interface MixerState {
  readonly running: boolean;
  readonly connection: ConnectionState | null;
  readonly channels: readonly ChannelState[];
  readonly masterGainDecibels: GainDecibels;
  readonly systemAudioError: string | null;
}

export interface LevelReading {
  readonly peak: number;
  readonly rms: number;
}

export interface ChannelMeter extends LevelReading {
  readonly channelId: ChannelId;
}

export interface Meters {
  readonly channels: readonly ChannelMeter[];
  readonly masterLeft: LevelReading;
  readonly masterRight: LevelReading;
}

export type EngineErrorCode =
  | 'unknownCommand'
  | 'invalidMessage'
  | 'unknownChannel'
  | 'invalidChannelName'
  | 'deviceNotFound'
  | 'deviceUnavailable';

export interface EngineError {
  readonly code: EngineErrorCode;
  readonly message: string;
}

export interface EngineReadiness {
  readonly protocolVersion: number;
  readonly engineVersion: string;
}

export type EngineMessage =
  | { readonly type: 'ready'; readonly requestId: string | null; readonly payload: EngineReadiness }
  | { readonly type: 'ack'; readonly requestId: string | null; readonly payload: null }
  | { readonly type: 'error'; readonly requestId: string | null; readonly payload: EngineError }
  | { readonly type: 'devices'; readonly requestId: string | null; readonly payload: DeviceList }
  | { readonly type: 'mixerState'; readonly requestId: string | null; readonly payload: MixerState }
  | { readonly type: 'meters'; readonly requestId: string | null; readonly payload: Meters };

export type EngineMessageType = EngineMessage['type'];

export type EngineReply = Extract<EngineMessage, { readonly type: 'ack' | 'error' | 'devices' | 'mixerState' }>;
