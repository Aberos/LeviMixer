import type { ChannelSettings, EngineCommand, GainDecibels, MixerState, SessionPayload } from '@mixer/protocol';

export const sessionSchemaVersion = 1;

export interface Session {
  readonly schemaVersion: typeof sessionSchemaVersion;
  readonly inputDeviceId: string | null;
  readonly outputDeviceId: string | null;
  readonly sampleRate: number;
  readonly bufferFrames: number;
  readonly channels: readonly ChannelSettings[];
  readonly masterGainDecibels: GainDecibels;
}

export function sessionFromState(state: MixerState): Session | null {
  if (!state.running || state.connection === null) {
    return null;
  }

  return {
    schemaVersion: sessionSchemaVersion,
    inputDeviceId: state.connection.inputDeviceId,
    outputDeviceId: state.connection.outputDeviceId,
    sampleRate: state.connection.sampleRate,
    bufferFrames: state.connection.bufferFrames,
    channels: state.channels.map(({ id, name, gainDecibels, pan, muted, soloed }) => ({ id, name, gainDecibels, pan, muted, soloed })),
    masterGainDecibels: state.masterGainDecibels,
  };
}

export function applyCommand(session: Session, command: EngineCommand): Session {
  switch (command.type) {
    case 'setChannelGain':
      return updateChannel(session, command.payload.channelId, { gainDecibels: command.payload.gainDecibels });
    case 'setChannelMute':
      return updateChannel(session, command.payload.channelId, { muted: command.payload.value });
    case 'setChannelSolo':
      return updateChannel(session, command.payload.channelId, { soloed: command.payload.value });
    case 'setChannelPan':
      return updateChannel(session, command.payload.channelId, { pan: command.payload.pan });
    case 'renameChannel':
      return updateChannel(session, command.payload.channelId, { name: command.payload.name });
    case 'setMasterGain':
      return { ...session, masterGainDecibels: command.payload.gainDecibels };
    default:
      return session;
  }
}

export function toSessionPayload(session: Session): SessionPayload {
  return {
    inputDeviceId: session.inputDeviceId,
    outputDeviceId: session.outputDeviceId,
    sampleRate: session.sampleRate,
    bufferFrames: session.bufferFrames,
    channels: session.channels,
    masterGainDecibels: session.masterGainDecibels,
  };
}

export function withDefaultDevices(session: Session): Session {
  return { ...session, inputDeviceId: null, outputDeviceId: null };
}

function updateChannel(session: Session, channelId: string, change: Partial<ChannelSettings>): Session {
  if (!session.channels.some((channel) => channel.id === channelId)) {
    return session;
  }

  return {
    ...session,
    channels: session.channels.map((channel) => (channel.id === channelId ? { ...channel, ...change } : channel)),
  };
}
