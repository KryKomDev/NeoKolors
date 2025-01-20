//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;

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
    
    private static int FATAL_COLOR = Palette.Colors[0];
    private static int ERROR_COLOR = Palette.Colors[1];
    private static int WARN_COLOR = Palette.Colors[2];
    private static int INFO_COLOR = Palette.Colors[3];
    private static int DEBUG_COLOR = Palette.Colors[4];
    
    /// <summary>
    /// color of the fatal messages
    /// </summary>
    public static int FatalColor {
        get => FATAL_COLOR;
        set {
            FATAL_COLOR = value;
            Palette.Colors[0] = value;
        }
    }

    /// <summary>
    /// color of the error messages
    /// </summary>
    public static int ErrorColor {
        get => ERROR_COLOR;
        set {
            ERROR_COLOR = value;
            Palette.Colors[1] = value;
        }
    }

    /// <summary>
    /// color of the warning messages
    /// </summary>
    public static int WarnColor {
        get => WARN_COLOR;
        set {
            WARN_COLOR = value;
            Palette.Colors[2] = value;
        }
    }

    /// <summary>
    /// color of the info messages
    /// </summary>
    public static int InfoColor {
        get => INFO_COLOR;
        set {
            INFO_COLOR = value;
            Palette.Colors[3] = value;
        }
    }

    /// <summary>
    /// color of the debug messages
    /// </summary>
    public static int DebugColor {
        get => DEBUG_COLOR;
        set {
            DEBUG_COLOR = value;
            Palette.Colors[4] = value;
        }
    }

    /// <summary>
    /// what will be shown in the console <br/>
    /// 0 - nothing <br/>
    /// 1 - error <br/>
    /// 2 - error + warn <br/>
    /// 3 - error + warn + info <br/>
    /// </summary>
    public static DebugLevel Level { get; set; } = DebugLevel.ERRORS_WARNS;

    public enum DebugLevel {
        
        /// <summary>
        /// only debug messages will be shown
        /// </summary>
        DEBUG_ONLY = -1,
        
        /// <summary>
        /// no log messages will be shown
        /// </summary>
        NOTHING = 0,
        
        /// <summary>
        /// only error messages will be shown
        /// </summary>
        ONLY_ERRORS = 1,
        
        /// <summary>
        /// error and warning messages will be shown
        /// </summary>
        ERRORS_WARNS = 2,
        
        /// <summary>
        /// all log messages will be shown
        /// </summary>
        ALL = 3,
    }
    
    /// <summary>
    /// whether to show the highlight when throwing a fancy exception
    /// </summary>
    public static bool ShowHighlight { get; set; } = true;
    
    /// <summary>
    /// the highlight color of a fancy exception
    /// </summary>
    public static Color HighlightColor { get; set; } = new(ConsoleColor.DarkRed);
    
    /// <summary>
    /// the color of the exception type shown when a fancy exception is thrown
    /// </summary>
    public static Color NameColor { get; set; } = new(ConsoleColor.Yellow);
    
    /// <summary>
    /// the color of the exception message shown when a fancy exception is thrown
    /// </summary>
    public static Color MessageColor { get; set; } = new(ConsoleColor.Red);
    
    /// <summary>
    /// the color of the filename printed as a part of the stack trace shown when a fancy exception is thrown
    /// </summary>
    public static Color FileNameColor { get; set; } = new(ConsoleColor.Blue);
    
    /// <summary>
    /// the color of the path to a source file printed as a part of the stack trace shown when a fancy exception is
    /// thrown
    /// </summary>
    public static Color PathColor { get; set; } = new(ConsoleColor.Gray);
    
    /// <summary>
    /// whether the path to a source file printed as a part of the stack trace shown when a fancy exception is thrown
    /// should be faint
    /// </summary>
    public static bool FaintPath { get; set; } = false;
    
    /// <summary>
    /// the color of the method name printed as a part of the stack trace shown when a fancy exception is thrown
    /// </summary>
    public static Color MethodColor { get; set; } = new(ConsoleColor.Blue);
    
    /// <summary>
    /// the color of the namespace printed as a part of the stack trace shown when a fancy exception is thrown
    /// </summary>
    public static Color NamespaceColor { get; set; } = new(ConsoleColor.Gray);
    
    /// <summary>
    /// whether the namespace printed as a part of the stack trace shown when
    /// </summary>
    public static bool FaintNamespace { get; set; } = false;
    
    /// <summary>
    /// the color of the line number printed as a part of the stack trace shown when a fancy exception is thrown
    /// </summary>
    public static Color LineNumberColor { get; set; } = new(ConsoleColor.Green);
}