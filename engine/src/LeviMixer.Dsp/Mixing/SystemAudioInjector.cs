using LeviMixer.Dsp.Buffers;
using LeviMixer.Dsp.Resampling;

namespace LeviMixer.Dsp.Mixing;

public sealed class SystemAudioInjector : IAudioBlockProcessor
{
    private const int Channels = 2;

    private readonly IAudioBlockProcessor _mixer;
    private readonly DriftCompensatingReader _systemAudio;
    private readonly float[] _buffer;

    public SystemAudioInjector(IAudioBlockProcessor mixer, DriftCompensatingReader systemAudio, int maxFrames)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxFrames);
        _mixer = mixer;
        _systemAudio = systemAudio;
        _buffer = new float[maxFrames * Channels];
    }

    public void Process(AudioBlock block)
    {
        var frames = Math.Min(block.FrameCount, _buffer.Length / Channels);
        var systemAudio = _buffer.AsSpan(0, frames * Channels);
        _systemAudio.ReadInto(systemAudio);
        _mixer.Process(new AudioBlock(block.InterfaceInput, new InterleavedInput(systemAudio, Channels), block.Output));
    }
}
