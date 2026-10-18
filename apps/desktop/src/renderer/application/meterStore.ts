import type { LevelReading, Meters } from '@levimixer/protocol';

const silence: LevelReading = { peak: 0, rms: 0 };

export class MeterStore {
  #channels = new Map<string, LevelReading>();
  #masterLeft: LevelReading = silence;
  #masterRight: LevelReading = silence;

  update(meters: Meters): void {
    this.#channels = new Map(meters.channels.map((channel) => [channel.channelId, channel]));
    this.#masterLeft = meters.masterLeft;
    this.#masterRight = meters.masterRight;
  }

  channel(channelId: string): LevelReading {
    return this.#channels.get(channelId) ?? silence;
  }

  master(): readonly [LevelReading, LevelReading] {
    return [this.#masterLeft, this.#masterRight];
  }
}
