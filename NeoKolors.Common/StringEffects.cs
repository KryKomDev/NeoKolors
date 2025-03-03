//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Text.RegularExpressions;
using static NeoKolors.Common.EscapeCodes;

namespace NeoKolors.Common;

public static class StringEffects {
    
    /// <summary>
    /// adds a color to the characters of a string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="hex">the hexadecimal representation of the color</param>
    /// <returns>string with colored characters</returns>
    public static string AddColor(this string text, int hex) {
        text = ApplyStyles(text);
        return $"{hex.ControlChar()}{text}{CUSTOM_COLOR_END}";
    }

    /// <summary>
    /// adds a color to the background of a string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="hex">the hexadecimal representation of the color</param>
    /// <returns>string with colored characters</returns>
    public static string AddColorB(this string text, int hex) {
        text = ApplyStyles(text);
        return $"{hex.ControlCharB()}{text}{CUSTOM_BACKGROUND_COLOR_END}";
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
        text = ApplyStyles(text);
        return $"{CUSTOM_COLOR_START}{red};{green};{blue}m{text}{CUSTOM_COLOR_END}";
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
        text = ApplyStyles(text);
        return $"{CUSTOM_BACKGROUND_COLOR_START}{red};{green};{blue}m{text}{CUSTOM_BACKGROUND_COLOR_END}";
    }

    /// <summary>
    /// adds styles to text using html-like tags
    /// </summary>
    /// <param name="text">input text with tags</param>
    /// <returns>text that when printed to console has styles</returns>
    public static string ApplyStyles(this string text) {
        text = text.Replace("<b>", BOLD_START);
        text = text.Replace("</b>", BOLD_END);
        text = text.Replace("<i>", ITALIC_START);
        text = text.Replace("</i>", ITALIC_END);
        text = text.Replace("<u>", UNDERLINE_START);
        text = text.Replace("</u>", UNDERLINE_END);
        text = text.Replace("<f>", FAINT_START);
        text = text.Replace("</f>", FAINT_END);
        text = text.Replace("<n>", NEGATIVE_START);
        text = text.Replace("</n>", NEGATIVE_END);
        text = text.Replace("<s>", STRIKETHROUGH_START);
        text = text.Replace("</s>", STRIKETHROUGH_END);
        return text;
    }

    /// <summary>
    /// adds text styles to the input string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="styles">styles applied to the text</param>
    /// <returns>string with the styles applied</returns>
    /// <exception cref="ArgumentOutOfRangeException">an invalid style was inputted</exception>
    public static string AddStyles(this string text, params TextStyle[] styles) {
        foreach (var s in styles) {
            text = text.AddStyle(s);
        }
        
        return text;
    }

    /// <summary>
    /// adds a single text style to the input string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="style">style applied to the text</param>
    /// <returns>string with the style applied</returns>
    /// <exception cref="ArgumentOutOfRangeException">an invalid style was inputted</exception>
    public static string AddStyle(this string text, TextStyle style) {
        return style switch {
            TextStyle.BOLD => $"{BOLD_START}{text}{BOLD_END}",
            TextStyle.ITALIC => $"{ITALIC_START}{text}{ITALIC_END}",
            TextStyle.UNDERLINE => $"{UNDERLINE_START}{text}{UNDERLINE_END}",
            TextStyle.FAINT => $"{FAINT_START}{text}{FAINT_END}",
            TextStyle.NEGATIVE => $"{NEGATIVE_START}{text}{NEGATIVE_END}",
            TextStyle.STRIKETHROUGH => $"{STRIKETHROUGH_START}{text}{STRIKETHROUGH_END}",
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };
    }
    
    /// <summary>
    /// adds a color of the terminal palette to the text
    /// </summary>
    /// <returns>text with colors</returns>
    public static string AddColor(this string text, ConsoleColor color) {
        text = ApplyStyles(text);
        return $"{color.ControlChar()}{text}{PALETTE_COLOR_END}";
    }
    
    /// <summary>
    /// adds a color of the terminal palette to the text's background
    /// </summary>
    /// <returns>text with colored background</returns>
    public static string AddColorB(this string text, ConsoleColor color) {
        text = ApplyStyles(text);
        return $"{color.ControlCharB()}{text}{PALETTE_BACKGROUND_COLOR_END}";
    }

    /// <summary>
    /// adds a color to the text
    /// </summary>
    /// <returns>colored text</returns>
    public static string AddColor(this string text, Color color) {
        text = ApplyStyles(text);
        return $"{color.ControlChar}{text}{color.ControlCharEnd}";
    }
    
    /// <summary>
    /// adds a colored background to the text
    /// </summary>
    /// <returns>text with colored background</returns>
    public static string AddColorB(this string text, Color color) {
        text = ApplyStyles(text);
        return $"{color.ControlCharB}{text}{color.ControlCharEndB}";
    }
    
    /// <summary>
    /// adds a color to the text
    /// </summary>
    /// <returns>colored text</returns>
    public static string AddColor(this string text, System.Drawing.Color color) {
        return text.AddColor(color.R, color.G, color.B);
    }
    
    /// <summary>
    /// adds a colored background to the text
    /// </summary>
    /// <returns>text with colored background</returns>
    public static string AddColorB(this string text, System.Drawing.Color color) {
        return text.AddColorB(color.R, color.G, color.B);
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
            ConsoleColor.Black => PALETTE_COLOR_BLACK,
            ConsoleColor.DarkRed => PALETTE_COLOR_DARK_RED,
            ConsoleColor.DarkGreen => PALETTE_COLOR_DARK_GREEN,
            ConsoleColor.DarkYellow => PALETTE_COLOR_DARK_YELLOW,
            ConsoleColor.DarkBlue => PALETTE_COLOR_DARK_BLUE,
            ConsoleColor.DarkMagenta => PALETTE_COLOR_DARK_MAGENTA,
            ConsoleColor.DarkCyan => PALETTE_COLOR_DARK_CYAN,
            ConsoleColor.Gray => PALETTE_COLOR_GRAY,
            ConsoleColor.DarkGray => PALETTE_COLOR_DARK_GRAY,
            ConsoleColor.Red => PALETTE_COLOR_RED,
            ConsoleColor.Green => PALETTE_COLOR_GREEN,
            ConsoleColor.Yellow => PALETTE_COLOR_YELLOW,
            ConsoleColor.Blue => PALETTE_COLOR_BLUE,
            ConsoleColor.Magenta => PALETTE_COLOR_MAGENTA,
            ConsoleColor.Cyan => PALETTE_COLOR_CYAN,
            ConsoleColor.White => PALETTE_COLOR_WHITE,
            _ => throw new ArgumentOutOfRangeException(nameof(ConsoleColor), color, null)
        };
    }

    /// <summary>
    /// returns string containing ansi escape sequence coloring background
    /// </summary>
    public static string ControlCharB(this ConsoleColor color) {
        return color switch {
            ConsoleColor.Black => PALETTE_BACKGROUND_COLOR_BLACK,
            ConsoleColor.DarkRed => PALETTE_BACKGROUND_COLOR_DARK_RED,
            ConsoleColor.DarkGreen => PALETTE_BACKGROUND_COLOR_DARK_GREEN,
            ConsoleColor.DarkYellow => PALETTE_BACKGROUND_COLOR_DARK_YELLOW,
            ConsoleColor.DarkBlue => PALETTE_BACKGROUND_COLOR_DARK_BLUE,
            ConsoleColor.DarkMagenta => PALETTE_BACKGROUND_COLOR_DARK_MAGENTA,
            ConsoleColor.DarkCyan => PALETTE_BACKGROUND_COLOR_DARK_CYAN,
            ConsoleColor.Gray => PALETTE_BACKGROUND_COLOR_GRAY,
            ConsoleColor.DarkGray => PALETTE_BACKGROUND_COLOR_DARK_GRAY,
            ConsoleColor.Red => PALETTE_BACKGROUND_COLOR_RED,
            ConsoleColor.Green => PALETTE_BACKGROUND_COLOR_GREEN,
            ConsoleColor.Yellow => PALETTE_BACKGROUND_COLOR_YELLOW,
            ConsoleColor.Blue => PALETTE_BACKGROUND_COLOR_BLUE,
            ConsoleColor.Magenta => PALETTE_BACKGROUND_COLOR_MAGENTA,
            ConsoleColor.Cyan => PALETTE_BACKGROUND_COLOR_CYAN,
            ConsoleColor.White => PALETTE_BACKGROUND_COLOR_WHITE,
            _ => throw new ArgumentOutOfRangeException(nameof(ConsoleColor), color, null)
        };
    }

    /// <summary>
    /// adds color to both the text and the background
    /// </summary>
    public static string AddColor(this string s, Color text, Color background) {
        s = ApplyStyles(s);
        s = AddColor(s, text);
        s = AddColorB(s, background);
        return s;
    }

    /// <summary>
    /// returns string containing ansi escape sequence coloring the text
    /// </summary>
    public static string ControlChar(this int color) =>
        $"{CUSTOM_COLOR_START}{(byte)(color >> 16)};{(byte)(color >> 8)};{(byte)color}m";

    /// <summary>
    /// returns string containing ansi escape sequence coloring background
    /// </summary>
    public static string ControlCharB(this int color) =>
        $"{CUSTOM_BACKGROUND_COLOR_START}{(byte)(color >> 16)};{(byte)(color >> 8)};{(byte)color}m";
    
    /// <summary>
    /// style types
    /// </summary>
    public enum TextStyle {
        BOLD = 1,
        ITALIC = 3,
        UNDERLINE = 4,
        FAINT = 2,
        NEGATIVE = 7,
        STRIKETHROUGH = 9
    }
}