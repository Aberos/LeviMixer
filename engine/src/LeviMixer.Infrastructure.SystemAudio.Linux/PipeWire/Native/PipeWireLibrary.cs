using System.Runtime.InteropServices;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire.Native;

internal static unsafe partial class PipeWireLibrary
{
    public const uint DirectionInput = 0;
    public const uint DirectionOutput = 1;
    public const uint PortFlagMapBuffers = 1;
    public const uint FilterFlagRealtimeProcess = 1 << 2;
    public const uint InvalidId = uint.MaxValue;

    private const string Name = "libpipewire-0.3.so.0";

    [LibraryImport(Name, EntryPoint = "pw_init")]
    public static partial void Init(int* argc, byte*** argv);

    [LibraryImport(Name, EntryPoint = "pw_thread_loop_new", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void* ThreadLoopNew(string name, void* properties);

    [LibraryImport(Name, EntryPoint = "pw_thread_loop_get_loop")]
    public static partial void* ThreadLoopGetLoop(void* loop);

    [LibraryImport(Name, EntryPoint = "pw_thread_loop_start")]
    public static partial int ThreadLoopStart(void* loop);

    [LibraryImport(Name, EntryPoint = "pw_thread_loop_stop")]
    public static partial void ThreadLoopStop(void* loop);

    [LibraryImport(Name, EntryPoint = "pw_thread_loop_lock")]
    public static partial void ThreadLoopLock(void* loop);

    [LibraryImport(Name, EntryPoint = "pw_thread_loop_unlock")]
    public static partial void ThreadLoopUnlock(void* loop);

    [LibraryImport(Name, EntryPoint = "pw_thread_loop_destroy")]
    public static partial void ThreadLoopDestroy(void* loop);

    [LibraryImport(Name, EntryPoint = "pw_properties_new_string", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void* PropertiesNewString(string text);

    [LibraryImport(Name, EntryPoint = "pw_filter_new_simple", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void* FilterNewSimple(void* loop, string name, void* properties, void* events, void* data);

    [LibraryImport(Name, EntryPoint = "pw_filter_add_port")]
    public static partial void* FilterAddPort(void* filter, uint direction, uint flags, nuint portDataSize, void* properties, void** parameters, uint parameterCount);

    [LibraryImport(Name, EntryPoint = "pw_filter_connect")]
    public static partial int FilterConnect(void* filter, uint flags, void** parameters, uint parameterCount);

    [LibraryImport(Name, EntryPoint = "pw_filter_get_node_id")]
    public static partial uint FilterGetNodeId(void* filter);

    [LibraryImport(Name, EntryPoint = "pw_filter_get_dsp_buffer")]
    public static partial float* FilterGetDspBuffer(void* port, uint frames);

    [LibraryImport(Name, EntryPoint = "pw_filter_destroy")]
    public static partial void FilterDestroy(void* filter);
}
