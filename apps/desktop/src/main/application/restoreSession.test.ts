import { describe, expect, it } from 'vitest';
import { FakeEngineGateway } from '../testing/fakeEngineGateway.ts';
import { savedSession } from '../testing/fixtures.ts';
import { MemorySessionRepository } from '../testing/memorySessionRepository.ts';
import { RestoreSession } from './restoreSession.ts';

describe('RestoreSession', () => {
  it('starts with default devices when nothing was saved', async () => {
    const engine = new FakeEngineGateway();

    await new RestoreSession(engine, new MemorySessionRepository()).execute();

    expect(engine.requests).toEqual([{ type: 'startMixing', payload: {} }]);
  });

  it('applies the saved session', async () => {
    const engine = new FakeEngineGateway();

    await new RestoreSession(engine, new MemorySessionRepository(savedSession)).execute();

    expect(engine.requests[0]).toMatchObject({ type: 'applySession', payload: { inputDeviceId: 'umc22-in', masterGainDecibels: -6 } });
  });

  it('keeps channel settings on default devices when the saved interface is gone', async () => {
    const engine = new FakeEngineGateway().replyWith({ type: 'error', requestId: 'r1', payload: { code: 'deviceNotFound', message: 'gone' } });

    await new RestoreSession(engine, new MemorySessionRepository(savedSession)).execute();

    expect(engine.requests[1]).toMatchObject({ type: 'applySession', payload: { inputDeviceId: null, outputDeviceId: null, channels: savedSession.channels } });
  });
});
