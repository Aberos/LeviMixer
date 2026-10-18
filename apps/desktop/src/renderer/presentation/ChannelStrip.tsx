import type { ChannelState } from '@levimixer/protocol';
import { sourceLabel } from '../domain/channelLabel.ts';
import { ChannelName } from './ChannelName.tsx';
import { Fader } from './Fader.tsx';
import { LevelMeter } from './LevelMeter.tsx';
import { useMixerServices } from './MixerContext.tsx';
import { PanControl } from './PanControl.tsx';
import { ToggleButton } from './ToggleButton.tsx';

export function ChannelStrip({ channel }: { readonly channel: ChannelState }) {
  const { store, meters } = useMixerServices();

  return (
    <article className={`strip${channel.source === 'system' ? ' strip-system' : ''}`} aria-label={channel.name}>
      <span className="strip-source">{sourceLabel(channel)}</span>
      <PanControl value={channel.pan} label={`Pan de ${channel.name}`} onChange={(pan) => void store.setChannelPan(channel.id, pan)} />
      <div className="strip-buttons">
        <ToggleButton label="M" title={`Mute ${channel.name}`} variant="mute" active={channel.muted} onToggle={() => void store.toggleMute(channel)} />
        <ToggleButton label="S" title={`Solo ${channel.name}`} variant="solo" active={channel.soloed} onToggle={() => void store.toggleSolo(channel)} />
      </div>
      <div className="strip-fader">
        <LevelMeter read={() => [meters.channel(channel.id)]} />
        <Fader value={channel.gainDecibels} label={`Volume de ${channel.name}`} onChange={(gain) => void store.setChannelGain(channel.id, gain)} />
      </div>
      <ChannelName name={channel.name} onRename={(name) => void store.renameChannel(channel.id, name)} />
    </article>
  );
}
