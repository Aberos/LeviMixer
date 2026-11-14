namespace LeviMixer.Infrastructure.SystemAudio.Linux.Tests;

internal static class PipeWireDump
{
    public const string InterfaceName = "PCM2902 Audio Codec Estéreo analógico";

    public const string Json = """
        [
          { "id": 0, "type": "PipeWire:Interface:Core", "info": { "props": { "core.name": "pipewire-0" } } },
          { "id": 5, "type": "PipeWire:Interface:Metadata", "metadata": [] },
          { "id": 46, "type": "PipeWire:Interface:Node", "info": { "props": { "node.name": "alsa_input.umc22", "node.description": "PCM2902 Audio Codec Estéreo analógico", "media.class": "Audio/Source" } } },
          { "id": 47, "type": "PipeWire:Interface:Node", "info": { "props": { "node.name": "alsa_output.umc22", "node.description": "PCM2902 Audio Codec Estéreo analógico", "media.class": "Audio/Sink" } } },
          { "id": 61, "type": "PipeWire:Interface:Node", "info": { "props": { "node.name": "alsa_output.hdmi", "node.description": "HDMI", "media.class": "Audio/Sink" } } },
          { "id": 70, "type": "PipeWire:Interface:Port", "info": { "direction": "output", "props": { "node.id": 46, "port.id": 1, "port.name": "capture_FR", "port.direction": "out" } } },
          { "id": 69, "type": "PipeWire:Interface:Port", "info": { "direction": "output", "props": { "node.id": 46, "port.id": 0, "port.name": "capture_FL", "port.direction": "out" } } },
          { "id": 65, "type": "PipeWire:Interface:Port", "info": { "direction": "input", "props": { "node.id": 47, "port.id": 0, "port.name": "playback_FL", "port.direction": "in" } } },
          { "id": 66, "type": "PipeWire:Interface:Port", "info": { "direction": "input", "props": { "node.id": 47, "port.id": 1, "port.name": "playback_FR", "port.direction": "in" } } },
          { "id": 67, "type": "PipeWire:Interface:Port", "info": { "direction": "output", "props": { "node.id": 47, "port.id": 0, "port.name": "monitor_FL", "port.direction": "out", "port.monitor": true } } },
          { "id": 62, "type": "PipeWire:Interface:Port", "info": { "direction": "input", "props": { "node.id": 61, "port.id": 0, "port.name": "playback_MONO", "port.direction": "in" } } }
        ]
        """;
}
