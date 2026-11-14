using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Hexa.NET.MiniAudio;
using LeviMixer.Application.Devices;
using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Infrastructure.Miniaudio;

internal sealed unsafe class MiniaudioCaptureConnection : IDisposable
{
    private const int Channels = 2;

    private readonly ISampleWriter _destination;
    private readonly NativeDevice _native;
    private readonly StreamFormat _format;
    private GCHandle _self;
    private bool _isInitialized;

    private MiniaudioCaptureConnection(MaDeviceId device, StreamFormat format, ISampleWriter destination)
    {
        _destination = destination;
        _format = format;
        _native = new NativeDevice(new DeviceIdPair(device, device));
        _self = GCHandle.Alloc(this);
    }

    public static MiniaudioCaptureConnection Open(MiniaudioContext context, MaDeviceId device, StreamFormat format, ISampleWriter destination)
    {
        var connection = new MiniaudioCaptureConnection(device, format, destination);
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
        var config = MiniAudio.DeviceConfigInit(MaDeviceType.Capture);
        config.SampleRate = (uint)_format.SampleRate;
        config.PeriodSizeInFrames = (uint)_format.BufferFrames;
        config.PerformanceProfile = MaPerformanceProfile.LowLatency;
        config.Capture.PDeviceID = _native.CaptureId;
        config.Capture.Format = MaFormat.F32;
        config.Capture.Channels = Channels;
        config.DataCallback = (delegate* unmanaged[Cdecl]<MaDevice*, void*, void*, uint, void>)&OnData;
        config.PUserData = (void*)GCHandle.ToIntPtr(_self);
        MiniaudioResult.EnsureSuccess(MiniAudio.DeviceInit(ref *context.Handle, in config, ref *_native.Device), "Capture device initialization");
        _isInitialized = true;
        MiniaudioResult.EnsureSuccess(MiniAudio.DeviceStart(ref *_native.Device), "Capture device start");
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnData(MaDevice* device, void* output, void* input, uint frameCount)
    {
        var connection = (MiniaudioCaptureConnection)GCHandle.FromIntPtr((nint)device->PUserData).Target!;
        connection._destination.Write(new ReadOnlySpan<float>(input, (int)frameCount * Channels));
    }
}
