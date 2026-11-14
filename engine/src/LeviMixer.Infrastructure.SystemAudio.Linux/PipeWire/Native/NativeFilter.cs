using System.Collections.Immutable;
using System.Runtime.InteropServices;
using LeviMixer.Application.Devices;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire.Native;

internal sealed unsafe class NativeFilter : IDisposable
{
    private const int EventsSize = 80;
    private const int ProcessSlot = 7;
    private const uint EventsVersion = 1;

    private static int _isLibraryInitialized;

    private readonly void* _loop;
    private readonly nint* _events;
    private void* _filter;
    private bool _isRunning;

    private NativeFilter(delegate* unmanaged[Cdecl]<void*, void*, void> process)
    {
        InitializeLibrary();
        _loop = PipeWireLibrary.ThreadLoopNew("levimixer-audio", null);
        _events = (nint*)NativeMemory.AllocZeroed(EventsSize);
        *(uint*)_events = EventsVersion;
        _events[ProcessSlot] = (nint)process;
    }

    public nint[] InputPorts { get; private set; } = [];

    public nint[] OutputPorts { get; private set; } = [];

    public uint NodeId
    {
        get
        {
            PipeWireLibrary.ThreadLoopLock(_loop);
            var id = PipeWireLibrary.FilterGetNodeId(_filter);
            PipeWireLibrary.ThreadLoopUnlock(_loop);
            return id;
        }
    }

    public static NativeFilter Open(FilterSettings settings, delegate* unmanaged[Cdecl]<void*, void*, void> process, void* userData)
    {
        var filter = new NativeFilter(process);
        try
        {
            filter.Connect(settings, userData);
            return filter;
        }
        catch
        {
            filter.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        if (_filter != null)
        {
            PipeWireLibrary.ThreadLoopLock(_loop);
            PipeWireLibrary.FilterDestroy(_filter);
            PipeWireLibrary.ThreadLoopUnlock(_loop);
            _filter = null;
        }

        if (_loop != null)
        {
            StopLoop();
            PipeWireLibrary.ThreadLoopDestroy(_loop);
        }

        NativeMemory.Free(_events);
    }

    private static void InitializeLibrary()
    {
        if (Interlocked.Exchange(ref _isLibraryInitialized, 1) == 0)
        {
            PipeWireLibrary.Init(null, null);
        }
    }

    private void Connect(FilterSettings settings, void* userData)
    {
        if (_loop == null)
        {
            throw new AudioDeviceException("Could not create the PipeWire loop.");
        }

        PipeWireLibrary.ThreadLoopLock(_loop);
        try
        {
            CreateFilter(settings, userData);
        }
        finally
        {
            PipeWireLibrary.ThreadLoopUnlock(_loop);
        }

        _isRunning = PipeWireLibrary.ThreadLoopStart(_loop) >= 0;
        if (!_isRunning)
        {
            throw new AudioDeviceException("Could not start the PipeWire loop.");
        }
    }

    private void CreateFilter(FilterSettings settings, void* userData)
    {
        var properties = PipeWireLibrary.PropertiesNewString(settings.NodeProperties);
        _filter = PipeWireLibrary.FilterNewSimple(PipeWireLibrary.ThreadLoopGetLoop(_loop), settings.NodeName, properties, _events, userData);
        if (_filter == null)
        {
            throw new AudioDeviceException("Could not create the PipeWire filter.");
        }

        InputPorts = AddPorts(settings.InputPorts, PipeWireLibrary.DirectionInput);
        OutputPorts = AddPorts(settings.OutputPorts, PipeWireLibrary.DirectionOutput);
        if (PipeWireLibrary.FilterConnect(_filter, PipeWireLibrary.FilterFlagRealtimeProcess, null, 0) < 0)
        {
            throw new AudioDeviceException("Could not connect the PipeWire filter.");
        }
    }

    private nint[] AddPorts(ImmutableArray<string> names, uint direction)
    {
        var ports = new nint[names.Length];
        for (var index = 0; index < names.Length; index++)
        {
            var properties = PipeWireLibrary.PropertiesNewString(FilterSettings.PortProperties(names[index]));
            ports[index] = (nint)PipeWireLibrary.FilterAddPort(_filter, direction, PipeWireLibrary.PortFlagMapBuffers, (nuint)sizeof(nint), properties, null, 0);
            if (ports[index] == 0)
            {
                throw new AudioDeviceException($"Could not add the PipeWire port '{names[index]}'.");
            }
        }

        return ports;
    }

    private void StopLoop()
    {
        if (_isRunning)
        {
            PipeWireLibrary.ThreadLoopStop(_loop);
            _isRunning = false;
        }
    }
}
