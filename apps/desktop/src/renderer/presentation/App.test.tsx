import { screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it } from 'vitest';
import { mixerState } from '../testing/fakeMixerGateway.ts';
import { App } from './App.tsx';
import { renderWithMixer } from './renderWithMixer.tsx';

const missingDriver = { systemAudioError: 'Install VB-Cable', systemAudioErrorCode: 'driverMissing' } as const;

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

    gateway.emit({ kind: 'engine', message: { type: 'mixerState', requestId: null, payload: { ...mixerState, systemAudioError: 'Device busy', systemAudioErrorCode: 'unavailable' } } });

    expect(await screen.findByText('Canal PC indisponível: Device busy')).toBeInTheDocument();
    expect(screen.queryByRole('button', { name: 'Instalar VB-Cable' })).not.toBeInTheDocument();
  });

  it('installs the missing PC driver from the banner', async () => {
    const { gateway } = await renderWithMixer(<App />);
    gateway.emit({ kind: 'engine', message: { type: 'mixerState', requestId: null, payload: { ...mixerState, ...missingDriver } } });

    await userEvent.click(await screen.findByRole('button', { name: 'Instalar VB-Cable' }));

    expect(gateway.driverInstalls).toBe(1);
    expect(await screen.findByText(/VB-Cable instalado/)).toBeInTheDocument();
  });

  it('shows the driver download progress while installing', async () => {
    const { gateway } = await renderWithMixer(<App />);
    gateway.holdDriverInstall = true;
    gateway.emit({ kind: 'engine', message: { type: 'mixerState', requestId: null, payload: { ...mixerState, ...missingDriver } } });

    await userEvent.click(await screen.findByRole('button', { name: 'Instalar VB-Cable' }));
    gateway.emit({ kind: 'driverInstall', progress: { stage: 'downloading', percent: 42 } });

    expect(await screen.findByText(/Baixando o VB-Cable do site oficial \(42%\)/)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Instalando…' })).toBeDisabled();

    gateway.emit({ kind: 'driverInstall', progress: { stage: 'runningSetup' } });
    expect(await screen.findByText(/Autorize a permissão do Windows/)).toBeInTheDocument();

    gateway.finishDriverInstall();
    expect(await screen.findByText(/VB-Cable instalado/)).toBeInTheDocument();
  });

  it('checks again after the driver website was opened', async () => {
    const { gateway } = await renderWithMixer(<App />);
    gateway.driverInstallOutcome = 'openedWebsite';
    gateway.emit({ kind: 'engine', message: { type: 'mixerState', requestId: null, payload: { ...mixerState, ...missingDriver } } });

    await userEvent.click(await screen.findByRole('button', { name: 'Instalar VB-Cable' }));
    await userEvent.click(await screen.findByRole('button', { name: 'Verificar de novo' }));

    expect(gateway.restarts).toBe(1);
  });

  it('reports a cancelled driver install and offers it again', async () => {
    const { gateway } = await renderWithMixer(<App />);
    gateway.driverInstallFailure = 'A instalação do VB-Cable foi cancelada ou não terminou.';
    gateway.emit({ kind: 'engine', message: { type: 'mixerState', requestId: null, payload: { ...mixerState, ...missingDriver } } });

    await userEvent.click(await screen.findByRole('button', { name: 'Instalar VB-Cable' }));

    expect(await screen.findByRole('alert')).toHaveTextContent('A instalação do VB-Cable foi cancelada');
    expect(screen.getByRole('button', { name: 'Instalar VB-Cable' })).toBeEnabled();
  });
});
