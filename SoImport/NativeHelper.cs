using System.Runtime.InteropServices;

namespace SoImport;

public class NativeHelper
{
    private static IntPtr _libraryHandle;

    public static void LoadLibrary(string path)
    {
        if (!File.Exists(path))
            throw new Exception("Файл .so не найден");
        
        _libraryHandle = NativeLibrary.Load(path);
        Console.WriteLine($"Library loaded: {path}");
    }

    public static T GetFunction<T>(string name) where T : Delegate
    {
        var ptr = NativeLibrary.GetExport(_libraryHandle, name);
        return Marshal.GetDelegateForFunctionPointer<T>(ptr);
    }

    public static void UnloadLibrary()
    {
        if (_libraryHandle != IntPtr.Zero)
        {
            NativeLibrary.Free(_libraryHandle);
            _libraryHandle = IntPtr.Zero;
        }
    }
}