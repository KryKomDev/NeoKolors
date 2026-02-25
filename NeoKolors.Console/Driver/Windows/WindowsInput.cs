//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NeoKolors.Console.Driver.Windows;

/// <summary>
/// Represents a utility class for interacting with Windows console input handles.
/// Performs operations related to retrieving and managing standard input handles in a Windows environment.
/// </summary>
internal class WindowsInput : IDisposable {
    
    private static readonly NKLogger LOGGER = NKDebug.GetLogger<WindowsInput>();
    
    private readonly WinImports.WinVtInModes _originalModes;
    private readonly nint _handle;
    private bool _disposed = false;

    public WindowsInput() {
        _handle = WinImports.GetStdIn();

        // handle some cases of the handle
        switch (_handle) {
            case -1: {
                Win32Exception.Throw(WinImports.GetLastError());
            } return;
            case 0: {
                LOGGER.Crit("No associated stdin handle available.");
            } return;
        }

        if (!WinImports.GetStdInMode(_handle, out _originalModes))
            Win32Exception.ThrowLast();

        // configure the modes
        var modes = (_originalModes 
            |  WinImports.WinVtInModes.ENABLE_MOUSE_INPUT 
            |  WinImports.WinVtInModes.ENABLE_EXTENDED_FLAGS)
            & ~WinImports.WinVtInModes.ENABLE_QUICK_EDIT_MODE
            & ~WinImports.WinVtInModes.ENABLE_PROCESSED_INPUT;
        
        if (!WinImports.SetStdInMode(_handle, modes))
            Win32Exception.ThrowLast();
    }

    /// <summary>
    /// Determines whether there are pending input events in the standard input buffer.
    /// </summary>
    /// <returns>
    /// A boolean value indicating whether input events are available in the console's input buffer.
    /// Returns true if there are one or more input events available for processing; otherwise, false.
    /// </returns>
    public bool HasInput() {
        const int read = 1;
        nint buff = Marshal.AllocHGlobal(Marshal.SizeOf<WinImports.WinInputRecord>() * read);
        
        if (!WinImports.PeekConsoleInput(_handle, buff, read, out var eventsRead))
            LOGGER.Error($"Failed to peek stdin buffer. Exit code: {WinImports.GetLastError()}");
        
        Marshal.FreeHGlobal(buff);
        
        return eventsRead > 0;
    }

    /// <summary>
    /// Reads a single input event from the standard input buffer of the Windows console.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="WinImports.WinInputRecord"/> representing the input events read from the standard input buffer.
    /// If no input events are available, returns an empty collection.
    /// </returns>
    public IEnumerable<WinImports.WinInputRecord> ReadInput() {
        const int read = 1;
        nint buff = Marshal.AllocHGlobal(Marshal.SizeOf<WinImports.WinInputRecord>() * read);

        try {
            if (!WinImports.ReadConsoleInput(_handle, buff, read, out var eventsRead))
                LOGGER.Error($"Failed to read stdin buffer. Exit code: {WinImports.GetLastError()}");

            return eventsRead == 0 
                ? [] 
                : [Marshal.PtrToStructure<WinImports.WinInputRecord>(buff)];
        }
        finally {
            Marshal.FreeHGlobal(buff);
        }
    }

    /// <summary>
    /// Attempts to retrieve input events from the console's input buffer.
    /// </summary>
    /// <param name="input">When this method returns, contains the input event records
    /// retrieved from the console's input buffer, if available; otherwise, an empty collection.</param>
    /// <returns>
    /// A boolean value indicating whether input events were successfully retrieved.
    /// Returns true if one or more input events are available and retrieved; otherwise, false.
    /// </returns>
    public bool TryGetInput(out IEnumerable<WinImports.WinInputRecord> input) {
        const int read = 1;
        nint buff = Marshal.AllocHGlobal(Marshal.SizeOf<WinImports.WinInputRecord>() * read);

        try {
            if (!WinImports.ReadConsoleInput(_handle, buff, read, out var eventsRead))
                LOGGER.Error($"Failed to read stdin buffer. Exit code: {WinImports.GetLastError()}");

            if (eventsRead > 0) {
                input = [Marshal.PtrToStructure<WinImports.WinInputRecord>(buff)];
                return true;
            }

            input = [];
            return false;
        }
        finally {
            Marshal.FreeHGlobal(buff);
        }
    }
    
    public void Dispose() {
        if (_disposed) return;
        
        _disposed = true;

        if (!WinImports.FlushConsoleInputBuffer(_handle)) {
            throw new Win32Exception(
                $"Failed to flush stdin buffer. Exit code: {WinImports.GetLastError()}, {Marshal.GetLastWin32Error()}"
            );   
        }

        WinImports.SetStdInMode(_handle, _originalModes);
    }
}