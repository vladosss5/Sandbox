using System.Runtime.InteropServices;

namespace SoImport;

public class NativeMethods
{
    private const string LibraryName = "libmynativelib";
    
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetNativeString();
    
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetNativeString2(string strData);
    
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetNativeString3(string strData);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void FreeNativeString3(IntPtr strData);
}