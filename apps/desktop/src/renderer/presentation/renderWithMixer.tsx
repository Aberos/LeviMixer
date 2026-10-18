import { render } from '@testing-library/react';
import type { ReactElement } from 'react';
import { MeterStore } from '../application/meterStore.ts';
import { MixerStore } from '../application/mixerStore.ts';
import { FakeMixerGateway } from '../testing/fakeMixerGateway.ts';
import { MixerProvider } from './MixerContext.tsx';

export async function renderWithMixer(element: ReactElement) {
  const gateway = new FakeMixerGateway();
  const meters = new MeterStore();
  const store = new MixerStore(gateway, meters);
  await store.initialize();
  const view = render(
    <MixerProvider store={store} meters={meters}>
      {element}
    </MixerProvider>,
  );
  return { gateway, store, view };
}
