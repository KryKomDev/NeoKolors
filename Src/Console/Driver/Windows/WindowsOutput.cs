// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.ComponentModel;
using static NeoKolors.Console.Driver.Windows.WinImports;

namespace NeoKolors.Console.Driver.Windows;

/// <summary>
/// Represents a Windows-specific implementation for managing standard output
/// with capabilities for configuring and handling virtual terminal modes.
/// </summary>
public class WindowsOutput : IDisposable {
    
    private static readonly NKLogger LOGGER = NKDebug.GetLogger<WindowsOutput>();

    private          bool          _disposed = false;
    private readonly nint          _handle;
    private readonly WinVtOutModes _originalModes;

    public WindowsOutput() {
        _handle = GetStdOut();

        // handle some cases of the handle
        switch (_handle) {
            case -1: {
                Win32Exception.Throw(GetLastError());
            } return;
            case 0: {
                LOGGER.Crit("No associated stdin handle available.");
            } return;
        }

        // store the original VT modes to be restored on this instance disposal
        if (!GetStdOutMode(_handle, out _originalModes))
            Win32Exception.ThrowLast();

        const WinVtOutModes enabled = 0
            | WinVtOutModes.ENABLE_VIRTUAL_TERMINAL_PROCESSING
            | WinVtOutModes.ENABLE_PROCESSED_OUTPUT;

        const WinVtOutModes disabled = 0
            | WinVtOutModes.ENABLE_WRAP_AT_EOL_OUTPUT;
        
        // configure VT the modes
        var modes = (_originalModes | enabled) & ~disabled;
        
        // set the VT modes
        if (!SetStdOutMode(_handle, modes))
            Win32Exception.ThrowLast();
    }

    /// <summary>
    /// Writes the specified text to the console output using the configured handle.
    /// </summary>
    /// <param name="text">The text to be written to the console output.</param>
    public void Write(ReadOnlySpan<char> text) {

        if (!WriteConsole(_handle, text, out var written))
            Win32Exception.ThrowLast();

        var totalWritten = written;
        
        while (totalWritten < text.Length) {
            if (!WriteConsole(_handle, text[unchecked((int)totalWritten)..], out written))
                Win32Exception.ThrowLast();
            
            totalWritten += written;
        }
    }
    
    public void Dispose() {
        if (_disposed) 
            return;
        
        _disposed = true;
        SetStdOutMode(_handle, _originalModes);
    }
}