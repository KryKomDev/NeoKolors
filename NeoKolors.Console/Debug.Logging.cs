//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using static NeoKolors.Common.EscapeCodes;

namespace NeoKolors.Console;

/// <summary>
/// debug console utils 
/// </summary>
public static partial class Debug {
    
    /// <summary>
    /// prints red error text
    /// </summary>
    /// <param name="s">desired string message</param>
    public static void Fatal(string s) {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (!Level.HasFlag(DebugLevel.FATAL)) return;

        if (SimpleMessages) {
            Output.WriteLine(HideTime
                ? $"[ FATAL ] : {s}"
                : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ FATAL ] : {s}");
        }
        else {
            if (IsTerminalPaletteSafe) {
                Output.WriteLine(HideTime
                    ?  $"<f-dark-red><b><f-black><b-dark-red> FATAL <f-dark-red></b-color></b> : {s}</f-color>"
                        .ApplyStyles().ApplyColors()
                    : ($"<f-dark-red>[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] " +
                       $"<b><f-black><b-dark-red> FATAL <f-dark-red></b-color></b> : {s}</f-color>")
                        .ApplyStyles().ApplyColors());
            }
            else {
                Output.WriteLine(HideTime
                    ? ($"{FATAL_COLOR.ControlChar}<b><f-black>{FATAL_COLOR.ControlCharB} FATAL {FATAL_COLOR.ControlChar}" +
                       $"{CUSTOM_BACKGROUND_COLOR_END}</b> : {s}").ApplyStyles().ApplyColors()
                    : ($"{FATAL_COLOR.ControlChar}[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] " +
                       $"{CUSTOM_BACKGROUND_COLOR_END}{FATAL_COLOR.ControlCharB} FATAL {FATAL_COLOR.ControlChar}" +
                       $"{CUSTOM_BACKGROUND_COLOR_END}</b> : {s}").ApplyStyles().ApplyColors());
            }
        }
        
        Output.Flush();
    }
    
    /// <summary>
    /// prints red fatal error text using the ToString method of the object o and <see cref="Fatal(string)"/>
    /// </summary>
    public static void Fatal(object o) => Fatal(o.ToString()!);

    /// <summary>
    /// prints red error text
    /// </summary>
    /// <param name="s">desired string message</param>
    public static void Error(string s) {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (!Level.HasFlag(DebugLevel.ERROR)) return;

        if (SimpleMessages) {
            Output.WriteLine(HideTime
                ? $"[ ERROR ] : {s}"
                : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ ERROR ] : {s}");
        }
        else {
            if (IsTerminalPaletteSafe) {
                Output.WriteLine(HideTime
                    ?  $"<f-red><f-black><b-red> ERROR <f-red></b-color> : {s}</f-color>".ApplyColors()
                    : ($"<f-red>[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}]" +
                       $" <f-black><b-red> ERROR <f-red></b-color> : {s}</f-color>").ApplyColors());
            }
            else {
                Output.WriteLine(HideTime
                    ? ($"{ERROR_COLOR.ControlChar}<f-black>{ERROR_COLOR.ControlCharB} ERROR {ERROR_COLOR.ControlChar}" +
                       $"{CUSTOM_BACKGROUND_COLOR_END} : {s}</f-color>").ApplyColors()
                    : ($"{ERROR_COLOR.ControlChar}[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] " +
                       $"{CUSTOM_BACKGROUND_COLOR_END}{ERROR_COLOR.ControlCharB} ERROR {ERROR_COLOR.ControlChar}" +
                       $"{CUSTOM_BACKGROUND_COLOR_END} : {s}</f-color>").ApplyColors());
            }
        }
        
        Output.Flush();
    }
    
    /// <summary>
    /// prints red error text using the ToString method of the object o and <see cref="Error(string)"/>
    /// </summary>
    public static void Error(object o) => Error(o.ToString()!);

    /// <summary>
    /// prints yellow warning text
    /// </summary>
    /// <param name="s">desired string message</param>
    public static void Warn(string s) {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (!Level.HasFlag(DebugLevel.WARN)) return;
        
        if (SimpleMessages) {
            Output.WriteLine(HideTime
                ? $"[ WARN ] : {s}"
                : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ WARN ] : {s}");
        }
        else {
            Output.WriteLine((HideTime
                    ? $"[ WARN ] : {s}"
                    : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ WARN ] : {s}")
                .AddColor((NKColor)(IsTerminalPaletteSafe ? NKConsoleColor.YELLOW : WARN_COLOR)));
        }
        
        Output.Flush();
    }
    
    /// <summary>
    /// prints yellow warning text using the ToString method of the object o and <see cref="Warn(string)"/>
    /// </summary>
    public static void Warn(object o) => Warn(o.ToString()!);
    
    /// <summary>
    /// prints green info text
    /// </summary>
    /// <param name="s">desired string message</param>
    public static void Info(string s) {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (!Level.HasFlag(DebugLevel.INFO)) return;

        if (SimpleMessages) {
            Output.WriteLine(HideTime
                ? $"[ INFO ] : {s}"
                : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ INFO ] : {s}");
        }
        else {
            Output.WriteLine((HideTime
                    ? $"[ INFO ] : {s}"
                    : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ INFO ] : {s}")
                .AddColor((NKColor)(IsTerminalPaletteSafe ? NKConsoleColor.GREEN : INFO_COLOR)));
        }
        
        Output.Flush();
    }

    /// <summary>
    /// prints green info text using the ToString method of the object o and <see cref="Info(string)"/>
    /// </summary>
    public static void Info(object o) => Info(o.ToString()!);

    /// <summary>
    /// prints a debug message, does not work when built in release mode
    /// </summary>
    /// <param name="s">desired string message</param>
    public static void Log(string s) {
        if (!Level.HasFlag(DebugLevel.DEBUG)) return;
        if (s == null) throw new ArgumentNullException(nameof(s));

        if (SimpleMessages) {
            Output.WriteLine(HideTime
                ? $"[ DEBUG ] : {s}"
                : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ DEBUG ] : {s}");
        }
        else {
            Output.WriteLine((HideTime
                    ? $"[ DEBUG ] : {s}"
                    : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ DEBUG ] : {s}")
                .AddColor((NKColor)(IsTerminalPaletteSafe ? NKConsoleColor.BLUE : DEBUG_COLOR)));
        }
        
        Output.Flush();
    }

    /// <summary>
    /// prints debug text using the ToString method of the object o and <see cref="Log(string)"/>
    /// </summary>
    public static void Log(object o) => Log(o.ToString()!);
}