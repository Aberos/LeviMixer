using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Hexa.NET.MiniAudio;
using Mixer.Application.Devices;

namespace Mixer.Infrastructure.Miniaudio;

public sealed unsafe class MiniaudioContext : IDisposable
{
    private const int StereoFallbackChannelCount = 2;

    private readonly Lock _gate = new();
    private MaContext* _context;

    public MiniaudioContext()
    {
        _context = (MaContext*)NativeMemory.AllocZeroed(MiniAudio.ContextSizeof());
        var config = MiniAudio.ContextConfigInit();
        var result = MiniAudio.ContextInit(null, 0, in config, ref *_context);
        if (result != MaResult.Success)
        {
            NativeMemory.Free(_context);
            _context = null;
            MiniaudioResult.EnsureSuccess(result, "Audio context initialization");
        }
    }

    internal MaContext* Handle => _context != null ? _context : throw new ObjectDisposedException(nameof(MiniaudioContext));

    internal NativeEndpointList Enumerate()
    {
        lock (_gate)
        {
            MaDeviceInfo* playback = null;
            MaDeviceInfo* capture = null;
            uint playbackCount = 0;
            uint captureCount = 0;
            MiniaudioResult.EnsureSuccess(
                MiniAudio.ContextGetDevices(ref *Handle, ref playback, &playbackCount, ref capture, &captureCount),
                "Audio device enumeration");
            return new NativeEndpointList(
                Describe(capture, captureCount, MaDeviceType.Capture),
                Describe(playback, playbackCount, MaDeviceType.Playback));
        }
    }

    public void Dispose()
    {
        if (_context == null)
        {
            return;
        }

        MiniAudio.ContextUninit(ref *_context);
        NativeMemory.Free(_context);
        _context = null;
    }

    private ImmutableArray<NativeEndpoint> Describe(MaDeviceInfo* devices, uint count, MaDeviceType type)
    {
        var names = new UniqueNames();
        var endpoints = ImmutableArray.CreateBuilder<NativeEndpoint>((int)count);
        for (var index = 0; index < count; index++)
        {
            endpoints.Add(DescribeOne(devices[index], type, names));
        }

        return endpoints.MoveToImmutable();
    }

    private NativeEndpoint DescribeOne(MaDeviceInfo device, MaDeviceType type, UniqueNames names)
    {
        var name = Marshal.PtrToStringUTF8((nint)(&device.Name_0)) ?? "Unknown device";
        var endpoint = new AudioEndpoint(new AudioDeviceId(names.Reserve(name)), name, ChannelCountOf(device.Id, type), device.IsDefault != 0);
        return new NativeEndpoint(endpoint, device.Id);
    }

    private int ChannelCountOf(MaDeviceId id, MaDeviceType type)
    {
        var details = default(MaDeviceInfo);
        if (MiniAudio.ContextGetDeviceInfo(ref *Handle, type, in id, ref details) != MaResult.Success)
        {
            return StereoFallbackChannelCount;
        }

        var formats = details.NativeDataFormats[..(int)details.NativeDataFormatCount];
        var channels = 0u;
        foreach (var format in formats)
        {
            channels = Math.Max(channels, format.Channels);
        }

        return channels == 0 ? StereoFallbackChannelCount : (int)channels;
    }
}
