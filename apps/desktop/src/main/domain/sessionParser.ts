import type { ChannelSettings } from '@levimixer/protocol';
import { sessionSchemaVersion, type Session } from './session.ts';

type UnknownRecord = Record<string, unknown>;

export function parseSession(value: unknown): Session | null {
  if (!isRecord(value) || value['schemaVersion'] !== sessionSchemaVersion) {
    return null;
  }

  const channels = value['channels'];
  if (!Array.isArray(channels) || !channels.every(isChannelSettings)) {
    return null;
  }

  if (!isOptionalString(value['inputDeviceId']) || !isOptionalString(value['outputDeviceId'])) {
    return null;
  }

  if (!isPositiveInteger(value['sampleRate']) || !isPositiveInteger(value['bufferFrames']) || !isGain(value['masterGainDecibels'])) {
    return null;
  }

  return {
    schemaVersion: sessionSchemaVersion,
    inputDeviceId: value['inputDeviceId'],
    outputDeviceId: value['outputDeviceId'],
    sampleRate: value['sampleRate'],
    bufferFrames: value['bufferFrames'],
    channels,
    masterGainDecibels: value['masterGainDecibels'],
  };
}

function isChannelSettings(value: unknown): value is ChannelSettings {
  return (
    isRecord(value) &&
    typeof value['id'] === 'string' &&
    typeof value['name'] === 'string' &&
    isGain(value['gainDecibels']) &&
    typeof value['pan'] === 'number' &&
    typeof value['muted'] === 'boolean' &&
    typeof value['soloed'] === 'boolean'
  );
}

function isRecord(value: unknown): value is UnknownRecord {
  return typeof value === 'object' && value !== null && !Array.isArray(value);
}

function isOptionalString(value: unknown): value is string | null {
  return value === null || typeof value === 'string';
}

function isPositiveInteger(value: unknown): value is number {
  return typeof value === 'number' && Number.isInteger(value) && value > 0;
}

function isGain(value: unknown): value is number | null {
  return value === null || (typeof value === 'number' && Number.isFinite(value));
}
