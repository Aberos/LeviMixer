import { useMixerServices, useMixerState } from './MixerContext.tsx';
import { SystemAudioBanner } from './SystemAudioBanner.tsx';

export function ErrorBanner() {
  const { store } = useMixerServices();
  const { error, mixer, driverInstall, driverDownloadPercent } = useMixerState();

  return (
    <>
      {mixer !== null && <SystemAudioBanner mixer={mixer} driverInstall={driverInstall} downloadPercent={driverDownloadPercent} />}
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
