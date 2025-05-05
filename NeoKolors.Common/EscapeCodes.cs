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
    /// Sequence for coloring a string with a custom color.
    /// Should be followed by <c>r;g;bm</c> where r, g, and b represent red green and blue respectively
    /// </summary>
    public const string CUSTOM_COLOR_PREFIX = "\e[38;2;";

    /// <summary>
    /// Sequence for setting a custom background color.
    /// Should be followed by <c>r;g;bm</c> where r, g, and b represent red, green, and blue components respectively.
    /// </summary>
    public const string CUSTOM_BACKGROUND_COLOR_PREFIX = "\e[48;2;";
    
    public static string GetPaletteFColor(byte color) => $"\e[38;5;{color}m";
    public static string GetPaletteBColor(byte color) => $"\e[48;5;{color}m";
    
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

    /// <summary>
    /// ANSI escape sequence used to reset the text color to the default terminal color.
    /// Used to revert any previously applied text color changes.
    /// </summary>
    public const string TEXT_COLOR_END = "\e[39m";
    
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

    /// <summary>
    /// ANSI escape sequence that resets the background color to its default value.
    /// Used to end custom background coloring in a string.
    /// </summary>
    public const string BACKGROUND_COLOR_END = "\e[49m";

    /// <summary>
    /// Resets all text formatting attributes, including color and style, to their default values.
    /// </summary>
    public const string FORMATTING_RESET = "\e[0m";
    
    /// <summary>
    /// Switches colors of background and text. We have no idea why it works, and it is not documented.
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
    public const string PaletteColorEnd = TEXT_COLOR_END;
    public const string PaletteBackgroundColorEnd = BACKGROUND_COLOR_END;
    public const string EnableSecondaryContext = ENABLE_SECONDARY_CONTEXT;
    public const string DisableSecondaryContext = DISABLE_SECONDARY_CONTEXT;
    public const string SwitchColors = SWITCH_COLORS;
    // ReSharper restore InconsistentNaming
}