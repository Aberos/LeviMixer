import type { MixerState } from '@levimixer/protocol';
import type { DriverInstallState } from '../application/mixerViewState.ts';
import { useMixerServices } from './MixerContext.tsx';

interface SystemAudioBannerProps {
  readonly mixer: MixerState;
  readonly driverInstall: DriverInstallState;
  readonly downloadPercent: number | null;
}

export function SystemAudioBanner({ mixer, driverInstall, downloadPercent }: SystemAudioBannerProps) {
  const { store } = useMixerServices();

  if (mixer.systemAudioError === null) {
    return null;
  }

  if (mixer.systemAudioErrorCode !== 'driverMissing') {
    return (
      <div className="banner banner-warning" role="status">
        Canal PC indisponível: {mixer.systemAudioError}
      </div>
    );
  }

  const isInstalling = driverInstall === 'downloading' || driverInstall === 'runningSetup';

  return (
    <div className="banner banner-warning" role="status">
      <span>{messageFor(driverInstall, downloadPercent)}</span>
      {driverInstall === 'installed' || driverInstall === 'openedWebsite' ? (
        <button type="button" className="button" onClick={() => void store.restartEngine()}>
          Verificar de novo
        </button>
      ) : (
        <button type="button" className="button" disabled={isInstalling} onClick={() => void store.installSystemAudioDriver()}>
          {isInstalling ? 'Instalando…' : 'Instalar VB-Cable'}
        </button>
      )}
    </div>
  );
}

function messageFor(driverInstall: DriverInstallState, downloadPercent: number | null): string {
  switch (driverInstall) {
    case 'idle':
      return 'Canal PC indisponível: falta o VB-Cable, o cabo de áudio virtual que leva o som do computador para a mesa.';
    case 'downloading':
      return `Baixando o VB-Cable do site oficial${downloadPercent === null ? '' : ` (${downloadPercent}%)`}… O servidor é lento e pode levar um minuto.`;
    case 'runningSetup':
      return 'Autorize a permissão do Windows e clique em “Install Driver” na janela do instalador do VB-Cable.';
    case 'installed':
      return 'VB-Cable instalado. Se o canal PC continuar indisponível, reinicie o computador.';
    case 'openedWebsite':
      return 'A página do VB-Cable foi aberta no navegador. Depois de instalar, clique em “Verificar de novo”.';
  }
}
