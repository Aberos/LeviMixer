using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Hexa.NET.MiniAudio;
using Mixer.Application.Devices;
using Mixer.Dsp.Buffers;

namespace Mixer.Infrastructure.Miniaudio;

internal sealed unsafe class MiniaudioDuplexConnection : IAudioConnection
{
    private const int OutputChannelCount = 2;

    private readonly IAudioBlockProcessor _processor;
    private readonly NativeDevice _native;
    private readonly int _inputChannelCount;
    private GCHandle _self;
    private bool _isInitialized;

    private MiniaudioDuplexConnection(AudioConnectionRequest request, DeviceIdPair devices, IAudioBlockProcessor processor)
    {
        Request = request;
        _processor = processor;
        _inputChannelCount = request.Input.ChannelCount;
        _native = new NativeDevice(devices);
        _self = GCHandle.Alloc(this);
    }

    public AudioConnectionRequest Request { get; }

    public static MiniaudioDuplexConnection Open(MiniaudioContext context, AudioConnectionRequest request, DeviceIdPair devices, IAudioBlockProcessor processor)
    {
        var connection = new MiniaudioDuplexConnection(request, devices, processor);
        try
        {
            connection.Initialize(context);
            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        if (_isInitialized)
        {
            MiniAudio.DeviceUninit(ref *_native.Device);
            _isInitialized = false;
        }

        if (_self.IsAllocated)
        {
            _self.Free();
        }

        _native.Dispose();
    }

    private void Initialize(MiniaudioContext context)
    {
        var config = CreateConfig();
        MiniaudioResult.EnsureSuccess(MiniAudio.DeviceInit(ref *context.Handle, in config, ref *_native.Device), "Audio device initialization");
        _isInitialized = true;
        MiniaudioResult.EnsureSuccess(MiniAudio.DeviceStart(ref *_native.Device), "Audio device start");
    }

    private MaDeviceConfig CreateConfig()
    {
        var config = MiniAudio.DeviceConfigInit(MaDeviceType.Duplex);
        config.SampleRate = (uint)Request.Format.SampleRate;
        config.PeriodSizeInFrames = (uint)Request.Format.BufferFrames;
        config.PerformanceProfile = MaPerformanceProfile.LowLatency;
        config.Capture.PDeviceID = _native.CaptureId;
        config.Capture.Format = MaFormat.F32;
        config.Capture.Channels = (uint)_inputChannelCount;
        config.Playback.PDeviceID = _native.PlaybackId;
        config.Playback.Format = MaFormat.F32;
        config.Playback.Channels = OutputChannelCount;
        config.DataCallback = (delegate* unmanaged[Cdecl]<MaDevice*, void*, void*, uint, void>)&OnData;
        config.PUserData = (void*)GCHandle.ToIntPtr(_self);
        return config;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnData(MaDevice* device, void* output, void* input, uint frameCount)
    {
        var connection = (MiniaudioDuplexConnection)GCHandle.FromIntPtr((nint)device->PUserData).Target!;
        connection.Render(new ReadOnlySpan<float>(input, (int)frameCount * connection._inputChannelCount), new Span<float>(output, (int)frameCount * OutputChannelCount));
    }

    private void Render(ReadOnlySpan<float> input, Span<float> output) =>
        _processor.Process(new AudioBlock(new InterleavedInput(input, _inputChannelCount), InterleavedInput.None, new InterleavedOutput(output, OutputChannelCount)));
}
