//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using static NeoKolors.Console.Debug.DebugLevel;

namespace NeoKolors.Console;

/// <summary>
/// settings of the debug console
/// </summary>
public static partial class Debug {

    /// <summary>
    /// sets the console output encoding to UTF-8
    /// </summary>
    static Debug() => System.Console.OutputEncoding = System.Text.Encoding.UTF8;

    /// <summary>
    /// the default custom color palette
    /// </summary>
    public static readonly ColorPalette DEFAULT_PALETTE = new("a11d3c-ef476f-ffd166-06d6a0-0aabe1-9b4f98");

    /// <summary>
    /// custom palette
    /// </summary>
    public static ColorPalette Palette { get; set; } = DEFAULT_PALETTE;

    /// <summary>
    /// if enabled the console will use the terminal colors
    /// </summary>
    public static bool IsTerminalPaletteSafe { get; set; } = true;

    /// <summary>
    /// if true does not show the timestamp in the log messages
    /// </summary>
    public static bool HideTime { get; set; } = false;

    /// <summary>
    /// the output stream of the logger
    /// </summary>
    public static TextWriter Output { get; set; } = System.Console.Out;

    /// <summary>
    /// if true displays simple log messages instead of the colorful nice ones
    /// (turning on is recommended when not outputting to console)
    /// </summary>
    public static bool SimpleMessages { get; set; } = false;

    private static NKColor FATAL_COLOR = Palette.Colors[0];
    private static NKColor ERROR_COLOR = Palette.Colors[1];
    private static NKColor WARN_COLOR = Palette.Colors[2];
    private static NKColor INFO_COLOR = Palette.Colors[3];
    private static NKColor DEBUG_COLOR = Palette.Colors[4];

    /// <summary>
    /// color of the fatal messages
    /// </summary>
    public static NKColor FatalColor {
        get => FATAL_COLOR;
        set {
            FATAL_COLOR = value;
            Palette.Colors[0] = value;
        }
    }

    /// <summary>
    /// color of the error messages
    /// </summary>
    public static NKColor ErrorColor {
        get => ERROR_COLOR;
        set {
            ERROR_COLOR = value;
            Palette.Colors[1] = value;
        }
    }

    /// <summary>
    /// color of the warning messages
    /// </summary>
    public static NKColor WarnColor {
        get => WARN_COLOR;
        set {
            WARN_COLOR = value;
            Palette.Colors[2] = value;
        }
    }

    /// <summary>
    /// color of the info messages
    /// </summary>
    public static NKColor InfoColor {
        get => INFO_COLOR;
        set {
            INFO_COLOR = value;
            Palette.Colors[3] = value;
        }
    }

    /// <summary>
    /// color of the debug messages
    /// </summary>
    public static NKColor DebugColor {
        get => DEBUG_COLOR;
        set {
            DEBUG_COLOR = value;
            Palette.Colors[4] = value;
        }
    }

    /// <summary>
    /// determines what log messages will be shown
    /// </summary>
    public static DebugLevel Level { get; set; } = FATAL | ERROR | WARN | INFO | DEBUG;

    [Flags]
    public enum DebugLevel {
        FATAL = 1,
        ERROR = 2,
        WARN = 4,
        INFO = 8,
        DEBUG = 16
    }

    /// <summary>
    /// turns all log messages on
    /// </summary>
    public static void LogAll() => Level = FATAL | ERROR | WARN | INFO | DEBUG;

    /// <summary>
    /// turns on logging of errors and warns
    /// </summary>
    public static void LogWarns() => Level = FATAL | ERROR | WARN;

    /// <summary>
    /// turns on logging of errors
    /// </summary>
    public static void LogErrors() => Level = FATAL | ERROR;

    /// <summary>
    /// turns all log messages off
    /// </summary>
    public static void LogNone() => Level = 0;

    
    // --- settings for exception fancy ---
    
    /// <summary>
    /// whether to show the highlight when throwing a fancy exception
    /// </summary>
    public static bool ShowHighlight { get; set; } = true;
    
    /// <summary>
    /// the highlight color of a fancy exception
    /// </summary>
    public static NKColor HighlightColor { get; set; } = new(ConsoleColor.DarkRed);
    
    /// <summary>
    /// the color of the exception type shown when a fancy exception is thrown
    /// </summary>
    public static NKColor ExceptionNameColor { get; set; } = new(ConsoleColor.Yellow);
    
    /// <summary>
    /// the color of the exception message shown when a fancy exception is thrown
    /// </summary>
    public static NKColor MessageColor { get; set; } = new(ConsoleColor.Red);
    
    /// <summary>
    /// the color of the filename printed as a part of the stack trace shown when a fancy exception is thrown
    /// </summary>
    public static NKColor FileNameColor { get; set; } = new(ConsoleColor.Blue);
    
    /// <summary>
    /// the color of the path to a source file printed as a part of the stack trace shown when a fancy exception is
    /// thrown
    /// </summary>
    public static NKColor PathColor { get; set; } = new(ConsoleColor.Gray);
    
    /// <summary>
    /// whether the path to a source file printed as a part of the stack trace shown when a fancy exception is thrown
    /// should be faint
    /// </summary>
    public static bool FaintPath { get; set; } = false;
    
    /// <summary>
    /// the color of the method name printed as a part of the stack trace shown when a fancy exception is thrown
    /// </summary>
    public static NKColor MethodColor { get; set; } = new(ConsoleColor.Blue);
    
    /// <summary>
    /// the color of the namespace printed as a part of the stack trace shown when a fancy exception is thrown
    /// </summary>
    public static NKColor NamespaceColor { get; set; } = new(ConsoleColor.Gray);
    
    /// <summary>
    /// whether the namespace printed as a part of the stack trace shown when
    /// </summary>
    public static bool FaintNamespace { get; set; } = false;
    
    /// <summary>
    /// the color of the line number printed as a part of the stack trace shown when a fancy exception is thrown
    /// </summary>
    public static NKColor LineNumberColor { get; set; } = new(ConsoleColor.Green);

    /// <summary>
    /// if true makes the exception name italic
    /// </summary>
    public static bool ItalicExceptionName { get; set; } = true;

    /// <summary>
    /// if true makes the method names in exception stack trace italic
    /// </summary>
    public static bool ItalicMethodName { get; set; } = true;

    /// <summary>
    /// if true makes all unhandled exceptions look fancy
    /// </summary>
    public static bool EnableAutoFancy { 
        get;
        set {
            if (value)
                AppDomain.CurrentDomain.UnhandledException += WriteUnhandled;
            else
                AppDomain.CurrentDomain.UnhandledException -= WriteUnhandled;

            field = value;
        } 
    } = true;
}