//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static NeoKolors.Console.Driver.Windows.WinImports;

namespace NeoKolors.Console.Driver.Windows;

/// <summary>
/// Represents a utility class for interacting with Windows console input handles.
/// Performs operations related to retrieving and managing standard input handles in a Windows environment.
/// </summary>
internal class WindowsInput : IDisposable {
    
    private static readonly NKLogger LOGGER = NKDebug.GetLogger<WindowsInput>();
    
    private readonly WinVtInModes _originalModes;
    private readonly nint _handle;
    private bool _disposed = false;

    public bool IsEnabled { get; private set; }
    
    public WinVtInModes Modes => GetModes();
    
    public WindowsInput() {
        _handle = GetStdIn();

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
        if (!GetStdInMode(_handle, out _originalModes))
            Win32Exception.ThrowLast();

        const WinVtInModes enabled = 0
            | WinVtInModes.ENABLE_MOUSE_INPUT
            | WinVtInModes.ENABLE_EXTENDED_FLAGS;

        const WinVtInModes disabled = 0
            | WinVtInModes.ENABLE_QUICK_EDIT_MODE
            | WinVtInModes.ENABLE_PROCESSED_INPUT;
        
        // configure VT the modes
        var modes = (_originalModes | enabled) & ~disabled;
        
        // set the VT modes
        if (!SetStdInMode(_handle, modes))
            Win32Exception.ThrowLast();
    }

    /// <summary>
    /// Updates the standard input mode of the console to the specified configuration.
    /// </summary>
    /// <param name="modes">
    /// A set of flags representing the desired console input modes. The modes define specific behaviors
    /// for handling input, such as enabling virtual terminal input or mouse input.
    /// </param>
    /// <exception cref="Win32Exception">
    /// Thrown when the operation to set the console's input mode fails.
    /// </exception>
    internal void SetModes(WinVtInModes modes) {
        if (!SetStdInMode(_handle, modes))
            Win32Exception.ThrowLast();
    }

    /// <summary>
    /// Retrieves the current input mode of the console's standard input handle.
    /// </summary>
    /// <param name="mode">
    /// An output parameter that, on successful completion, contains a set of flags
    /// representing the current console input modes. These flags define specific
    /// behaviors for handling input, such as processing line input or enabling mouse input.
    /// </param>
    /// <exception cref="Win32Exception">
    /// Thrown when the operation to retrieve the current input mode fails.
    /// </exception>
    public void GetModes(out WinVtInModes mode) {
        if (!GetStdInMode(_handle, out mode))
            Win32Exception.ThrowLast();
    }
    
    /// <summary>
    /// Retrieves the current input mode of the console's standard input handle.
    /// </summary>
    /// <returns>
    /// A set of flags representing the current console input modes. These flags define specific
    /// behaviors for handling input, such as processing line input or enabling mouse input.
    /// </returns>
    /// <exception cref="Win32Exception">
    /// Thrown when the operation to retrieve the current input mode fails.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WinVtInModes GetModes() {
        GetModes(out var mode);
        return mode;
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
        nint buff = Marshal.AllocHGlobal(Marshal.SizeOf<WinInputRecord>() * read);
        
        if (!PeekConsoleInput(_handle, buff, read, out var eventsRead))
            LOGGER.Error($"Failed to peek stdin buffer. Exit code: {GetLastError()}");
        
        Marshal.FreeHGlobal(buff);
        
        return eventsRead > 0;
    }

    /// <summary>
    /// Retrieves the number of unread input events currently available in the console input buffer.
    /// </summary>
    /// <returns>
    /// The count of unread input events in the console input buffer as an unsigned 32-bit integer.
    /// </returns>
    /// <exception cref="Win32Exception">
    /// Thrown when there is a failure in retrieving the count of input events from the console.
    /// </exception>
    public uint GetUnreadCount() {
        GetNumberOfConsoleInputEvents(_handle, out var count);
        return count;
    }

    /// <summary>
    /// Reads a single input event from the standard input buffer of the Windows console.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="WinImports.WinInputRecord"/> representing the input events read from the standard input buffer.
    /// If no input events are available, returns an empty collection.
    /// </returns>
    public WinInputRecord? ReadInput() {
        const int read = 1;
        nint buff = Marshal.AllocHGlobal(Marshal.SizeOf<WinInputRecord>() * read);

        try {
            if (!ReadConsoleInput(_handle, buff, read, out var eventsRead))
                LOGGER.Error($"Failed to read stdin buffer. Exit code: {GetLastError()}");

            return eventsRead == 0 
                ? default 
                : Marshal.PtrToStructure<WinInputRecord>(buff);
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
    public unsafe bool TryGetInputs(out Span<WinInputRecord> input) {
        GetNumberOfConsoleInputEvents(_handle, out var pendingCount);
        
        int read = checked((int)pendingCount);
        
        nint buff = Marshal.AllocHGlobal(Marshal.SizeOf<WinInputRecord>() * read);

        try {
            if (!ReadConsoleInput(_handle, buff, pendingCount, out var eventsRead))
                LOGGER.Error($"Failed to read stdin buffer. Exit code: {GetLastError()}");

            if (eventsRead > 0) {
                input = new Span<WinInputRecord>((void*)buff, (int)eventsRead);
                return true;
            }

            input = [];
            return false;
        }
        finally {
            Marshal.FreeHGlobal(buff);
        }
    }

    /// <summary>
    /// Attempts to retrieve a single input event from the console input buffer.
    /// </summary>
    /// <param name="input">
    /// When this method returns, contains the retrieved input event record if successful; otherwise, null.
    /// </param>
    /// <returns>
    /// True if an input event was successfully retrieved; otherwise, false.
    /// </returns>
    public bool TryGetInput([NotNullWhen(true)] out WinInputRecord? input) {
        const int read = 1;
        nint buff = Marshal.AllocHGlobal(Marshal.SizeOf<WinInputRecord>() * read);

        try {
            if (!ReadConsoleInput(_handle, buff, read, out var eventsRead))
                LOGGER.Error($"Failed to read stdin buffer. Exit code: {GetLastError()}");

            if (eventsRead > 0) {
                input = Marshal.PtrToStructure<WinInputRecord>(buff);
                return true;
            }

            input = null;
            return false;
        }
        finally {
            Marshal.FreeHGlobal(buff);
        }
    }
    
    public void Dispose() {
        if (_disposed) return;
        
        _disposed = true;

        if (!FlushConsoleInputBuffer(_handle)) {
            throw new Win32Exception(
                $"Failed to flush stdin buffer. Exit code: {GetLastError()}, {Marshal.GetLastWin32Error()}"
            );   
        }

        SetStdInMode(_handle, _originalModes);
    }
}