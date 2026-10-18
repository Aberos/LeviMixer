import type { MixerBridge } from '../../shared/bridge.ts';
import type { MixerGateway } from '../application/mixerGateway.ts';

declare global {
  interface Window {
    readonly mixer: MixerBridge;
  }
}

export function bridgeGateway(): MixerGateway {
  return window.mixer;
}
