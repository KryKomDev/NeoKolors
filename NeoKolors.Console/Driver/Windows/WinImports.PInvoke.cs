// 
// NeoKolors
// Copyright (c) 2026 KryKom
// 

#if NET7_0_OR_GREATER
#define NK_LIBIMPORT
#endif

using System.Runtime.InteropServices;

namespace NeoKolors.Console.Driver.Windows;

internal static partial class WinImports {
    
    // --------------------------- P/INVOKE --------------------------- // 

    #region P/Invoke Declarations
    
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/errhandlingapi/nf-errhandlingapi-getlasterror"/>
    #if NK_LIBIMPORT
    [LibraryImport("kernel32.dll")]
    public static partial uint GetLastError();
    #else
    [DllImport("kernel32.dll")]
    public static extern uint GetLastError();
    #endif
    
    private const int STD_INPUT_HANDLE_WORD  = -10;
    private const int STD_OUTPUT_HANDLE_WORD = -11;
    private const int STD_ERROR_HANDLE_WORD  = -12;
    
    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/getstdhandle"/>
    #if NK_LIBIMPORT
    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial nint GetStdHandle(int nStdHandle);
    #else
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GetStdHandle(int nStdHandle);
    #endif
    
    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/setstdhandle"/>
    #if NK_LIBIMPORT
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetStdHandle(int nStdHandle, nint hHandle);
    #else
    [DllImport("kernel32.dll")]
    private static extern bool SetStdHandle(int nStdHandle, nint hHandle);
    #endif

    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/setconsolemode#parameters"/>
    [Flags]
    internal enum WinVtInModes : uint {
        ENABLE_PROCESSED_INPUT        = 0x0001,
        ENABLE_LINE_INPUT             = 0x0002,
        ENABLE_ECHO_INPUT             = 0x0004,
        ENABLE_WINDOW_INPUT           = 0x0008,
        ENABLE_MOUSE_INPUT            = 0x0010,
        ENABLE_INSERT_MODE            = 0x0020,
        ENABLE_QUICK_EDIT_MODE        = 0x0040,
        ENABLE_EXTENDED_FLAGS         = 0x0080,
        ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200,
    }

    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/setconsolemode#parameters"/>
    [Flags]
    internal enum WinVtOutModes : uint {
        ENABLE_PROCESSED_OUTPUT            = 0x0001,
        ENABLE_WRAP_AT_EOL_OUTPUT          = 0x0002,
        ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
        DISABLE_NEWLINE_AUTO_RETURN        = 0x0008,
        ENABLE_LVB_GRID_WORLDWIDE          = 0x0010,
    } 
    
    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/setconsolemode"/>
    #if NK_LIBIMPORT
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleMode(nint hConsoleHandle, uint dwMode);
    #else
    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(nint hConsoleHandle, uint dwMode);
    #endif
    
    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/getconsolemode"/>
    #if NK_LIBIMPORT
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);
    #else
    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);
    #endif

    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/flushconsoleinputbuffer"/>
    #if NK_LIBIMPORT
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FlushConsoleInputBuffer(nint hConsoleInput);
    #else
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool FlushConsoleInputBuffer(nint hConsoleInput);
    #endif

    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/peekconsoleinput"/>
    #if NK_LIBIMPORT
    [LibraryImport("kernel32.dll", EntryPoint = "PeekConsoleInputW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool PeekConsoleInput (
        nint hConsoleInput,
        nint lpBuffer,
        uint nLength,
        out uint lpNumberOfEventsRead
    );
    #else
    [DllImport ("kernel32.dll", EntryPoint = "PeekConsoleInputW", CharSet = CharSet.Unicode)]
    public static extern bool PeekConsoleInput (
        nint hConsoleInput,
        nint lpBuffer,
        uint nLength,
        out uint lpNumberOfEventsRead
    );
    #endif

    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/readconsoleinput"/>
    #if NK_LIBIMPORT
    [LibraryImport("kernel32.dll", EntryPoint = "ReadConsoleInputW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool ReadConsoleInput(
        nint hConsoleInput,
        nint lpBuffer,
        uint nLength,
        out uint lpNumberOfEventsRead
    );
    #else
    [DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode)]
    public static extern bool ReadConsoleInput(
        nint hConsoleInput,
        nint lpBuffer,
        uint nLength,
        out uint lpNumberOfEventsRead
    );
    #endif

    /// <see href="https://learn.microsoft.com/en-us/windows/console/getnumberofconsoleinputevents"/>
    #if NK_LIBIMPORT
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetNumberOfConsoleInputEvents(nint hConsoleInput, out uint numberOfEvents);
    #else
    [DllImport("kernel32.dll")]
    internal static extern bool GetNumberOfConsoleInputEvents(nint hConsoleInput, out uint numberOfEvents);
    #endif

    /// <see href="https://learn.microsoft.com/en-us/windows/console/writeconsole"/>
    #if NK_LIBIMPORT
    [LibraryImport("kernel32.dll", EntryPoint = "WriteConsoleW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static unsafe partial bool WriteConsole(
        nint hConsoleOutput,
        nint lpBuffer,
        uint nNumberOfBytesToWrite,
        out uint lpNumberOfCharsWritten,
        void* lpReserved
    ); 
    #else
    [DllImport("kernel32.dll", EntryPoint = "WriteConsoleW", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern unsafe bool WriteConsole(
        nint hConsoleOutput,
        nint lpBuffer,
        uint nNumberOfBytesToWrite,
        out uint lpNumberOfCharsWritten,
        void* lpReserved
    ); 
    #endif

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-writefile"/>
    #if NK_LIBIMPORT
    [LibraryImport("kernel32.dll", EntryPoint = "WriteFileW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool WriteFile(
        nint hFile,
        nint lpBuffer,
        uint nNumberOfBytesToWrite,
        out uint lpNumberOfCharsWritten,
        nint lpOverlapped
    ); 
    #else
    [DllImport("kernel32.dll", EntryPoint = "WriteFileW", CharSet = CharSet.Unicode)]
    private static extern bool WriteFile(
        nint hFile,
        nint lpBuffer,
        uint nNumberOfBytesToWrite,
        out uint lpNumberOfCharsWritten,
        nint lpOverlapped
    ); 
    #endif
    
    #endregion
    
    
    // --------------------------- WRAPPER --------------------------- //

    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/getstdhandle"/>
    public static nint GetStdIn () => GetStdHandle(STD_INPUT_HANDLE_WORD);
    
    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/getstdhandle"/>
    public static nint GetStdOut() => GetStdHandle(STD_OUTPUT_HANDLE_WORD);
   
    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/getstdhandle"/>
    public static nint GetStdErr() => GetStdHandle(STD_ERROR_HANDLE_WORD);

    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/setstdhandle"/>
    public static bool SetStdIn (nint handle) => SetStdHandle(STD_INPUT_HANDLE_WORD,  handle);
    
    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/setstdhandle"/>
    public static bool SetStdOut(nint handle) => SetStdHandle(STD_OUTPUT_HANDLE_WORD, handle);
    
    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/setstdhandle"/>
    public static bool SetStdErr(nint handle) => SetStdHandle(STD_ERROR_HANDLE_WORD,  handle);

    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/getconsolemode"/>
    public static bool GetStdInMode(nint handle, out WinVtInModes mode) {
        var res = GetConsoleMode(handle, out uint lpMode);
        mode = (WinVtInModes)lpMode;
        return res;
    }

    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/getconsolemode"/>
    public static bool GetStdOutMode(nint handle, out WinVtOutModes mode) {
        var res = GetConsoleMode(handle, out uint lpMode);
        mode = (WinVtOutModes)lpMode;
        return res;
    }

    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/setconsolemode"/>
    public static bool SetStdInMode(nint handle, WinVtInModes mode) => SetConsoleMode(handle, (uint)mode);

    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/setconsolemode"/>
    public static bool SetStdOutMode(nint handle, WinVtOutModes mode) => SetConsoleMode(handle, (uint)mode);

    public static unsafe bool WriteConsole(nint handle, ReadOnlySpan<char> chars, out uint written) {
        fixed (char* ptr = chars) {
            
            // not redirected
            if (WriteConsole(handle, (nint)ptr, (uint)chars.Length, out written, (void*)IntPtr.Zero))
                return true;
            
            // redirected
            return WriteFile(handle, (nint)ptr, (uint)chars.Length * 2, out written, IntPtr.Zero);
        }
    }
}