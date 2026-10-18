import { createContext, useContext, useSyncExternalStore, type ReactNode } from 'react';
import type { MeterStore } from '../application/meterStore.ts';
import type { MixerStore } from '../application/mixerStore.ts';
import type { MixerViewState } from '../application/mixerViewState.ts';

interface MixerServices {
  readonly store: MixerStore;
  readonly meters: MeterStore;
}

const MixerContext = createContext<MixerServices | null>(null);

export function MixerProvider({ store, meters, children }: MixerServices & { readonly children: ReactNode }) {
  return <MixerContext value={{ store, meters }}>{children}</MixerContext>;
}

export function useMixerServices(): MixerServices {
  const services = useContext(MixerContext);
  if (services === null) {
    throw new Error('useMixerServices must be used inside MixerProvider.');
  }

  return services;
}

export function useMixerState(): MixerViewState {
  const { store } = useMixerServices();
  return useSyncExternalStore(store.subscribe, store.getSnapshot);
}
