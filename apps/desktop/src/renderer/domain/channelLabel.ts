import type { ChannelState } from '@levimixer/protocol';

export function sourceLabel(channel: ChannelState): string {
  return channel.source === 'system' ? 'PC' : `IN ${(channel.inputIndex ?? 0) + 1}`;
}
