// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

#define NK_NKCONSOLE_PROFILING

using System.Diagnostics;
using Metriks;
using static NeoKolors.Common.EscapeCodes;

namespace NeoKolors.Console;

public static partial class NKConsole {

    #region WINDOW MANIPULATION

    /// <summary>
    /// Minimizes the console window by sending an escape sequence to iconify the window.
    /// </summary>
    public static void Minimize() => OutputDriver.Write(ICONIFY_WINDOW);

    /// <summary>
    /// Restores the console window from a minimized state to its normal size.
    /// </summary>
    public static void Maximize() => OutputDriver.Write(DEICONIFY_WINDOW);

    #endregion

    #region DECSET / DECRST

    /// <summary>
    /// Sets the specified DEC private modes for the terminal.
    /// </summary>
    /// <param name="modes">An array of DEC private modes to enable.</param>
    public static void SetModes(params DecMode[] modes) => OutputDriver.Write(GetDecSet(modes));

    /// <summary>
    /// Resets the specified DEC Private Modes in the terminal.
    /// </summary>
    /// <param name="modes">An array of DEC Private Modes to reset.</param>
    public static void ResetModes(params DecMode[] modes) => OutputDriver.Write(GetDecRst(modes));

    /// <summary>
    /// Saves the current position of the console cursor.
    /// The cursor's state can be restored later using a corresponding restore method.
    /// </summary>
    public static void SaveCursor() => OutputDriver.Write(SAVE_CURSOR_STATE);

    /// <summary>
    /// Restores the cursor to its previously saved position in the console.
    /// </summary>
    public static void RestoreCursor() => OutputDriver.Write(RESTORE_CURSOR_STATE);

    /// <summary>
    /// Hides the cursor in the console window.
    /// </summary>
    public static void HideCursor() => OutputDriver.Write(DISABLE_CURSOR_VISIBLE);

    /// <summary>
    /// Enables the visibility of the cursor in the console.
    /// </summary>
    public static void ShowCursor() => OutputDriver.Write(ENABLE_CURSOR_VISIBLE);

    #endregion
    
    #if NK_NKCONSOLE_PROFILING
    private static          ulong     CSR_POS_COUNT  = 0;
    private static readonly Stopwatch CSR_POS_BOUNDS = new();
    private static readonly Stopwatch CSR_POS_SET    = new();

    public static TimeSpan CursorPosition_BoundsCheckTime    => CSR_POS_BOUNDS.Elapsed;
    public static TimeSpan CursorPosition_SetPosTime         => CSR_POS_SET   .Elapsed;
    public static TimeSpan CursorPosition_AvgBoundsCheckTime => CSR_POS_BOUNDS.Elapsed / CSR_POS_COUNT;
    public static TimeSpan CursorPosition_AvgSetPosTime      => CSR_POS_SET   .Elapsed / CSR_POS_COUNT;
    #endif
    
    /// <summary>
    /// Attempts to set the cursor position in the console window to the specified coordinates.
    /// </summary>
    /// <param name="x">The horizontal position of the cursor, measured in columns.</param>
    /// <param name="y">The vertical position of the cursor, measured in rows.</param>
    /// <returns>Returns true if the cursor position was successfully set; otherwise, false.</returns>
    public static bool TrySetCursorPosition(int x, int y) {

        #if NK_NKCONSOLE_PROFILING
        CSR_POS_COUNT++;
        CSR_POS_BOUNDS.Start();
        #endif

        var s = InputDriver.GetSize();
        if (!(x >= 0 && x < s.X && y >= 0 && y < s.Y)) 
            return false;

        #if NK_NKCONSOLE_PROFILING
        CSR_POS_BOUNDS.Stop();
        CSR_POS_SET.Start();
        #endif
        
        try {
            Stdio.SetCursorPosition(x, y);
        }
        catch (Exception) {
            LOGGER.Error("Tried to set cursor position outside of bounds.");
            return false;
        }

        #if NK_NKCONSOLE_PROFILING
        CSR_POS_SET.Stop();
        #endif
        
        return true;
    }
    
    public static Size2D WindowSize => new(Stdio.WindowWidth, Stdio.WindowHeight);
    public static Size2D BufferSize => InputDriver.GetSize();
    
}