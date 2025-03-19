//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static NeoKolors.Common.EscapeCodes;

namespace NeoKolors.Common;

public static class StringEffects {
    
    /// <summary>
    /// adds a color to the characters of a string
    /// </summary>
    /// <param name="s">input string</param>
    /// <param name="hex">the hexadecimal representation of the color</param>
    /// <returns>string with colored characters</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColor(this string s, int hex) => 
        $"{hex.ControlChar()}{s}{CUSTOM_COLOR_END}";

    
    /// <summary>
    /// adds a color to the background of a string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="hex">the hexadecimal representation of the color</param>
    /// <returns>string with colored characters</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColorB(this string text, int hex) => 
        $"{hex.ControlCharB()}{text}{CUSTOM_BACKGROUND_COLOR_END}";

    
    /// <summary>
    /// adds a color to the characters of a string
    /// </summary>
    /// <param name="s">input string</param>
    /// <param name="red">red value of the color</param>
    /// <param name="green">green value of the color</param>
    /// <param name="blue">blue value of the color</param>
    /// <returns>string with colored characters</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColor(this string s, byte red, byte green, byte blue) => 
        $"{CUSTOM_COLOR_START}{red};{green};{blue}m{s}{CUSTOM_COLOR_END}";

    
    /// <summary>
    /// adds a color to the background of a string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="red">red value of the color</param>
    /// <param name="green">green value of the color</param>
    /// <param name="blue">blue value of the color</param>
    /// <returns>string with colored characters</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColorB(this string text, byte red, byte green, byte blue) => 
        $"{CUSTOM_BACKGROUND_COLOR_START}{red};{green};{blue}m{text}{CUSTOM_BACKGROUND_COLOR_END}";
    
    
    /// <summary>
    /// adds a color of the terminal palette to the text
    /// </summary>
    /// <returns>text with colors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColor(this string s, ConsoleColor color) => 
        $"{color.ControlChar()}{s}{PALETTE_COLOR_END}";

    
    /// <summary>
    /// adds a color of the terminal palette to the text's background
    /// </summary>
    /// <returns>text with colored background</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColorB(this string text, ConsoleColor color) => 
        $"{color.ControlCharB()}{text}{PALETTE_BACKGROUND_COLOR_END}";

    
    /// <summary>
    /// adds a color to the text
    /// </summary>
    /// <returns>colored text</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColor(this string s, Color color) => 
        $"{color.ControlChar}{s}{color.ControlCharEnd}";

    
    /// <summary>
    /// adds a colored background to the text
    /// </summary>
    /// <returns>text with colored background</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColorB(this string text, Color color) => 
        $"{color.ControlCharB}{text}{color.ControlCharEndB}";

    
    /// <summary>
    /// adds a color to the text
    /// </summary>
    /// <returns>colored text</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColor(this string s, System.Drawing.Color color) => 
        s.AddColor(color.R, color.G, color.B);

    
    /// <summary>
    /// adds a colored background to the text
    /// </summary>
    /// <returns>text with colored background</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColorB(this string text, System.Drawing.Color color) {
        return text.AddColorB(color.R, color.G, color.B);
    }

    
    /// <summary>
    /// colors the string using symbols defined by the symbol variable of the tuple
    /// </summary>
    /// <param name="s">the string to be colored</param>
    /// <param name="colors">array of tuples containing the symbol and the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColor(this string s, params (string symbol, Color color)[] colors) => 
        colors.Aggregate(s, (current, c) => current.Replace(c.symbol, c.color.ControlChar));
    
    
    /// <summary>
    /// adds color to both the text and the background
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColor(this string s, Color text, Color background) => 
        s.AddColor(text).AddColorB(background);

    
    /// <summary>
    /// adds styles to text using html-like tags
    /// </summary>
    /// <param name="s">input text with tags</param>
    /// <returns>text that when printed to console has styles</returns>
    public static string ApplyStyles(this string s) {
        s = s.Replace("<b>", BOLD_START);
        s = s.Replace("</b>", BOLD_END);
        s = s.Replace("<i>", ITALIC_START);
        s = s.Replace("</i>", ITALIC_END);
        s = s.Replace("<u>", UNDERLINE_START);
        s = s.Replace("</u>", UNDERLINE_END);
        s = s.Replace("<f>", FAINT_START);
        s = s.Replace("</f>", FAINT_END);
        s = s.Replace("<n>", NEGATIVE_START);
        s = s.Replace("</n>", NEGATIVE_END);
        s = s.Replace("<s>", STRIKETHROUGH_START);
        s = s.Replace("</s>", STRIKETHROUGH_END);
        return s;
    }

    /// <summary>
    /// applies colors to a string, colors are specified in the xml tag format i.e. <c>&lt;f#RRGGBB&gt;&lt;/f#&gt;</c>
    /// for text coloring or <c>&lt;b#RRGGBB&gt;&lt;/b#&gt;</c> for background coloring where <c>RRGGBB</c> represents
    /// the color in hexadecimal, you can also use tags with the console color names,
    /// as closing tags just use <c>&lt;/color&gt;</c> 
    /// </summary>
    /// <param name="s">string to be colored</param>
    /// <returns>colored string</returns>
    public static string ApplyColors(this string s) {
        
        s = Regex.Replace(s, "<f#.{6}>", match => {
            string hex = match.Value.Substring(3, 6);
            int i = int.Parse(hex, NumberStyles.HexNumber);
            return i.ControlChar();
        });
        
        s = Regex.Replace(s, "<b#.{6}>", match => {
            string hex = match.Value.Substring(3, 6);
            int i = int.Parse(hex, NumberStyles.HexNumber);
            return i.ControlCharB();
        });

        s = s.Replace("</f#>", CUSTOM_COLOR_END);
        s = s.Replace("</b#>", CUSTOM_BACKGROUND_COLOR_END);

        s = s.Replace("<f-black>", PALETTE_COLOR_BLACK);
        s = s.Replace("<f-dark-red>", PALETTE_COLOR_DARK_RED);
        s = s.Replace("<f-dark-green>", PALETTE_COLOR_DARK_GREEN);
        s = s.Replace("<f-dark-yellow>", PALETTE_COLOR_DARK_YELLOW);
        s = s.Replace("<f-dark-blue>", PALETTE_COLOR_DARK_BLUE);
        s = s.Replace("<f-dark-magenta>", PALETTE_COLOR_DARK_MAGENTA);
        s = s.Replace("<f-dark-cyan>", PALETTE_COLOR_DARK_CYAN);
        s = s.Replace("<f-gray>", PALETTE_COLOR_GRAY);
        s = s.Replace("<f-dark-gray>", PALETTE_COLOR_DARK_GRAY);
        s = s.Replace("<f-red>", PALETTE_COLOR_RED);
        s = s.Replace("<f-green>", PALETTE_COLOR_GREEN);
        s = s.Replace("<f-yellow>", PALETTE_COLOR_YELLOW);
        s = s.Replace("<f-blue>", PALETTE_COLOR_BLUE);
        s = s.Replace("<f-magenta>", PALETTE_COLOR_MAGENTA);
        s = s.Replace("<f-cyan>", PALETTE_COLOR_CYAN);
        s = s.Replace("<f-white>", PALETTE_COLOR_WHITE);
        s = s.Replace("</f-color>", PALETTE_COLOR_END);
        
        s = s.Replace("<b-black>", PALETTE_BACKGROUND_COLOR_BLACK);
        s = s.Replace("<b-dark-red>", PALETTE_BACKGROUND_COLOR_DARK_RED);
        s = s.Replace("<b-dark-green>", PALETTE_BACKGROUND_COLOR_DARK_GREEN);
        s = s.Replace("<b-dark-yellow>", PALETTE_BACKGROUND_COLOR_DARK_YELLOW);
        s = s.Replace("<b-dark-blue>", PALETTE_BACKGROUND_COLOR_DARK_BLUE);
        s = s.Replace("<b-dark-magenta>", PALETTE_BACKGROUND_COLOR_DARK_MAGENTA);
        s = s.Replace("<b-dark-cyan>", PALETTE_BACKGROUND_COLOR_DARK_CYAN);
        s = s.Replace("<b-gray>", PALETTE_BACKGROUND_COLOR_GRAY);
        s = s.Replace("<b-dark-gray>", PALETTE_BACKGROUND_COLOR_DARK_GRAY);
        s = s.Replace("<b-red>", PALETTE_BACKGROUND_COLOR_RED);
        s = s.Replace("<b-green>", PALETTE_BACKGROUND_COLOR_GREEN);
        s = s.Replace("<b-yellow>", PALETTE_BACKGROUND_COLOR_YELLOW);
        s = s.Replace("<b-blue>", PALETTE_BACKGROUND_COLOR_BLUE);
        s = s.Replace("<b-magenta>", PALETTE_BACKGROUND_COLOR_MAGENTA);
        s = s.Replace("<b-cyan>", PALETTE_BACKGROUND_COLOR_CYAN);
        s = s.Replace("<b-white>", PALETTE_BACKGROUND_COLOR_WHITE);
        s = s.Replace("</b-color>", PALETTE_BACKGROUND_COLOR_END);
        
        return s;
    }

    
    /// <summary>
    /// adds text styles to the input string
    /// </summary>
    /// <param name="s">input string</param>
    /// <param name="styles">styles applied to the text</param>
    /// <returns>string with the styles applied</returns>
    /// <exception cref="ArgumentOutOfRangeException">an invalid style was inputted</exception>
    public static string AddStyle(this string s, int styles) {
        if ((styles & 0b000001) == 0b000001) s = s.AddStyle(TextStyles.BOLD);
        if ((styles & 0b000010) == 0b000010) s = s.AddStyle(TextStyles.ITALIC);
        if ((styles & 0b000100) == 0b000100) s = s.AddStyle(TextStyles.UNDERLINE);
        if ((styles & 0b001000) == 0b001000) s = s.AddStyle(TextStyles.FAINT);
        if ((styles & 0b010000) == 0b010000) s = s.AddStyle(TextStyles.NEGATIVE);
        if ((styles & 0b100000) == 0b100000) s = s.AddStyle(TextStyles.STRIKETHROUGH);
        
        return s;
    }

    
    /// <summary>
    /// adds a single text style to the input string
    /// </summary>
    /// <param name="s">input string</param>
    /// <param name="style">style applied to the text</param>
    /// <returns>string with the style applied</returns>
    /// <exception cref="ArgumentOutOfRangeException">an invalid style was inputted</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddStyle(this string s, TextStyles style) {
        return style switch {
            TextStyles.BOLD => $"{BOLD_START}{s}{BOLD_END}",
            TextStyles.ITALIC => $"{ITALIC_START}{s}{ITALIC_END}",
            TextStyles.UNDERLINE => $"{UNDERLINE_START}{s}{UNDERLINE_END}",
            TextStyles.FAINT => $"{FAINT_START}{s}{FAINT_END}",
            TextStyles.NEGATIVE => $"{NEGATIVE_START}{s}{NEGATIVE_END}",
            TextStyles.STRIKETHROUGH => $"{STRIKETHROUGH_START}{s}{STRIKETHROUGH_END}",
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };
    }


    /// <summary>
    /// adds styles to single character string
    /// </summary>
    /// <param name="c">input single-character string</param>
    /// <param name="styles">styles applied to text</param>
    /// <returns>styled string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddStyle(this char c, int styles) => (c + "").AddStyle(styles);
    
    
    /// <summary>
    /// returns string containing ansi escape sequence coloring the text
    /// </summary>    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    /// returns string containing ansi escape sequence coloring the text
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ControlChar(this int color) =>
        $"{CUSTOM_COLOR_START}{(byte)(color >> 16)};{(byte)(color >> 8)};{(byte)color}m";

    
    /// <summary>
    /// returns string containing ansi escape sequence coloring background
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ControlCharB(this int color) =>
        $"{CUSTOM_BACKGROUND_COLOR_START}{(byte)(color >> 16)};{(byte)(color >> 8)};{(byte)color}m";

    
    /// <summary>
    /// adds styles to a string using the <see cref="Style"/> structure
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddStyle(this string s, Style style) => 
        s.AddStyle(style.Styles).AddColor(style.FColor, style.BColor);
    
    
    /// <summary>
    /// adds styles to a string using the <see cref="Style"/> structure
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddStyle(this char s, Style style) => 
        s.AddStyle(style.Styles).AddColor(style.FColor, style.BColor);


    /// <summary>
    /// returns the total count of the printable / visible characters of a string
    /// (for example ansi escape characters are not counted)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int VisibleLength(this string text) =>
        Regex.Replace(text, @"\e([^m]*)m", "").Length;
}