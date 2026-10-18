using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Testing.Fakes;

public sealed class FakeAudioConnection(AudioConnectionRequest request, IAudioBlockProcessor processor) : IAudioConnection
{
    public AudioConnectionRequest Request { get; } = request;

    public IAudioBlockProcessor Processor { get; } = processor;

    public bool IsClosed { get; private set; }

    public float[] Render(float inputSample, int frameCount)
    {
        var inputChannels = Request.Input.ChannelCount;
        var input = Enumerable.Repeat(inputSample, frameCount * inputChannels).ToArray();
        var output = new float[frameCount * 2];
        Processor.Process(new AudioBlock(new InterleavedInput(input, inputChannels), InterleavedInput.None, new InterleavedOutput(output, 2)));
        return output;
    }

    public void Dispose() => IsClosed = true;
}
