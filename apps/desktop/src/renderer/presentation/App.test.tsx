import { screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it } from 'vitest';
import { mixerState } from '../testing/fakeMixerGateway.ts';
import { App } from './App.tsx';
import { renderWithMixer } from './renderWithMixer.tsx';

describe('App', () => {
  it('shows one strip per channel plus the main strip', async () => {
    await renderWithMixer(<App />);

    expect(screen.getByRole('article', { name: 'Vocal' })).toBeInTheDocument();
    expect(screen.getByRole('article', { name: 'PC' })).toBeInTheDocument();
    expect(screen.getByRole('article', { name: 'Main' })).toBeInTheDocument();
  });

  it('mutes a channel from its strip', async () => {
    const { gateway } = await renderWithMixer(<App />);

    await userEvent.click(screen.getByRole('button', { name: 'Mute Vocal' }));

    expect(gateway.requests.at(-1)).toEqual({ type: 'setChannelMute', payload: { channelId: 'in-1', value: true } });
    expect(screen.getByRole('button', { name: 'Mute Vocal' })).toHaveAttribute('aria-pressed', 'true');
  });

  it('moves a fader with the keyboard', async () => {
    const { gateway } = await renderWithMixer(<App />);
    const fader = within(screen.getByRole('article', { name: 'PC' })).getByRole('slider', { name: 'Volume de PC' });

    fader.focus();
    await userEvent.keyboard('{End}');

    expect(gateway.requests.at(-1)).toEqual({ type: 'setChannelGain', payload: { channelId: 'pc', gainDecibels: null } });
    expect(fader).toHaveAttribute('aria-valuetext', '-∞ dB');
  });

  it('resets a fader to unity on double click', async () => {
    const { gateway } = await renderWithMixer(<App />);

    await userEvent.dblClick(screen.getByRole('slider', { name: 'Volume de PC' }));

    expect(gateway.requests.at(-1)).toEqual({ type: 'setChannelGain', payload: { channelId: 'pc', gainDecibels: 0 } });
  });

  it('renames a channel after double clicking its name', async () => {
    const { gateway } = await renderWithMixer(<App />);

    await userEvent.dblClick(screen.getByRole('button', { name: 'Vocal' }));
    const input = screen.getByRole('textbox', { name: 'Nome do canal' });
    await userEvent.clear(input);
    await userEvent.type(input, 'Lead{Enter}');

    expect(gateway.requests.at(-1)).toEqual({ type: 'renameChannel', payload: { channelId: 'in-1', name: 'Lead' } });
  });

  it('offers a restart when the engine stops', async () => {
    const { gateway } = await renderWithMixer(<App />);

    gateway.emit({ kind: 'status', status: { state: 'stopped', reason: 'exit code 1' } });
    await userEvent.click(await screen.findByRole('button', { name: 'Reiniciar engine' }));

    expect(gateway.restarts).toBe(1);
  });

  it('warns when the PC channel cannot capture', async () => {
    const { gateway } = await renderWithMixer(<App />);

    gateway.emit({ kind: 'engine', message: { type: 'mixerState', requestId: null, payload: { ...mixerState, systemAudioError: 'Install VB-Cable' } } });

    expect(await screen.findByText('Canal PC indisponível: Install VB-Cable')).toBeInTheDocument();
  });
});
