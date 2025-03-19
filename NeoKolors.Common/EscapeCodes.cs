//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Common;

/// <summary>
/// contains ansi escape sequences  
/// </summary>
public static class EscapeCodes {

    /// <summary>
    /// sequence for coloring a string with a custom color,
    /// should be followed by <c>r;g;bm</c> where r, g and b represent red green and blue respectively
    /// </summary>
    public const string CUSTOM_COLOR_START = "\e[38;2;";
    
    /// <summary>
    /// sequence for ending the custom coloring of a string,
    /// ends only the custom coloring, all other styles are left untouched
    /// </summary>
    public const string CUSTOM_COLOR_END = "\e[38;1;m";
    
    public const string CUSTOM_BACKGROUND_COLOR_START = "\e[48;2;";
    public const string CUSTOM_BACKGROUND_COLOR_END = "\e[48;1;m";

    public const string PALETTE_COLOR_BLACK = "\e[38;5;0m";
    public const string PALETTE_COLOR_DARK_RED = "\e[38;5;1m";
    public const string PALETTE_COLOR_DARK_GREEN = "\e[38;5;2m";
    public const string PALETTE_COLOR_DARK_YELLOW ="\e[38;5;3m";
    public const string PALETTE_COLOR_DARK_BLUE ="\e[38;5;4m";
    public const string PALETTE_COLOR_DARK_MAGENTA = "\e[38;5;5m";
    public const string PALETTE_COLOR_DARK_CYAN = "\e[38;5;6m";
    public const string PALETTE_COLOR_GRAY = "\e[38;5;7m";
    public const string PALETTE_COLOR_DARK_GRAY = "\e[38;5;8m";
    public const string PALETTE_COLOR_RED = "\e[38;5;9m";
    public const string PALETTE_COLOR_GREEN = "\e[38;5;10m";
    public const string PALETTE_COLOR_YELLOW = "\e[38;5;11m";
    public const string PALETTE_COLOR_BLUE = "\e[38;5;12m";
    public const string PALETTE_COLOR_MAGENTA = "\e[38;5;13m";
    public const string PALETTE_COLOR_CYAN = "\e[38;5;14m";
    public const string PALETTE_COLOR_WHITE = "\e[38;5;15m";
    
    public const string PALETTE_COLOR_END = "\e[39m";
    
    public const string PALETTE_BACKGROUND_COLOR_BLACK = "\e[48;5;0m";
    public const string PALETTE_BACKGROUND_COLOR_DARK_RED = "\e[48;5;1m";
    public const string PALETTE_BACKGROUND_COLOR_DARK_GREEN = "\e[48;5;2m";
    public const string PALETTE_BACKGROUND_COLOR_DARK_YELLOW ="\e[48;5;3m";
    public const string PALETTE_BACKGROUND_COLOR_DARK_BLUE ="\e[48;5;4m";
    public const string PALETTE_BACKGROUND_COLOR_DARK_MAGENTA = "\e[48;5;5m";
    public const string PALETTE_BACKGROUND_COLOR_DARK_CYAN = "\e[48;5;6m";
    public const string PALETTE_BACKGROUND_COLOR_GRAY = "\e[48;5;7m";
    public const string PALETTE_BACKGROUND_COLOR_DARK_GRAY = "\e[48;5;8m";
    public const string PALETTE_BACKGROUND_COLOR_RED = "\e[48;5;9m";
    public const string PALETTE_BACKGROUND_COLOR_GREEN = "\e[48;5;10m";
    public const string PALETTE_BACKGROUND_COLOR_YELLOW = "\e[48;5;11m";
    public const string PALETTE_BACKGROUND_COLOR_BLUE = "\e[48;5;12m";
    public const string PALETTE_BACKGROUND_COLOR_MAGENTA = "\e[48;5;13m";
    public const string PALETTE_BACKGROUND_COLOR_CYAN = "\e[48;5;14m";
    public const string PALETTE_BACKGROUND_COLOR_WHITE = "\e[48;5;15m";
    
    public const string PALETTE_BACKGROUND_COLOR_END = "\e[49m";

    /// <summary>
    /// disables all styles
    /// </summary>
    public const string STYLE_END = "\e[0m";
    
    /// <summary>
    /// switches colors of background and text
    /// </summary>
    public const string SWITCH_COLORS = "\e[38;1;7m";
    
    public const string BOLD_START = "\e[1m";
    public const string ITALIC_START = "\e[3m";
    public const string UNDERLINE_START = "\e[4m";
    public const string FAINT_START = "\e[2m";
    public const string NEGATIVE_START = "\e[7m";
    public const string STRIKETHROUGH_START = "\e[9m";
    
    public const string BOLD_END = "\e[22m";
    public const string ITALIC_END = "\e[23m";
    public const string UNDERLINE_END = "\e[24m";
    public const string FAINT_END = "\e[22m";
    public const string NEGATIVE_END = "\e[27m";
    public const string STRIKETHROUGH_END = "\e[29m";

    // public const string MOUSE_BUTTON_ON = "\e[?1000h";
    // public const string MOUSE_HIGHLIGHT_ON = "\e[?1001h";
    // public const string MOUSE_DRAG_ON = "\e[?1002h";
    // public const string MOUSE_MOTION_ON = "\e[?1003h";
    // public const string MOUSE_SRG_ON = "\e[?1006h";
    //
    // public const string MOUSE_BUTTON_OFF = "\e[?1000l";
    // public const string MOUSE_HIGHLIGHT_OFF = "\e[?1001l";
    // public const string MOUSE_DRAG_OFF = "\e[?1002l";
    // public const string MOUSE_MOTION_OFF = "\e[?1003l";
    // public const string MOUSE_SRG_OFF = "\e[?1006l";
    //
    // public static void EnableMouseEvents() {
    //     System.Console.Write(MOUSE_BUTTON_ON);
    //     System.Console.Write(MOUSE_HIGHLIGHT_ON);
    //     System.Console.Write(MOUSE_DRAG_ON);
    //     System.Console.Write(MOUSE_MOTION_ON);
    //     System.Console.Write(MOUSE_SRG_ON);
    // }
    //
    // public static void DisableMouseEvents() {
    //     System.Console.Write(MOUSE_BUTTON_OFF);
    //     System.Console.Write(MOUSE_HIGHLIGHT_OFF);
    //     System.Console.Write(MOUSE_DRAG_OFF);
    //     System.Console.Write(MOUSE_MOTION_OFF);
    //     System.Console.Write(MOUSE_SRG_OFF);
    // }

    public const string ENABLE_SECONDARY_CONTEXT = "\e[?1049h";
    public const string DISABLE_SECONDARY_CONTEXT = "\e[?1049l";

    public static void EnableSecondary() => Console.Write(ENABLE_SECONDARY_CONTEXT);
    public static void DisableSecondary() => Console.Write(DISABLE_SECONDARY_CONTEXT);
    
    // ReSharper disable InconsistentNaming
    public const string CustomColorEnd = CUSTOM_COLOR_END;
    public const string CustomBackgroundColorEnd = CUSTOM_BACKGROUND_COLOR_END;
    public const string PaletteColorEnd = PALETTE_COLOR_END;
    public const string PaletteBackgroundColorEnd = PALETTE_BACKGROUND_COLOR_END;
    public const string EnableSecondaryContext = ENABLE_SECONDARY_CONTEXT;
    public const string DisableSecondaryContext = DISABLE_SECONDARY_CONTEXT;
    public const string SwitchColors = SWITCH_COLORS;
    // ReSharper restore InconsistentNaming
}