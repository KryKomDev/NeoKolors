//
// NeoKolors
// Copyright (c) 2025 KryKom
//

#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static NeoKolors.Common.EscapeCodes;
using static NeoKolors.Common.TextStyles;

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
        $"{hex.ControlChar()}{s}{TEXT_COLOR_END}";

    
    /// <summary>
    /// adds a color to the background of a string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="hex">the hexadecimal representation of the color</param>
    /// <returns>string with colored characters</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColorB(this string text, int hex) => 
        $"{hex.ControlCharB()}{text}{BACKGROUND_COLOR_END}";

    
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
        $"{CUSTOM_COLOR_START}{red};{green};{blue}m{s}{TEXT_COLOR_END}";

    
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
        $"{CUSTOM_BACKGROUND_COLOR_START}{red};{green};{blue}m{text}{BACKGROUND_COLOR_END}";
    
    
    /// <summary>
    /// adds a color of the terminal palette to the text
    /// </summary>
    /// <returns>text with colors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColor(this string s, NKConsoleColor color) => 
        $"{color.ControlChar()}{s}{TEXT_COLOR_END}";

    
    /// <summary>
    /// adds a color of the terminal palette to the text's background
    /// </summary>
    /// <returns>text with colored background</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColorB(this string text, NKConsoleColor color) => 
        $"{color.ControlCharB()}{text}{BACKGROUND_COLOR_END}";

    
    /// <summary>
    /// adds a color of the terminal palette to the text
    /// </summary>
    /// <returns>text with colors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColor(this string s, ConsoleColor color) => 
        $"{color.ControlChar()}{s}{TEXT_COLOR_END}";

    
    /// <summary>
    /// adds a color of the terminal palette to the text's background
    /// </summary>
    /// <returns>text with colored background</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColorB(this string text, ConsoleColor color) => 
        $"{color.ControlCharB()}{text}{BACKGROUND_COLOR_END}";
    
    /// <summary>
    /// adds a color to the text
    /// </summary>
    /// <returns>colored text</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColor(this string s, NKColor color) => 
        $"{color.Text}{s}{TEXT_COLOR_END}";

    
    /// <summary>
    /// adds a colored background to the text
    /// </summary>
    /// <returns>text with colored background</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColorB(this string text, NKColor color) => 
        $"{color.Bckg}{text}{BACKGROUND_COLOR_END}";

    
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
    public static string AddColorB(this string text, System.Drawing.Color color) => 
        text.AddColorB(color.R, color.G, color.B);


    /// <summary>
    /// colors the string using symbols defined by the symbol variable of the tuple
    /// </summary>
    /// <param name="s">the string to be colored</param>
    /// <param name="colors">array of tuples containing the symbol and the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColor(this string s, params (string symbol, NKColor color)[] colors) => 
        colors.Aggregate(s, (current, c) => current.Replace(c.symbol, c.color.Text));
    
    
    /// <summary>
    /// adds color to both the text and the background
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddColor(this string s, NKColor text, NKColor background) => 
        s.AddColor(text).AddColorB(background);


    /// <summary>
    /// colors the string s using the composition formatting
    /// </summary>
    /// <param name="s">the string to be colored</param>
    /// <param name="colors">an ordered list of the colors</param>
    public static string AddColors(
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] 
        this string s, params NKColor[] colors) 
    {
        object[] sc = new object[colors.Length];
        
        for (int i = 0; i < colors.Length; i++)
            sc[i] = colors[i].Text;
        
        return string.Format(s, sc);
    }
    
    /// <summary>
    /// colors the string's background using the composition formatting
    /// </summary>
    /// <param name="s">the string to be colored</param>
    /// <param name="colors">an ordered list of the colors</param>
    public static string AddColorsB(
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)] 
            this string s, params NKColor[] colors) 
    {
        object[] sc = new object[colors.Length];
        
        for (int i = 0; i < colors.Length; i++)
            sc[i] = colors[i].Bckg;
        
        return string.Format(s, sc);
    }
    
    
    /// <summary>
    /// applies styles (using the <see cref="ApplyStyles"/>) and colors
    /// (using the <see cref="ApplyColors"/>) to the string
    /// </summary>
    /// <param name="s">input string with style and color tags</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ApplyEffects(this string s) => 
        s.ApplyStyles().ApplyColors();
    
    
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
        s = s.Replace("</f-color>", TEXT_COLOR_END);
        s = s.Replace("</f#>", TEXT_COLOR_END);
        
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
        s = s.Replace("</b-color>", BACKGROUND_COLOR_END);
        s = s.Replace("</b#>", BACKGROUND_COLOR_END);
        
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
        if ((styles & 0b000001) == 0b000001) s = s.AddStyle(BOLD);
        if ((styles & 0b000010) == 0b000010) s = s.AddStyle(ITALIC);
        if ((styles & 0b000100) == 0b000100) s = s.AddStyle(UNDERLINE);
        if ((styles & 0b001000) == 0b001000) s = s.AddStyle(FAINT);
        if ((styles & 0b010000) == 0b010000) s = s.AddStyle(NEGATIVE);
        if ((styles & 0b100000) == 0b100000) s = s.AddStyle(STRIKETHROUGH);
        
        return s;
    }

    
    /// <summary>
    /// adds a single text style to the input string
    /// </summary>
    /// <param name="s">input string</param>
    /// <param name="style">style applied to the text</param>
    /// <returns>string with the style applied</returns>
    /// <exception cref="ArgumentOutOfRangeException">an invalid style was inputted</exception>
    public static string AddStyle(this string s, TextStyles style) {
        if (style.HasFlag(BOLD)) s = $"{BOLD_START}{s}{BOLD_END}";
        if (style.HasFlag(ITALIC)) s = $"{ITALIC_START}{s}{ITALIC_END}";
        if (style.HasFlag(UNDERLINE)) s = $"{UNDERLINE_START}{s}{UNDERLINE_END}";
        if (style.HasFlag(FAINT)) s = $"{FAINT_START}{s}{FAINT_END}";
        if (style.HasFlag(NEGATIVE)) s = $"{NEGATIVE_START}{s}{NEGATIVE_END}";
        if (style.HasFlag(STRIKETHROUGH)) s = $"{STRIKETHROUGH_START}{s}{STRIKETHROUGH_END}";
        return s;
    }


    /// <summary>
    /// adds styles to single character string
    /// </summary>
    /// <param name="c">input single-character string</param>
    /// <param name="styles">styles applied to text</param>
    /// <returns>styled string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddStyle(this char c, TextStyles styles) => (c + "").AddStyle(styles);


    /// <summary>
    /// returns string containing ansi escape sequence coloring the text
    /// </summary>    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ControlChar(this NKConsoleColor color) => GetPaletteFColor((byte)color);
    
    
    /// <summary>
    /// returns string containing ansi escape sequence coloring background
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ControlCharB(this NKConsoleColor color) => GetPaletteBColor((byte)color);
    
    
    /// <summary>
    /// returns string containing ansi escape sequence coloring the text
    /// </summary>    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ControlChar(this ConsoleColor color) => 
        GetPaletteFColor((byte)ColorFormat.SystemToNK(color));

    
    /// <summary>
    /// returns string containing ansi escape sequence coloring background
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ControlCharB(this ConsoleColor color) => 
        GetPaletteBColor((byte)ColorFormat.SystemToNK(color));

    
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
    /// adds styles to a string using the <see cref="NKStyle"/> structure
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddStyle(this string s, NKStyle style) => 
        s.AddStyle(style.Styles).AddColor(style.FColor, style.BColor);
    
    
    /// <summary>
    /// adds styles to a string using the <see cref="NKStyle"/> structure
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddStyle(this char s, NKStyle style) => 
        s.AddStyle(style.Styles).AddColor(style.FColor, style.BColor);
}