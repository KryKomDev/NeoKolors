//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Console;

/// <summary>
/// debug console utils 
/// </summary>
public static partial class Debug {
    
    /// <summary>
    /// prints red error text
    /// </summary>
    /// <param name="s">desired string message</param>
    /// <param name="hideTime">hides time if true</param>
    public static void Fatal(string s, bool hideTime = false) {
        if (Level <= DebugLevel.NOTHING) return;

        if (IsTerminalPaletteSafe) {
            ConsoleColors.PrintColored(hideTime ? "" : $"\e[1m[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] ", ConsoleColor.DarkRed);
            ConsoleColors.PrintColoredB("\e[1m\e[38;2;36;36;36m[ FATAL ]", ConsoleColor.DarkRed);
            ConsoleColors.PrintColored($"\e[1m : {s}\n", ConsoleColor.DarkRed);
        }
        else {
            ConsoleColors.PrintColored(hideTime ? "" : $"\e[1m[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] ", FATAL_COLOR);
            ConsoleColors.PrintColoredB("\e[1m\e[38;2;36;36;36m[ FATAL ]", FATAL_COLOR);
            ConsoleColors.PrintColored($"\e[1m : {s}\n", FATAL_COLOR);
        }
    }
    
    /// <summary>
    /// prints red error text
    /// </summary>
    /// <param name="s">desired string message</param>
    /// <param name="hideTime">hides time if true</param>
    public static void Error(string s, bool hideTime = false) {
        if (Level <= DebugLevel.NOTHING) return;

        if (IsTerminalPaletteSafe) {
            ConsoleColors.PrintColored(hideTime ? "" : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] ", ConsoleColor.Red);
            ConsoleColors.PrintColoredB("\e[1m\e[38;2;36;36;36m[ ERROR ]", ConsoleColor.Red);
            ConsoleColors.PrintColored($" : {s}\n", ConsoleColor.Red);
        }
        else {
            ConsoleColors.PrintColored(hideTime ? "" : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] ", ERROR_COLOR);
            ConsoleColors.PrintColoredB("\e[1m\e[38;2;36;36;36m[ ERROR ]", ERROR_COLOR);
            ConsoleColors.PrintColored($" : {s}\n", ERROR_COLOR);
        }
    }

    /// <summary>
    /// prints yellow warning text
    /// </summary>
    /// <param name="s">desired string message</param>
    /// <param name="hideTime">hides time if true</param>
    public static void Warn(string s, bool hideTime = false) {
        if (Level < DebugLevel.ERRORS_WARNS) return;
        
        if (IsTerminalPaletteSafe) 
            ConsoleColors.PrintColored(
                hideTime 
                    ? $"[ WARN ] : {s}\n" 
                    : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ WARN ] : {s}\n", ConsoleColor.Yellow);
        else
            ConsoleColors.PrintColored(
                hideTime
                    ? $"[ WARN ] : {s}\n"
                    : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ WARN ] : {s}\n", WARN_COLOR);
    }
    
    /// <summary>
    /// prints green info text
    /// </summary>
    /// <param name="s">desired string message</param>
    /// <param name="hideTime">hides time if true</param>
    public static void Info(string s, bool hideTime = false) {
        if (Level < DebugLevel.ALL) return;

        if (IsTerminalPaletteSafe) {
            ConsoleColors.PrintColored(
                hideTime 
                    ? $"[ INFO ] : {s}\n" 
                    : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ INFO ] : {s}\n", ConsoleColor.Green);
        }
        else {
            ConsoleColors.PrintColored(
                hideTime 
                    ? $"[ INFO ] : {s}\n" 
                    : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ INFO ] : {s}\n", INFO_COLOR);

        }
    }
    
    /// <summary>
    /// prints a debug message, does not work when built in release mode
    /// </summary>
    /// <param name="s">desired string message</param>
    /// <param name="hideTime">hides time if true</param>
    public static void Msg(string s, bool hideTime = false) {
        #if !DEBUG
        if (Level == DebugLevel.NOTHING) return;

        if (IsTerminalPaletteSafe) {
            ConsoleColors.PrintColored(
                hideTime 
                    ? $"[ DEBUG ] : {s}\n" 
                    : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ DEBUG ] : {s}\n", ConsoleColor.Blue);
        }
        else {
            ConsoleColors.PrintColored(
                hideTime 
                    ? $"[ DEBUG ] : {s}\n" 
                    : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ DEBUG ] : {s}\n", DEBUG_COLOR);
        }
        #endif
    }
}