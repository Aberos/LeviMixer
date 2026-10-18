import { ConsoleView } from './ConsoleView.tsx';
import { EngineStatusView } from './EngineStatusView.tsx';
import { ErrorBanner } from './ErrorBanner.tsx';
import { useMixerState } from './MixerContext.tsx';
import { TopBar } from './TopBar.tsx';

export function App() {
  const { status, mixer } = useMixerState();

  return (
    <div className="app">
      <TopBar />
      <ErrorBanner />
      {status.state === 'running' && mixer !== null ? <ConsoleView mixer={mixer} /> : <EngineStatusView status={status} />}
    </div>
  );
}
