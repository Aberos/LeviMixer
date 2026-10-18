import type { GainDecibels } from '@mixer/protocol';
import { Fader } from './Fader.tsx';
import { LevelMeter } from './LevelMeter.tsx';
import { useMixerServices } from './MixerContext.tsx';

export function MasterStrip({ gainDecibels }: { readonly gainDecibels: GainDecibels }) {
  const { store, meters } = useMixerServices();

  return (
    <article className="strip strip-master" aria-label="Main">
      <span className="strip-source">MAIN</span>
      <div className="strip-fader">
        <LevelMeter read={() => meters.master()} />
        <Fader value={gainDecibels} label="Volume do Main" onChange={(gain) => void store.setMasterGain(gain)} />
      </div>
      <span className="strip-name">Main Out</span>
    </article>
  );
}
