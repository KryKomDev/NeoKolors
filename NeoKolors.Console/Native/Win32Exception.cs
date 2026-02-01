// NeoKolors
// Copyright (c) 2025 KryKom

using System.Runtime.InteropServices;

namespace NeoKolors.Console.Native;

// ReSharper disable once PartialTypeWithSinglePart
internal static partial class Win32Exception {

    #if NET7_0_OR_GREATER
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetStdHandle(int nStdHandle, IntPtr hHandle);
    #else
    [DllImport("kernel32.dll")]
    private static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);
    #endif

    private const int STD_ERROR_HANDLE = -12;

    internal static void Mute(bool safe = true) {
        if (safe) {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT) 
                SetStdHandle(STD_ERROR_HANDLE, IntPtr.Zero);   
            
            return;
        }
        
        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            throw new PlatformNotSupportedException("Exception muting supported only on Windows.");
        
        SetStdHandle(STD_ERROR_HANDLE, IntPtr.Zero);
    }
}