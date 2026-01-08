//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.Contracts;

namespace NeoKolors.Common;

/// <summary>
/// contains ansi escape sequences  
/// </summary>
public static class EscapeCodes {

    #region COLORING

    /// <summary>
    /// ANSI escape sequence format string used to apply a custom text color in the terminal.
    /// The format uses RGB values, where placeholders {0}, {1}, and {2} represent
    /// the red, green, and blue color components respectively.
    /// </summary>
    public const string CUSTOM_TEXT_COLOR_FORMAT = "\e[38;2;{0};{1};{2}m";

    /// <summary>
    /// ANSI escape sequence format string used to apply a custom background color in the terminal.
    /// The format uses RGB values, where placeholders {0}, {1}, and {2} represent
    /// the red, green, and blue color components respectively.
    /// </summary>
    public const string CUSTOM_BCKG_COLOR_FORMAT = "\e[48;2;{0};{1};{2}m";
    
    /// <summary>
    /// ANSI escape sequence format string used to apply a custom background color in the terminal.
    /// The format uses RGB values, where placeholders {0}, {1}, and {2} represent
    /// the red, green, and blue color components respectively.
    /// </summary>
    public const string CUSTOM_UNDERLINE_COLOR_FORMAT = "\e[58;2;{0};{1};{2}m";

    [Pure]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static string GetPaletteFColor(byte color) => $"\e[38;5;{color}m";

    [Pure]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static string GetPaletteBColor(byte color) => $"\e[48;5;{color}m";
    
    [Pure]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static string GetPaletteUColor(byte color) => $"\e[58;5;{color}m";

    public const string PALETTE_COLOR_BLACK = "\e[38;5;0m";
    public const string PALETTE_COLOR_DARK_RED = "\e[38;5;1m";
    public const string PALETTE_COLOR_DARK_GREEN = "\e[38;5;2m";
    public const string PALETTE_COLOR_DARK_YELLOW = "\e[38;5;3m";
    public const string PALETTE_COLOR_DARK_BLUE = "\e[38;5;4m";
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
    public const string TEXT_COLOR_RESET = "\e[39m";

    public const string PALETTE_BCKG_COLOR_BLACK = "\e[48;5;0m";
    public const string PALETTE_BCKG_COLOR_DARK_RED = "\e[48;5;1m";
    public const string PALETTE_BCKG_COLOR_DARK_GREEN = "\e[48;5;2m";
    public const string PALETTE_BCKG_COLOR_DARK_YELLOW = "\e[48;5;3m";
    public const string PALETTE_BCKG_COLOR_DARK_BLUE = "\e[48;5;4m";
    public const string PALETTE_BCKG_COLOR_DARK_MAGENTA = "\e[48;5;5m";
    public const string PALETTE_BCKG_COLOR_DARK_CYAN = "\e[48;5;6m";
    public const string PALETTE_BCKG_COLOR_GRAY = "\e[48;5;7m";
    public const string PALETTE_BCKG_COLOR_DARK_GRAY = "\e[48;5;8m";
    public const string PALETTE_BCKG_COLOR_RED = "\e[48;5;9m";
    public const string PALETTE_BCKG_COLOR_GREEN = "\e[48;5;10m";
    public const string PALETTE_BCKG_COLOR_YELLOW = "\e[48;5;11m";
    public const string PALETTE_BCKG_COLOR_BLUE = "\e[48;5;12m";
    public const string PALETTE_BCKG_COLOR_MAGENTA = "\e[48;5;13m";
    public const string PALETTE_BCKG_COLOR_CYAN = "\e[48;5;14m";
    public const string PALETTE_BCKG_COLOR_WHITE = "\e[48;5;15m";

    /// <summary>
    /// ANSI escape sequence that resets the background color to its default value.
    /// Used to end custom background coloring in a string.
    /// </summary>
    public const string BCKG_COLOR_RESET = "\e[49m";
    
    public const string PALETTE_UNDERLINE_COLOR_BLACK = "\e[58;5;0m";
    public const string PALETTE_UNDERLINE_COLOR_DARK_RED = "\e[58;5;1m";
    public const string PALETTE_UNDERLINE_COLOR_DARK_GREEN = "\e[58;5;2m";
    public const string PALETTE_UNDERLINE_COLOR_DARK_YELLOW = "\e[58;5;3m";
    public const string PALETTE_UNDERLINE_COLOR_DARK_BLUE = "\e[58;5;4m";
    public const string PALETTE_UNDERLINE_COLOR_DARK_MAGENTA = "\e[58;5;5m";
    public const string PALETTE_UNDERLINE_COLOR_DARK_CYAN = "\e[58;5;6m";
    public const string PALETTE_UNDERLINE_COLOR_GRAY = "\e[58;5;7m";
    public const string PALETTE_UNDERLINE_COLOR_DARK_GRAY = "\e[58;5;8m";
    public const string PALETTE_UNDERLINE_COLOR_RED = "\e[58;5;9m";
    public const string PALETTE_UNDERLINE_COLOR_GREEN = "\e[58;5;10m";
    public const string PALETTE_UNDERLINE_COLOR_YELLOW = "\e[58;5;11m";
    public const string PALETTE_UNDERLINE_COLOR_BLUE = "\e[58;5;12m";
    public const string PALETTE_UNDERLINE_COLOR_MAGENTA = "\e[58;5;13m";
    public const string PALETTE_UNDERLINE_COLOR_CYAN = "\e[58;5;14m";
    public const string PALETTE_UNDERLINE_COLOR_WHITE = "\e[58;5;15m";

    /// <summary>
    /// ANSI escape sequence that resets the underline color to its default value.
    /// Used to end custom underline coloring in a string.
    /// </summary>
    public const string UNDERLINE_COLOR_RESET = "\e[59m";

    /// <summary>
    /// Resets all text formatting attributes, including color and style, to their default values.
    /// </summary>
    public const string FORMATTING_RESET = "\e[0m";

    #endregion

    #region STYLING

    public const string BOLD_START = "\e[1m";
    public const string FAINT_START = "\e[2m";
    public const string ITALIC_START = "\e[3m";
    public const string UNDERLINE_START = "\e[4m";
    public const string UNDERLINE_THICK_START = "\e[4:2m";
    public const string UNDERLINE_CURLY_START = "\e[4:3m";
    public const string UNDERLINE_DOTTED_START = "\e[4:4m";
    public const string UNDERLINE_DASHED_START = "\e[4:5m";
    public const string UNDERLINE_START_TYPE_FORMAT = "\e[4:{0}m";
    public const string BLINK_START = "\e[5m";
    public const string NEGATIVE_START = "\e[7m";
    public const string INVISIBLE_START = "\e[8m";
    public const string STRIKETHROUGH_START = "\e[9m";

    public const string BOLD_END = "\e[22m";
    public const string FAINT_END = "\e[22m";
    public const string ITALIC_END = "\e[23m";
    public const string UNDERLINE_END = "\e[24m";
    public const string BLINK_END = "\e[25m";
    public const string NEGATIVE_END = "\e[27m";
    public const string INVISIBLE_END = "\e[28m";
    public const string STRIKETHROUGH_END = "\e[29m";

    /// <summary>
    /// Generates an escape sequence to enable underlining with the specified underline type.
    /// </summary>
    /// <param name="type">The type of underline to apply, such as normal, thick, curly, dotted, or dashed.
    /// Defaults to normal if not specified.</param>
    /// <returns>A string containing the formatted escape sequence to enable the specified underline style.</returns>
    public static string GetUnderline(UnderlineType type = UnderlineType.NORMAL)
        => type == UnderlineType.NORMAL 
            ? UNDERLINE_START 
            : UNDERLINE_START_TYPE_FORMAT.Format((int)type);
    
    #endregion

    #region INPUT

    /// <summary>
    /// Enables mouse tracking mode only for mouse button presses.
    /// </summary>
    public const string MOUSE_EV_ON_P_ON = "\e[?9h";

    /// <summary>
    /// Enables mouse tracking mode only for mouse button presses and releases.
    /// </summary>
    public const string MOUSE_EV_ON_PR_ON = "\e[?1000h";

    // no idea
    public const string MOUSE_EV_IN_HIGHLIGHT_ON = "\e[?1001h";

    /// <summary>
    /// Enables mouse tracking mode only for mouse button presses, releases, and mouse drags.
    /// </summary>
    public const string MOUSE_EV_ON_PRD_ON = "\e[?1002h";

    /// <summary>
    /// Enables mouse tracking mode for all mouse events.
    /// </summary>
    public const string MOUSE_EV_ON_ALL_ON = "\e[?1003h";

    /// <summary>
    /// Enables mouse tracking Unicode encoding support.
    /// </summary>
    public const string MOUSE_EV_UTF8_ON = "\e[?1005h";

    /// <summary>
    /// Enables SGR mouse tracking protocol.
    /// </summary>
    public const string MOUSE_EV_SGR_ON = "\e[?1006h";

    public const string MOUSE_EV_SGR_PIXELS_ON = "\e[?1016h";

    [Obsolete("URXVT mode is not recommended.")]
    public const string MOUSE_EV_URXVT_ON = "\e[?1015h";

    public const string MOUSE_EV_ON_P_OFF = "\e[?9l";
    public const string MOUSE_EV_ON_PR_OFF = "\e[?1000l";
    public const string MOUSE_EV_IN_HIGHLIGHT_OFF = "\e[?1001l";
    public const string MOUSE_EV_ON_PRD_OFF = "\e[?1002l";
    public const string MOUSE_EV_ON_ALL_OFF = "\e[?1003l";
    public const string MOUSE_EV_UTF8_OFF = "\e[?1005l";
    public const string MOUSE_EV_SGR_OFF = "\e[?1006l";
    public const string MOUSE_EV_SGR_PIXELS_OFF = "\e[?1016l";

    [Obsolete("URXVT mode is not recommended.")]
    public const string MOUSE_EV_URXVT_OFF = "\e[?1015l";


    /// <summary>
    /// Escape sequence to enable bracketed paste mode in the terminal,
    /// allowing applications to handle clipboard pastes more effectively.
    /// </summary>
    public const string ENABLE_BRACKETED_PASTE_MODE = "\e[?2004h";

    /// <summary>
    /// Escape sequence used to disable bracketed paste mode in the terminal.
    /// Commonly used to revert clipboard handling to its default state.
    /// </summary>
    public const string DISABLE_BRACKETED_PASTE_MODE = "\e[?2004l";

    #endregion

    #region SCREEN

    /// <summary>
    /// Escape sequence for enabling the alternate buffer without saving the cursor state.
    /// This is used to switch to an alternate screen buffer, often in terminal-based applications.
    /// </summary>
    public const string ALT_BUFF_NO_SC_ENABLE = "\e[?1047h";

    /// <summary>
    /// Escape sequence for disabling the alternate buffer without restoring the cursor state.
    /// This is used to switch from an alternate screen buffer, often in terminal-based applications.
    /// </summary>
    public const string ALT_BUFF_NO_RC_DISABLE = "\e[?1047l";

    /// <summary>
    /// ANSI escape sequence to save the current state of the cursor in the terminal,
    /// including its position and potentially other states depending on terminal behavior.
    /// </summary>
    public const string SAVE_CURSOR_STATE = "\e[?1048h";

    /// <summary>
    /// ANSI escape sequence used to restore the previously saved cursor state
    /// in the terminal, including its position and other attributes.
    /// </summary>
    public const string RESTORE_CURSOR_STATE = "\e[?1048l";

    /// <summary>
    /// Escape sequence for enabling the alternate buffer while saving the cursor state.
    /// This is used to switch to an alternate screen buffer, often in terminal-based applications.
    /// </summary>
    public const string ALT_BUFF_SC_ENABLE = "\e[?1049h";

    /// <summary>
    /// Escape sequence for disabling the alternate buffer while restoring the cursor state.
    /// This is used to switch from an alternate screen buffer, often in terminal-based applications.
    /// </summary>
    public const string ALT_BUFF_RC_DISABLE = "\e[?1049l";

    #endregion

    public const string REPORT_FOCUS_ENABLE = "\e[?1004h";
    public const string REPORT_FOCUS_DISABLE = "\e[?1004l";

    #region WINDOW MANIPULATION

    /// <summary>
    /// Escape code sequence used to iconify or minimize the current terminal window.
    /// </summary>
    public const string ICONIFY_WINDOW = "\e[2t";

    /// <summary>
    /// Escape sequence to restore and deiconify a minimized window.
    /// Typically used in terminal applications to request the window to return to its
    /// normal, unminimized state.
    /// </summary>
    public const string DEICONIFY_WINDOW = "\e[1t";

    public const string MOVE_WINDOW_FORMAT = "\e[3;{0};{1}t";
    public const string RESIZE_WINDOW_PX_FORMAT = "\e[4;{0};{1}t";
    public const string RESIZE_WINDOW_CH_FORMAT = "\e[8;{0};{1}t";
    public const string RAISE_WINDOW = "\e[5t";
    public const string LOWER_WINDOW = "\e[6t";

    /// <summary>
    /// ANSI escape sequence used to report the current window position.
    /// This sequence can be used to query the terminal for its cursor position
    /// or other related spatial information in an active session.
    /// </summary>
    public const string REPORT_WINDOW_POS = "\e[13t";

    public const string REPORT_WINDOW_SIZE_PX = "\e[14t";

    #endregion

    #region OUTPUT

    /// <summary>
    /// Fills the specified rectangular area with the specified character.
    /// Parameter 0 denotes the filler character, params 1-4 denote the rectangle area
    /// (top, left, bottom, and right coordinates respectively).
    /// </summary>
    public const string FILL_RECT_FORMAT = "\e[{0};{1};{2};{3};{4}$x";

    /// <summary>
    /// Erases the specified rectangular area.
    /// Params 0-3 denote the rectangle area
    /// (top, left, bottom, and right coordinates respectively).
    /// </summary>
    public const string ERASE_RECT_FORMAT = "\e[{0};{1};{2};{3}$z";

    #endregion

    #region DECREQ

    public const string DECREQ_FORMAT = "\e[{0}$p";
    
    /// <summary>
    /// Requests the state of the alternate buffer.
    /// Should return <c>\e[?1049;1$y</c> if enabled, <c>\e[?1049;2$y</c> if disabled.
    /// </summary>
    public const string REQUEST_ALTBUF_STATE = "\e[?1049$p";

    /// <summary>
    /// Generates a Device Control Request (DECREQ) escape sequence for the specified terminal private mode.
    /// </summary>
    /// <param name="mode">The private mode for which the DECREQ escape sequence should be generated.</param>
    /// <returns>A string containing the formatted DECREQ escape sequence.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetDecReq(DecPrivateMode mode) => DECREQ_FORMAT.Format((int)mode);
    
    #endregion

    #region DEC PM

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetDecPm(DecPrivateMode mode, bool value) => $"\e[?{(int)mode}{(value ? 'h' : 'l')}";

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetDecSet(DecPrivateMode mode) => GetDecPm(mode, true);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetDecSet(params DecPrivateMode[] modes) 
        => $"\e[?{string.Join(';', modes.Select(m => (int)m))}h";
    
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetDecRst(DecPrivateMode mode) => GetDecPm(mode, false);
    
    
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetDecRst(params DecPrivateMode[] modes) 
        => $"\e[?{string.Join(';', modes.Select(m => (int)m))}l";
    
    public const string ENABLE_CURSOR_VISIBLE = "\e[?25h";
    public const string DISABLE_CURSOR_VISIBLE = "\e[?25l";
    
    #endregion
    
    public enum DecPrivateMode {
        REPORT_MOUSE_P = 9,
        REPORT_MOUSE_PR = 1000,
        REPORT_MOUSE_PRD = 1002,
        REPORT_MOUSE_ALL = 1003,
        REPORT_MOUSE_UTF8 = 1005,
        REPORT_MOUSE_PROTOCOL_SGR = 1006,
        REPORT_MOUSE_PROTOCOL_URXVT = 1015,
        REPORT_FOCUS = 1004,
        ALTERNATE_BUFFER = 1049,
        BRACKETED_PASTE_MODE = 2004,
        CURSOR_VISIBLE_MODE = 25,
    }
}