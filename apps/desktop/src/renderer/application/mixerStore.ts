import type { ChannelState, EngineCommand, EngineMessage, EngineReply, GainDecibels, MixerState } from '@mixer/protocol';
import type { BridgeEvent } from '../../shared/bridge.ts';
import type { MeterStore } from './meterStore.ts';
import type { MixerGateway } from './mixerGateway.ts';
import { initialViewState, type MixerViewState } from './mixerViewState.ts';

export interface DeviceChoice {
  readonly inputDeviceId: string;
  readonly outputDeviceId: string;
  readonly bufferFrames: number;
}

type Listener = () => void;

export class MixerStore {
  readonly #gateway: MixerGateway;
  readonly #meters: MeterStore;
  readonly #listeners = new Set<Listener>();
  #state: MixerViewState = initialViewState;

  constructor(gateway: MixerGateway, meters: MeterStore) {
    this.#gateway = gateway;
    this.#meters = meters;
  }

  readonly subscribe = (listener: Listener): (() => void) => {
    this.#listeners.add(listener);
    return () => this.#listeners.delete(listener);
  };

  readonly getSnapshot = (): MixerViewState => this.#state;

  async initialize(): Promise<() => void> {
    const unsubscribe = this.#gateway.onEvent((event) => this.#handle(event));
    this.#update({ status: await this.#gateway.engineStatus() });
    await this.refresh();
    return unsubscribe;
  }

  async refresh(): Promise<void> {
    if (this.#state.status.state === 'running') {
      await this.#send({ type: 'listDevices' });
      await this.#send({ type: 'getState' });
    }
  }

  setChannelGain(channelId: string, gainDecibels: GainDecibels): Promise<void> {
    this.#updateChannel(channelId, { gainDecibels });
    return this.#send({ type: 'setChannelGain', payload: { channelId, gainDecibels } });
  }

  toggleMute(channel: ChannelState): Promise<void> {
    this.#updateChannel(channel.id, { muted: !channel.muted });
    return this.#send({ type: 'setChannelMute', payload: { channelId: channel.id, value: !channel.muted } });
  }

  toggleSolo(channel: ChannelState): Promise<void> {
    this.#updateChannel(channel.id, { soloed: !channel.soloed });
    return this.#send({ type: 'setChannelSolo', payload: { channelId: channel.id, value: !channel.soloed } });
  }

  setChannelPan(channelId: string, pan: number): Promise<void> {
    this.#updateChannel(channelId, { pan });
    return this.#send({ type: 'setChannelPan', payload: { channelId, pan } });
  }

  renameChannel(channelId: string, name: string): Promise<void> {
    return this.#send({ type: 'renameChannel', payload: { channelId, name } });
  }

  setMasterGain(gainDecibels: GainDecibels): Promise<void> {
    this.#updateMixer((mixer) => ({ ...mixer, masterGainDecibels: gainDecibels }));
    return this.#send({ type: 'setMasterGain', payload: { gainDecibels } });
  }

  selectDevices(choice: DeviceChoice): Promise<void> {
    const mixer = this.#state.mixer;
    return this.#send({
      type: 'applySession',
      payload: {
        ...choice,
        channels: mixer?.channels.map(({ id, name, gainDecibels, pan, muted, soloed }) => ({ id, name, gainDecibels, pan, muted, soloed })) ?? [],
        masterGainDecibels: mixer?.masterGainDecibels ?? 0,
      },
    });
  }

  restartEngine(): Promise<void> {
    return this.#gateway.restartEngine();
  }

  dismissError(): void {
    this.#update({ error: null });
  }

  async #send(command: EngineCommand): Promise<void> {
    try {
      this.#apply(await this.#gateway.request(command));
    } catch (error) {
      this.#update({ error: error instanceof Error ? error.message : String(error) });
    }
  }

  #handle(event: BridgeEvent): void {
    if (event.kind === 'status') {
      this.#update({ status: event.status });
      if (event.status.state === 'running') {
        void this.refresh();
      }

      return;
    }

    this.#apply(event.message);
  }

  #apply(message: EngineMessage | EngineReply): void {
    switch (message.type) {
      case 'meters':
        this.#meters.update(message.payload);
        return;
      case 'mixerState':
        this.#update({ mixer: message.payload });
        return;
      case 'devices':
        this.#update({ devices: message.payload });
        return;
      case 'error':
        this.#update({ error: message.payload.message });
        return;
      default:
        return;
    }
  }

  #updateChannel(channelId: string, change: Partial<ChannelState>): void {
    this.#updateMixer((mixer) => ({
      ...mixer,
      channels: mixer.channels.map((channel) => (channel.id === channelId ? { ...channel, ...change } : channel)),
    }));
  }

  #updateMixer(change: (mixer: MixerState) => MixerState): void {
    if (this.#state.mixer !== null) {
      this.#update({ mixer: change(this.#state.mixer) });
    }
  }

  #update(change: Partial<MixerViewState>): void {
    this.#state = { ...this.#state, ...change };
    this.#listeners.forEach((listener) => listener());
  }
}
