import { describe, expect, it } from 'vitest';
import { FakeMixerGateway, mixerState } from '../testing/fakeMixerGateway.ts';
import { MeterStore } from './meterStore.ts';
import { MixerStore } from './mixerStore.ts';

async function startedStore() {
  const gateway = new FakeMixerGateway();
  const meters = new MeterStore();
  const store = new MixerStore(gateway, meters);
  await store.initialize();
  return { gateway, meters, store };
}

describe('MixerStore', () => {
  it('loads devices and mixer state when the engine is running', async () => {
    const { store } = await startedStore();

    expect(store.getSnapshot().mixer).toEqual(mixerState);
    expect(store.getSnapshot().devices.inputs).toHaveLength(1);
  });

  it('updates the fader optimistically and forwards the command', async () => {
    const { gateway, store } = await startedStore();

    const sent = store.setChannelGain('in-1', -10);

    expect(store.getSnapshot().mixer?.channels[0]?.gainDecibels).toBe(-10);
    await sent;
    expect(gateway.requests.at(-1)).toEqual({ type: 'setChannelGain', payload: { channelId: 'in-1', gainDecibels: -10 } });
  });

  it('toggles mute from the current channel state', async () => {
    const { gateway, store } = await startedStore();

    await store.toggleMute(mixerState.channels[0]!);

    expect(gateway.requests.at(-1)).toEqual({ type: 'setChannelMute', payload: { channelId: 'in-1', value: true } });
  });

  it('keeps channel settings when switching devices', async () => {
    const { gateway, store } = await startedStore();

    await store.selectDevices({ inputDeviceId: 'other-in', outputDeviceId: 'umc22-out', bufferFrames: 256 });

    expect(gateway.requests.at(-1)).toMatchObject({
      type: 'applySession',
      payload: { inputDeviceId: 'other-in', bufferFrames: 256, channels: [{ id: 'in-1', name: 'Vocal' }, { id: 'pc', gainDecibels: -6 }] },
    });
  });

  it('surfaces engine errors and lets the user dismiss them', async () => {
    const { gateway, store } = await startedStore();
    gateway.failNextWith = 'Channel does not exist.';

    await store.setChannelGain('in-1', 0);
    expect(store.getSnapshot().error).toBe('Channel does not exist.');

    store.dismissError();
    expect(store.getSnapshot().error).toBeNull();
  });

  it('routes meter events to the meter store without changing view state', async () => {
    const { gateway, meters, store } = await startedStore();
    const before = store.getSnapshot();

    gateway.emit({
      kind: 'engine',
      message: { type: 'meters', requestId: null, payload: { channels: [{ channelId: 'pc', peak: 0.5, rms: 0.2 }], masterLeft: { peak: 0.1, rms: 0 }, masterRight: { peak: 0.2, rms: 0 } } },
    });

    expect(meters.channel('pc').peak).toBe(0.5);
    expect(meters.master()[1].peak).toBe(0.2);
    expect(store.getSnapshot()).toBe(before);
  });

  it('refreshes after the engine restarts', async () => {
    const { gateway } = await startedStore();
    const requestsBefore = gateway.requests.length;

    gateway.emit({ kind: 'status', status: { state: 'running', engineVersion: '0.1.0' } });
    await Promise.resolve();

    expect(gateway.requests.slice(requestsBefore).map((command) => command.type)).toContain('listDevices');
  });
});
