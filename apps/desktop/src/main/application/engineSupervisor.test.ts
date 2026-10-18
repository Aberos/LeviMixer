import { describe, expect, it } from 'vitest';
import type { EngineStatus } from '../../shared/bridge.ts';
import { EngineClient } from '../infrastructure/engineClient.ts';
import { FakeEngineTransport } from '../testing/fakeEngineTransport.ts';
import { EngineSupervisor } from './engineSupervisor.ts';

function supervise(transport: FakeEngineTransport, readyTimeout = 1_000) {
  const supervisor = new EngineSupervisor({ launch: () => new EngineClient(transport, 100) }, readyTimeout);
  const statuses: EngineStatus[] = [];
  supervisor.onStatus((status) => statuses.push(status));
  return { supervisor, statuses };
}

describe('EngineSupervisor', () => {
  it('becomes running when the engine announces readiness', async () => {
    const transport = new FakeEngineTransport();
    const { supervisor, statuses } = supervise(transport);

    const started = supervisor.start();
    transport.emit({ type: 'ready', requestId: null, payload: { protocolVersion: 1, engineVersion: '0.1.0' } });
    await started;

    expect(statuses).toEqual([{ state: 'starting' }, { state: 'running', engineVersion: '0.1.0' }]);
  });

  it('fails to start when the engine stays silent', async () => {
    const { supervisor } = supervise(new FakeEngineTransport(), 10);

    await expect(supervisor.start()).rejects.toThrow('did not become ready');
  });

  it('reports a crash as stopped and rejects new requests', async () => {
    const transport = new FakeEngineTransport();
    const { supervisor, statuses } = supervise(transport);
    const started = supervisor.start();
    transport.emit({ type: 'ready', requestId: null, payload: { protocolVersion: 1, engineVersion: '0.1.0' } });
    await started;

    transport.exit('signal SIGKILL');

    expect(statuses.at(-1)).toEqual({ state: 'stopped', reason: 'signal SIGKILL' });
    await expect(supervisor.request({ type: 'getState' })).rejects.toThrow('not running');
  });
});
