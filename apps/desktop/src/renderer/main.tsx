import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { MeterStore } from './application/meterStore.ts';
import { MixerStore } from './application/mixerStore.ts';
import { bridgeGateway } from './infrastructure/bridgeGateway.ts';
import { App } from './presentation/App.tsx';
import { MixerProvider } from './presentation/MixerContext.tsx';
import './styles.css';

const meters = new MeterStore();
const store = new MixerStore(bridgeGateway(), meters);
void store.initialize();

createRoot(document.getElementById('root') as HTMLElement).render(
  <StrictMode>
    <MixerProvider store={store} meters={meters}>
      <App />
    </MixerProvider>
  </StrictMode>,
);
