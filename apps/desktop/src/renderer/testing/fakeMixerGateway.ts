import type { EngineCommand, EngineReply, MixerState } from '@mixer/protocol';
import type { BridgeEvent, EngineStatus } from '../../shared/bridge.ts';
import type { MixerGateway } from '../application/mixerGateway.ts';

export const mixerState: MixerState = {
  running: true,
  connection: { inputDeviceId: 'umc22-in', outputDeviceId: 'umc22-out', sampleRate: 48_000, bufferFrames: 128 },
  channels: [
    { id: 'in-1', name: 'Vocal', source: 'input', inputIndex: 0, gainDecibels: 0, pan: 0, muted: false, soloed: false },
    { id: 'pc', name: 'PC', source: 'system', inputIndex: null, gainDecibels: -6, pan: 0, muted: false, soloed: false },
  ],
  masterGainDecibels: 0,
  systemAudioError: null,
};

export class FakeMixerGateway implements MixerGateway {
  readonly requests: EngineCommand[] = [];
  readonly #listeners = new Set<(event: BridgeEvent) => void>();
  status: EngineStatus = { state: 'running', engineVersion: '0.1.0' };
  restarts = 0;
  failNextWith: string | null = null;

  request(command: EngineCommand): Promise<EngineReply> {
    this.requests.push(command);
    if (this.failNextWith !== null) {
      const message = this.failNextWith;
      this.failNextWith = null;
      return Promise.resolve({ type: 'error', requestId: null, payload: { code: 'unknownChannel', message } });
    }

    return Promise.resolve(this.#replyTo(command));
  }

  engineStatus(): Promise<EngineStatus> {
    return Promise.resolve(this.status);
  }

  restartEngine(): Promise<void> {
    this.restarts++;
    return Promise.resolve();
  }

  onEvent(listener: (event: BridgeEvent) => void): () => void {
    this.#listeners.add(listener);
    return () => this.#listeners.delete(listener);
  }

  emit(event: BridgeEvent): void {
    this.#listeners.forEach((listener) => listener(event));
  }

  #replyTo(command: EngineCommand): EngineReply {
    switch (command.type) {
      case 'getState':
        return { type: 'mixerState', requestId: null, payload: mixerState };
      case 'listDevices':
        return {
          type: 'devices',
          requestId: null,
          payload: {
            inputs: [{ id: 'umc22-in', name: 'UMC22', channelCount: 2, isDefault: true }],
            outputs: [{ id: 'umc22-out', name: 'UMC22', channelCount: 2, isDefault: true }],
          },
        };
      default:
        return { type: 'ack', requestId: null, payload: null };
    }
  }
}
