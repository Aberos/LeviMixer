import type { EngineStatus } from '../../shared/bridge.ts';
import { useMixerServices } from './MixerContext.tsx';

export function EngineStatusView({ status }: { readonly status: EngineStatus }) {
  const { store } = useMixerServices();

  if (status.state !== 'stopped') {
    return <div className="engine-status">Iniciando o engine de áudio…</div>;
  }

  return (
    <div className="engine-status">
      <p>O engine de áudio parou: {status.reason}</p>
      <button type="button" className="button" onClick={() => void store.restartEngine()}>
        Reiniciar engine
      </button>
    </div>
  );
}
