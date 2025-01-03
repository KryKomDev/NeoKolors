//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using System.Text.RegularExpressions;

namespace NeoKolors.Common;

public static class StringEffects {
    
    /// <summary>
    /// adds a color to the characters of a string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="hex">the hexadecimal representation of the color</param>
    /// <returns>string with colored characters</returns>
    public static string AddColor(this string text, int hex) {
        text = AddTextStyles(text);
        return $"{hex.ControlChar()}{text}{CUSTOM_CONTROL_END}";
    }

    /// <summary>
    /// adds a color to the background of a string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="hex">the hexadecimal representation of the color</param>
    /// <returns>string with colored characters</returns>
    public static string AddColorB(this string text, int hex) {
        text = AddTextStyles(text);
        return $"{hex.ControlCharB()}{text}{CUSTOM_CONTROL_BACKGROUND_END}";
    }

    /// <summary>
    /// adds a color to the characters of a string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="red">red value of the color</param>
    /// <param name="green">green value of the color</param>
    /// <param name="blue">blue value of the color</param>
    /// <returns>string with colored characters</returns>
    public static string AddColor(this string text, byte red, byte green, byte blue) {
        text = AddTextStyles(text);
        return $"\e[38;2;{red};{green};{blue}m{text}{CUSTOM_CONTROL_END}";
    }
    
    /// <summary>
    /// adds a color to the background of a string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="red">red value of the color</param>
    /// <param name="green">green value of the color</param>
    /// <param name="blue">blue value of the color</param>
    /// <returns>string with colored characters</returns>
    public static string AddColorB(this string text, byte red, byte green, byte blue) {
        text = AddTextStyles(text);
        return $"\e[48;2;{red};{green};{blue}m{text}{CUSTOM_CONTROL_BACKGROUND_END}";
    }

    /// <summary>
    /// adds styles to text using html-like tags
    /// </summary>
    /// <param name="text">input text with tags</param>
    /// <returns>text that when printed to console has styles</returns>
    public static string AddTextStyles(this string text) {
        text = text.Replace("<b>", "\x1b[1m");
        text = text.Replace("</b>", "\e[22m");
        text = text.Replace("<i>", "\e[3m");
        text = text.Replace("</i>", "\e[23m");
        text = text.Replace("<u>", "\e[4m");
        text = text.Replace("</u>", "\e[24m");
        text = text.Replace("<f>", "\e[2m");
        text = text.Replace("</f>", "\e[22m");
        text = text.Replace("<n>", "\e[7m");
        text = text.Replace("</n>", "\e[27m");
        text = text.Replace("<s>", "\e[9m");
        text = text.Replace("</s>", "\e[29m");
        return text;
    }

    /// <summary>
    /// adds a color of the terminal palette to the text
    /// </summary>
    /// <returns>text with colors</returns>
    public static string AddColor(this string text, ConsoleColor color) {
        text = AddTextStyles(text);
        return $"{color.ControlChar()}{text}{PALETTE_CONTROL_END}";
    }
    
    /// <summary>
    /// adds a color of the terminal palette to the text's background
    /// </summary>
    /// <returns>text with colored background</returns>
    public static string AddColorB(this string text, ConsoleColor color) {
        text = AddTextStyles(text);
        return $"{color.ControlCharB()}{text}{PALETTE_CONTROL_BACKGROUND_END}";
    }

    /// <summary>
    /// adds a color to the text
    /// </summary>
    /// <returns>colored text</returns>
    public static string AddColor(this string text, Color color) {
        text = AddTextStyles(text);
        return $"{color.ControlChar}{text}{color.ControlCharEnd}";
    }
    
    /// <summary>
    /// adds a colored background to the text
    /// </summary>
    /// <returns>text with colored background</returns>
    public static string AddColorB(this string text, Color color) {
        text = AddTextStyles(text);
        return $"{color.ControlCharB}{text}{color.ControlCharEndB}";
    }

    public static int VisibleLength(this string text) {
        text = Regex.Replace(text, @"\e([^m]*)m", "");
        
        return text.Length;
    }

    public static string AddColor(this string text, params (string symbol, Color color)[] colors) {
        return colors.Aggregate(text, (current, c) => current.Replace(c.symbol, c.color.ControlChar));
    }

    /// <summary>
    /// returns string containing ansi escape sequence coloring the text
    /// </summary>    
    public static string ControlChar(this ConsoleColor color) {
        return color switch {
            ConsoleColor.Black => "\e[38;5;0m",
            ConsoleColor.DarkRed => "\e[38;5;1m",
            ConsoleColor.DarkGreen => "\e[38;5;2m",
            ConsoleColor.DarkYellow => "\e[38;5;3m",
            ConsoleColor.DarkBlue => "\e[38;5;4m",
            ConsoleColor.DarkMagenta => "\e[38;5;5m",
            ConsoleColor.DarkCyan => "\e[38;5;6m",
            ConsoleColor.Gray => "\e[38;5;7m",
            ConsoleColor.DarkGray => "\e[38;5;8m",
            ConsoleColor.Red => "\e[38;5;9m",
            ConsoleColor.Green => "\e[38;5;10m",
            ConsoleColor.Yellow => "\e[38;5;11m",
            ConsoleColor.Blue => "\e[38;5;12m",
            ConsoleColor.Magenta => "\e[38;5;13m",
            ConsoleColor.Cyan => "\e[38;5;14m",
            ConsoleColor.White => "\e[38;5;15m",
            _ => throw new ArgumentOutOfRangeException(nameof(ConsoleColor), color, null)
        };
    }

    /// <summary>
    /// returns string containing ansi escape sequence coloring background
    /// </summary>
    public static string ControlCharB(this ConsoleColor color) {
        return color switch {
            ConsoleColor.Black => "\e[48;5;0m",
            ConsoleColor.DarkRed => "\e[48;5;1m",
            ConsoleColor.DarkGreen => "\e[48;5;2m",
            ConsoleColor.DarkYellow => "\e[48;5;3m",
            ConsoleColor.DarkBlue => "\e[48;5;4m",
            ConsoleColor.DarkMagenta => "\e[48;5;5m",
            ConsoleColor.DarkCyan => "\e[48;5;6m",
            ConsoleColor.Gray => "\e[48;5;7m",
            ConsoleColor.DarkGray => "\e[48;5;8m",
            ConsoleColor.Red => "\e[48;5;9m",
            ConsoleColor.Green => "\e[48;5;10m",
            ConsoleColor.Yellow => "\e[48;5;11m",
            ConsoleColor.Blue => "\e[48;5;12m",
            ConsoleColor.Magenta => "\e[48;5;13m",
            ConsoleColor.Cyan => "\e[48;5;14m",
            ConsoleColor.White => "\e[48;5;15m",
            _ => throw new ArgumentOutOfRangeException(nameof(ConsoleColor), color, null)
        };
    }

    /// <summary>
    /// returns string containing ansi escape sequence coloring the text
    /// </summary>
    public static string ControlChar(this int color) =>
        $"\e[38;2;{(byte)(color >> 16)};{(byte)(color >> 8)};{(byte)color}m";

    /// <summary>
    /// returns string containing ansi escape sequence coloring background
    /// </summary>
    public static string ControlCharB(this int color) =>
        $"\e[48;2;{(byte)(color >> 16)};{(byte)(color >> 8)};{(byte)color}m";
    
    public const string CUSTOM_CONTROL_END = "[38;1;m";
    public const string CUSTOM_CONTROL_BACKGROUND_END = "[48;1;m";
    public const string PALETTE_CONTROL_END = "\e[38;5;7m";
    public const string PALETTE_CONTROL_BACKGROUND_END = "\e[48;5;7m";

    // ReSharper disable once InconsistentNaming
    public const string CustomControlEnd = CUSTOM_CONTROL_END;
    // ReSharper disable once InconsistentNaming
    public const string CustomControlBackgroundEnd = CUSTOM_CONTROL_BACKGROUND_END;
    // ReSharper disable once InconsistentNaming
    public const string PaletteControlEnd = PALETTE_CONTROL_END;
    // ReSharper disable once InconsistentNaming
    public const string PaletteControlBackgroundEnd = PALETTE_CONTROL_BACKGROUND_END;

    /// <summary>
    /// switches colors of background and text
    /// </summary>
    public const string SWITCH_COLORS = "\e[38;1;7m";
}