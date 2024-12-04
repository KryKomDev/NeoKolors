//
// NeoKolors
// by KryKom 2024
//

using NeoKolors.Common;

namespace NeoKolors.Console;

/// <summary>
/// debug console utils 
/// </summary>
public static class Debug {

    public static readonly ColorPalette DEFAULT_PALETTE = new("a11d3c-ef476f-ffd166-06d6a0-0aabe1-9b4f98");
    public static ColorPalette Palette { get; set; } = DEFAULT_PALETTE;

    private static int FATAL_COLOR = Palette.Colors[0];
    public static int FatalColor {
        get => FATAL_COLOR;
        set {
            FATAL_COLOR = value;
            Palette.Colors[0] = value;
        }
    }
    
    private static int ERROR_COLOR = Palette.Colors[1];
    public static int ErrorColor {
        get => ERROR_COLOR;
        set {
            ERROR_COLOR = value;
            Palette.Colors[1] = value;
        }
    }
    
    private static int WARN_COLOR = Palette.Colors[2];
    public static int WarnColor {
        get => WARN_COLOR;
        set {
            WARN_COLOR = value;
            Palette.Colors[2] = value;
        }
    }
    
    private static int INFO_COLOR = Palette.Colors[3];
    public static int InfoColor {
        get => INFO_COLOR;
        set {
            INFO_COLOR = value;
            Palette.Colors[3] = value;
        }
    }
    
    private static int DEBUG_COLOR = Palette.Colors[4];
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
    private static DebugLevel DEBUG_LEVEL = DebugLevel.ERRORS_WARNS;

    public static DebugLevel Level {
        get => DEBUG_LEVEL;
        set => DEBUG_LEVEL = value;
    }
    
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
    /// prints red error text
    /// </summary>
    /// <param name="s">desired string message</param>
    /// <param name="hideTime">hides time if true</param>
    public static void Fatal(string s, bool hideTime = false) {
        if (DEBUG_LEVEL <= DebugLevel.NOTHING) return;
        
        ConsoleColors.PrintColored(hideTime ? "" : $"\e[1m[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] ", FATAL_COLOR);
        ConsoleColors.PrintColoredB("\e[1m\e[38;2;36;36;36m[ FATAL ]", FATAL_COLOR);
        ConsoleColors.PrintColored($"\e[1m : {s}\n", FATAL_COLOR);
    }
    
    /// <summary>
    /// prints red error text
    /// </summary>
    /// <param name="s">desired string message</param>
    /// <param name="hideTime">hides time if true</param>
    public static void Error(string s, bool hideTime = false) {
        if (DEBUG_LEVEL <= DebugLevel.NOTHING) return;
        
        ConsoleColors.PrintColored(hideTime ? "" : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] ", ERROR_COLOR);
        ConsoleColors.PrintColoredB("\e[1m\e[38;2;36;36;36m[ ERROR ]", ERROR_COLOR);
        ConsoleColors.PrintColored($" : {s}\n", ERROR_COLOR);
    }

    /// <summary>
    /// prints yellow warning text
    /// </summary>
    /// <param name="s">desired string message</param>
    /// <param name="hideTime">hides time if true</param>
    public static void Warn(string s, bool hideTime = false) {
        if (DEBUG_LEVEL < DebugLevel.ERRORS_WARNS) return;
        
        ConsoleColors.PrintColored(hideTime ? $"[ WARN ] : {s}\n" : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ WARN ] : {s}\n", WARN_COLOR);
    }
    
    /// <summary>
    /// prints green info text
    /// </summary>
    /// <param name="s">desired string message</param>
    /// <param name="hideTime">hides time if true</param>
    public static void Info(string s, bool hideTime = false) {
        if (DEBUG_LEVEL < DebugLevel.ALL) return;
        
        ConsoleColors.PrintColored(hideTime ? $"[ INFO ] : {s}\n" : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ INFO ] : {s}\n", INFO_COLOR);
    }
    
    /// <summary>
    /// prints red error text
    /// </summary>
    /// <param name="s">desired string message</param>
    /// <param name="hideTime">hides time if true</param>
    public static void Trace(string s, bool hideTime = false) {
        if (DEBUG_LEVEL == DebugLevel.NOTHING) return;
        ConsoleColors.PrintColored(hideTime ? $"[ DEBUG ] : {s}\n" : $"[{DateTime.Today:yyyy-MM-dd} {DateTime.Now:HH:mm:ss}] [ DEBUG ] : {s}\n", DEBUG_COLOR);
    }
}