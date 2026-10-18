import type { MixerState } from '@mixer/protocol';
import { ChannelStrip } from './ChannelStrip.tsx';
import { MasterStrip } from './MasterStrip.tsx';

export function ConsoleView({ mixer }: { readonly mixer: MixerState }) {
  return (
    <main className="console">
      <section className="strips" aria-label="Canais">
        {mixer.channels.map((channel) => (
          <ChannelStrip key={channel.id} channel={channel} />
        ))}
      </section>
      <MasterStrip gainDecibels={mixer.masterGainDecibels} />
    </main>
  );
}
