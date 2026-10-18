import { describe, expect, it } from 'vitest';
import type { EngineMessage } from '@levimixer/protocol';
import { FakeEngineTransport } from '../testing/fakeEngineTransport.ts';
import { EngineClient } from './engineClient.ts';

describe('EngineClient', () => {
  it('correlates replies with requests', async () => {
    const transport = new FakeEngineTransport();
    const client = new EngineClient(transport, 100);

    const reply = client.request({ type: 'listDevices' });
    transport.emit({ type: 'devices', requestId: transport.sent[0]?.['requestId'], payload: { inputs: [], outputs: [] } });

    await expect(reply).resolves.toMatchObject({ type: 'devices' });
  });

  it('broadcasts every message to listeners', () => {
    const transport = new FakeEngineTransport();
    const client = new EngineClient(transport, 100);
    const received: EngineMessage[] = [];
    client.onMessage((message) => received.push(message));

    transport.emit({ type: 'meters', requestId: null, payload: { channels: [], masterLeft: { peak: 0, rms: 0 }, masterRight: { peak: 0, rms: 0 } } });

    expect(received.map((message) => message.type)).toEqual(['meters']);
  });

  it('rejects pending requests when the engine exits', async () => {
    const transport = new FakeEngineTransport();
    const client = new EngineClient(transport, 100);

    const reply = client.request({ type: 'getState' });
    transport.exit('exit code 1');

    await expect(reply).rejects.toThrow('exit code 1');
  });

  it('kills the engine when shutdown takes too long', async () => {
    const transport = new FakeEngineTransport();
    transport.exitOnShutdown = false;

    await new EngineClient(transport, 10).shutdown();

    expect(transport.killed).toBe(true);
  });

  it('ignores lines that are not JSON', () => {
    const transport = new FakeEngineTransport();
    const client = new EngineClient(transport, 100);
    const received: EngineMessage[] = [];
    client.onMessage((message) => received.push(message));

    transport.emit('not json' as unknown as object);

    expect(received).toEqual([]);
  });
});
