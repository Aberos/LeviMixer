import { useMixerServices, useMixerState } from './MixerContext.tsx';

export function ErrorBanner() {
  const { store } = useMixerServices();
  const { error, mixer } = useMixerState();
  const systemAudioError = mixer?.systemAudioError ?? null;

  return (
    <>
      {systemAudioError !== null && (
        <div className="banner banner-warning" role="status">
          Canal PC indisponível: {systemAudioError}
        </div>
      )}
      {error !== null && (
        <div className="banner banner-error" role="alert">
          <span>{error}</span>
          <button type="button" className="banner-close" aria-label="Fechar aviso" onClick={() => store.dismissError()}>
            ×
          </button>
        </div>
      )}
    </>
  );
}
