import { readdirSync, readFileSync } from 'node:fs';
import { basename, join } from 'node:path';
import { describe, expect, it } from 'vitest';
import type { CommandEnvelope, EngineCommandType, EngineMessage, EngineMessageType } from '../src/index.ts';

const fixturesRoot = join(import.meta.dirname, '..', 'fixtures');

const commandTypes: Record<EngineCommandType, true> = {
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
  shutdown: true,
};

const messageTypes: Record<EngineMessageType, true> = {
  ready: true,
  ack: true,
  error: true,
  devices: true,
  mixerState: true,
  meters: true,
};

function readFixtures<T>(direction: 'inbound' | 'outbound'): Array<[string, T]> {
  const directory = join(fixturesRoot, direction);
  return readdirSync(directory).map((file) => [basename(file, '.json'), JSON.parse(readFileSync(join(directory, file), 'utf8')) as T]);
}

describe('protocol fixtures', () => {
  it.each(readFixtures<CommandEnvelope>('inbound'))('inbound %s uses a known command type', (_, command) => {
    expect(commandTypes[command.type]).toBe(true);
  });

  it.each(readFixtures<EngineMessage>('outbound'))('outbound %s uses a known message type', (_, message) => {
    expect(messageTypes[message.type]).toBe(true);
  });

  it('covers every command type with a fixture', () => {
    const covered = new Set(readFixtures<CommandEnvelope>('inbound').map(([, command]) => command.type));
    expect([...covered].sort()).toEqual(Object.keys(commandTypes).sort());
  });

  it('covers every message type with a fixture', () => {
    const covered = new Set(readFixtures<EngineMessage>('outbound').map(([, message]) => message.type));
    expect([...covered].sort()).toEqual(Object.keys(messageTypes).sort());
  });

  it('represents silent gain as null', () => {
    const silent = readFixtures<CommandEnvelope>('inbound').find(([name]) => name === 'setChannelGainSilent')?.[1];
    expect(silent).toMatchObject({ payload: { gainDecibels: null } });
  });
});
