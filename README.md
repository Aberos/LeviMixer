# LeviMixer

[English](#english) · [Português](#português)

---

## English

LeviMixer turns your computer into a small mixing desk. Every input on your audio interface becomes a channel strip, the sound coming from the computer itself (browser, music player, games, calls) gets its own strip too, and everything is summed into a Main output with its own fader.

I built it because I use a Behringer UMC22 at home and wanted to control mic, guitar and computer audio from one screen, the way you would on a hardware mixer, without opening the system sound settings every time.

### Features

- One strip per input of the selected audio interface, plus a "PC" strip for the computer's audio
- Faders in dB (from silence up to +10 dB), pan, mute and solo on every strip
- Main fader with a stereo level meter
- Level meters on each strip with peak hold
- Pick the input device, output device and buffer size from the top bar
- Rename strips by double-clicking the name
- Your settings are saved automatically and restored the next time you open the app
- Lives in the system tray: minimizing or closing the window keeps the audio running; use "Quit" in the tray menu to exit

### Platform status

| Platform | Interface channels | Computer audio ("PC" strip) |
|---|---|---|
| Linux (PipeWire) | Working | Working |
| Windows | Should work, not tested yet | Planned (will use VB-Cable) |
| macOS | Should work, not tested yet | Planned |

It has been tested with a Behringer UMC22 on Ubuntu with PipeWire 1.6. Other interfaces should work as long as the operating system sees them as a regular audio device.

### How it works

The app is split in two processes:

- **Audio engine** (C# / .NET 10). Opens the audio interface through [miniaudio](https://miniaud.io), mixes every block of audio in real time and sends level readings about 30 times per second. The audio path does not allocate memory, so the garbage collector never gets in the way of the sound.
- **Desktop app** (Electron, React and TypeScript). Draws the mixer, sends your changes to the engine and saves your session.

They talk to each other through the engine's standard input and output, one JSON message per line. If the window freezes or reloads, the audio keeps going.

On Linux, to control the computer's audio, the engine creates a virtual output called **"LeviMixer PC"** and sets it as the default output while the app is open. Programs play into it, the engine picks that sound up and sends it through the "PC" strip. When you quit, the previous default output comes back and the virtual output is removed, even if the engine crashes.

### Requirements

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org) 24 or newer
- On Linux: PipeWire with its command line tools (`pw-cli`, `pw-metadata`, `wpctl`)
- On GNOME, the tray icon needs the AppIndicator extension (enabled by default on Ubuntu)

### Getting started

```bash
npm install
dotnet build engine/LeviMixer.slnx
npm run dev
```

Use headphones the first time. With the inputs unmuted and the Main fader up, your microphone goes straight to the output, and speakers close to the mic will feed back.

Other useful commands:

```bash
npm start                  # production build of the app, then open it
npm test                   # app tests
npm run typecheck
dotnet test engine/LeviMixer.slnx --filter "Category!=Hardware"
```

Tests marked `Hardware` open the real audio devices, so they are skipped in the command above.

The engine also runs on its own from the terminal, which helps when checking a new interface:

```bash
dotnet run --project engine/src/LeviMixer.Engine.Host -- devices
dotnet run --project engine/src/LeviMixer.Engine.Host -- run --master -12
```

### Building a release

```bash
npm run release
```

This publishes the engine as a self-contained executable (no .NET install needed on the target machine), builds the app and packages it for the system you are on. On Linux the files land in `apps/desktop/release/`:

- `LeviMixer-<version>-amd64.deb`, install with `sudo apt install ./LeviMixer-<version>-amd64.deb`
- `LeviMixer-<version>-x86_64.AppImage`, a single file you can run directly (on Ubuntu 24.04 and newer it needs `sudo apt install libfuse2t64`)

The Windows (`nsis`) and macOS (`dmg`) targets are configured too, but each one has to be built on its own system.

### Project layout

```
apps/desktop/         Electron app (main process, preload and React interface)
engine/src/           audio engine
  LeviMixer.Domain/       channels, gain, pan, mute and solo rules
  LeviMixer.Dsp/          real-time mixing, meters, clock drift correction
  LeviMixer.Application/  use cases and the interfaces the engine depends on
  LeviMixer.Infrastructure.*/  miniaudio, PipeWire and the message channel
  LeviMixer.Engine.Host/  the levimixer-engine executable
engine/tests/         engine tests
packages/protocol/    message types shared by both sides, with example messages
```

### Roadmap

- Computer audio on Windows (VB-Cable) and macOS
- ASIO support on Windows
- Installers for the three systems
- Option to show input level before the fader

---

## Português

O LeviMixer transforma o computador numa pequena mesa de som. Cada entrada da interface de áudio vira um canal, o som que sai do próprio computador (navegador, player de música, jogos, chamadas) ganha um canal só dele, e tudo é somado numa saída Main com fader próprio.

Fiz o projeto porque uso uma Behringer UMC22 em casa e queria controlar microfone, guitarra e o áudio do computador numa tela só, como numa mesa física, sem precisar abrir as configurações de som do sistema toda hora.

### O que ele faz

- Um canal para cada entrada da interface escolhida, mais o canal "PC" para o áudio do computador
- Faders em dB (do silêncio até +10 dB), pan, mute e solo em todos os canais
- Fader Main com medidor de nível estéreo
- Medidor de nível em cada canal, com retenção de pico
- Escolha da entrada, da saída e do tamanho do buffer na barra superior
- Para renomear um canal, clique duas vezes no nome
- As configurações são salvas sozinhas e voltam quando o app é aberto de novo
- Fica na bandeja do sistema: minimizar ou fechar a janela mantém o áudio funcionando; para sair, use "Sair" no menu da bandeja

### Situação por sistema

| Sistema | Canais da interface | Áudio do computador (canal "PC") |
|---|---|---|
| Linux (PipeWire) | Funcionando | Funcionando |
| Windows | Deve funcionar, ainda não testado | Previsto (vai usar o VB-Cable) |
| macOS | Deve funcionar, ainda não testado | Previsto |

Os testes foram feitos com uma Behringer UMC22 no Ubuntu com PipeWire 1.6. Outras interfaces devem funcionar, desde que o sistema as reconheça como um dispositivo de áudio comum.

### Como funciona

O app é dividido em dois processos:

- **Engine de áudio** (C# / .NET 10). Abre a interface pela [miniaudio](https://miniaud.io), mixa cada bloco de áudio em tempo real e envia as leituras de nível umas 30 vezes por segundo. O caminho do áudio não aloca memória, então o garbage collector não atrapalha o som.
- **App desktop** (Electron, React e TypeScript). Desenha a mesa, manda suas alterações para o engine e salva a sessão.

Os dois conversam pela entrada e saída padrão do engine, uma mensagem JSON por linha. Se a janela travar ou recarregar, o áudio continua.

No Linux, para controlar o áudio do computador, o engine cria uma saída virtual chamada **"LeviMixer PC"** e a deixa como saída padrão enquanto o app está aberto. Os programas tocam nela, o engine capta esse som e manda pelo canal "PC". Ao sair, a saída padrão anterior volta e a saída virtual é removida, mesmo que o engine feche de forma inesperada.

### Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org) 24 ou mais novo
- No Linux: PipeWire com as ferramentas de linha de comando (`pw-cli`, `pw-metadata`, `wpctl`)
- No GNOME, o ícone da bandeja precisa da extensão AppIndicator (já vem ativa no Ubuntu)

### Como rodar

```bash
npm install
dotnet build engine/LeviMixer.slnx
npm run dev
```

Use fone de ouvido na primeira vez. Com as entradas sem mute e o Main aberto, o microfone vai direto para a saída, e caixas de som perto do microfone vão microfonar.

Outros comandos úteis:

```bash
npm start                  # build de produção do app e abre em seguida
npm test                   # testes do app
npm run typecheck
dotnet test engine/LeviMixer.slnx --filter "Category!=Hardware"
```

Os testes marcados como `Hardware` abrem os dispositivos de áudio de verdade, por isso ficam de fora no comando acima.

O engine também roda sozinho no terminal, o que ajuda a testar uma interface nova:

```bash
dotnet run --project engine/src/LeviMixer.Engine.Host -- devices
dotnet run --project engine/src/LeviMixer.Engine.Host -- run --master -12
```

### Gerando uma release

```bash
npm run release
```

O comando publica o engine como um executável independente (a máquina de destino não precisa ter o .NET instalado), faz o build do app e empacota para o sistema em que você está. No Linux os arquivos ficam em `apps/desktop/release/`:

- `LeviMixer-<versão>-amd64.deb`, instale com `sudo apt install ./LeviMixer-<versão>-amd64.deb`
- `LeviMixer-<versão>-x86_64.AppImage`, um arquivo único que roda direto (no Ubuntu 24.04 ou mais novo precisa de `sudo apt install libfuse2t64`)

Os alvos de Windows (`nsis`) e macOS (`dmg`) já estão configurados, mas cada um precisa ser gerado no próprio sistema.

### Organização do código

```
apps/desktop/         app Electron (processo principal, preload e interface React)
engine/src/           engine de áudio
  LeviMixer.Domain/       canais, ganho, pan e regras de mute e solo
  LeviMixer.Dsp/          mixagem em tempo real, medidores, correção de diferença de clock
  LeviMixer.Application/  casos de uso e as interfaces das quais o engine depende
  LeviMixer.Infrastructure.*/  miniaudio, PipeWire e o canal de mensagens
  LeviMixer.Engine.Host/  o executável levimixer-engine
engine/tests/         testes do engine
packages/protocol/    tipos das mensagens trocadas entre os dois lados, com exemplos
```

### Próximos passos

- Áudio do computador no Windows (VB-Cable) e no macOS
- Suporte a ASIO no Windows
- Instaladores para os três sistemas
- Opção de mostrar o nível da entrada antes do fader
