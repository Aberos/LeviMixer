using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using LeviMixer.Application.Devices;

namespace LeviMixer.Infrastructure.SystemAudio.Windows.Native;

[SupportedOSPlatform("windows")]
internal static unsafe partial class WindowsAudioEndpoints
{
    public const int ConsoleRole = 0;
    public const int MultimediaRole = 1;
    public const int CommunicationsRole = 2;

    private const int RenderFlow = 0;
    private const uint AllContexts = 0x17;
    private const uint MultithreadedApartment = 0;
    private const int ElementNotFound = unchecked((int)0x80070490);
    private const int ReleaseSlot = 2;
    private const int GetDefaultAudioEndpointSlot = 4;
    private const int GetIdSlot = 5;
    private const int SetDefaultEndpointSlot = 13;

    private static readonly Guid DeviceEnumeratorClass = new("bcde0395-e52f-467c-8e3d-c4579291692e");
    private static readonly Guid DeviceEnumeratorInterface = new("a95664d2-9614-4f35-a746-de8db63617e6");
    private static readonly Guid PolicyConfigClass = new("870af99c-171d-4f9e-af0d-e63df40c2bc9");
    private static readonly Guid PolicyConfigInterface = new("f8679f50-850a-41cf-9c72-430f290290c8");

    public static string? DefaultOutputOf(int role)
    {
        using var apartment = ComApartment.Enter();
        var enumerator = Create(DeviceEnumeratorClass, DeviceEnumeratorInterface);
        try
        {
            nint device;
            var result = ((delegate* unmanaged[Stdcall]<nint, int, int, nint*, int>)Slot(enumerator, GetDefaultAudioEndpointSlot))(enumerator, RenderFlow, role, &device);
            if (result == ElementNotFound)
            {
                return null;
            }

            EnsureSuccess(result, "Reading the default output");
            try
            {
                return IdOf(device);
            }
            finally
            {
                Release(device);
            }
        }
        finally
        {
            Release(enumerator);
        }
    }

    public static void SetDefaultOutput(string endpointId, int role)
    {
        using var apartment = ComApartment.Enter();
        var policy = Create(PolicyConfigClass, PolicyConfigInterface);
        try
        {
            fixed (char* id = endpointId)
            {
                var result = ((delegate* unmanaged[Stdcall]<nint, char*, int, int>)Slot(policy, SetDefaultEndpointSlot))(policy, id, role);
                EnsureSuccess(result, "Changing the default output");
            }
        }
        finally
        {
            Release(policy);
        }
    }

    private static string IdOf(nint device)
    {
        char* id;
        EnsureSuccess(((delegate* unmanaged[Stdcall]<nint, char**, int>)Slot(device, GetIdSlot))(device, &id), "Reading the output id");
        try
        {
            return new string(id);
        }
        finally
        {
            CoTaskMemFree((nint)id);
        }
    }

    private static nint Create(Guid classId, Guid interfaceId)
    {
        EnsureSuccess(CoCreateInstance(in classId, 0, AllContexts, in interfaceId, out var instance), "Creating a Windows audio object");
        return instance;
    }

    private static void* Slot(nint instance, int slot) => (*(void***)instance)[slot];

    private static void Release(nint instance) => ((delegate* unmanaged[Stdcall]<nint, uint>)Slot(instance, ReleaseSlot))(instance);

    private static void EnsureSuccess(int result, string operation)
    {
        if (result < 0)
        {
            throw new AudioDeviceException($"{operation} failed (HRESULT 0x{result:X8}).");
        }
    }

    [LibraryImport("ole32.dll")]
    private static partial int CoInitializeEx(nint reserved, uint apartment);

    [LibraryImport("ole32.dll")]
    private static partial void CoUninitialize();

    [LibraryImport("ole32.dll")]
    private static partial int CoCreateInstance(in Guid classId, nint outer, uint context, in Guid interfaceId, out nint instance);

    [LibraryImport("ole32.dll")]
    private static partial void CoTaskMemFree(nint memory);

    private readonly struct ComApartment(bool mustLeave) : IDisposable
    {
        public static ComApartment Enter() => new(CoInitializeEx(0, MultithreadedApartment) >= 0);

        public void Dispose()
        {
            if (mustLeave)
            {
                CoUninitialize();
            }
        }
    }
}
