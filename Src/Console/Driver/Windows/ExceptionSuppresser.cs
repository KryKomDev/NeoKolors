// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Console.Driver.Windows;

/// <summary>
/// Provides functionality to suppress exceptions in Windows-specific environments
/// by muting standard error streams.
/// </summary>
internal static class WinExceptionSuppressor {
    
    /// <summary>
    /// Suppresses exceptions in Windows-specific environments by muting the standard error stream.
    /// </summary>
    /// <param name="safe">
    /// Indicates whether the method should perform a safe, no-throw operation. If set to <c>true</c>,
    /// the method will silently attempt to mute standard error for supported platforms. If set to <c>false</c>,
    /// it throws a <see cref="PlatformNotSupportedException"/> if the operation is attempted on non-Windows platforms.
    /// </param>
    /// <exception cref="PlatformNotSupportedException">
    /// Thrown when the method is invoked on a platform other than Windows and the
    /// <paramref name="safe"/> parameter is set to <c>false</c>.
    /// </exception>
    internal static void Mute(bool safe = true) {
        if (safe) {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                WinImports.SetStdErr(IntPtr.Zero);   
            
            return;
        }
        
        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            throw new PlatformNotSupportedException("Exception muting supported only on Windows.");
        
        WinImports.SetStdErr(IntPtr.Zero);
    }
}