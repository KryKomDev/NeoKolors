//
// NeoKolors
// Copyright (c) 2025 KryKom
//

#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using NeoKolors.Common.Util;
using static NeoKolors.Common.EscapeCodes;
using static NeoKolors.Common.TextStyles;

namespace NeoKolors.Common;

public static class StringEffects {

    private const string COMPOSITE = StringSyntaxAttribute.CompositeFormat;

    /// <param name="s">input string</param>
    extension(string s) {
        
        /// <summary>
        /// adds a color to the characters of a string
        /// </summary>
        /// <param name="hex">the hexadecimal representation of the color</param>
        /// <returns>string with colored characters</returns>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AddColor(int hex) => 
            $"{hex.ControlChar()}{s}{TEXT_COLOR_RESET}";

        /// <summary>
        /// adds a color to the characters of a string
        /// </summary>
        /// <param name="hex">the hexadecimal representation of the color</param>
        /// <returns>string with colored characters</returns>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AddColor(uint hex) => 
            $"{hex.ControlChar()}{s}{TEXT_COLOR_RESET}";

        /// <summary>
        /// adds a color to the background of a string
        /// </summary>
        /// <param name="hex">the hexadecimal representation of the color</param>
        /// <returns>string with colored characters</returns>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AddColorB(int hex) => 
            $"{hex.ControlCharB()}{s}{BCKG_COLOR_RESET}";

        /// <summary>
        /// adds a color to the characters of a string
        /// </summary>
        /// <param name="red">red value of the color</param>
        /// <param name="green">green value of the color</param>
        /// <param name="blue">blue value of the color</param>
        /// <returns>string with colored characters</returns>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AddColor(byte red, byte green, byte blue) => 
            $"{CUSTOM_TEXT_COLOR_FORMAT.Format(red, green, blue)}{s}{TEXT_COLOR_RESET}";

        /// <summary>
        /// adds a color to the background of a string
        /// </summary>
        /// <param name="red">red value of the color</param>
        /// <param name="green">green value of the color</param>
        /// <param name="blue">blue value of the color</param>
        /// <returns>string with colored characters</returns>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AddColorB(byte red, byte green, byte blue) => 
            $"{CUSTOM_BCKG_COLOR_FORMAT.Format(red, green, blue)}{s}{BCKG_COLOR_RESET}";

        /// <summary>
        /// adds a color of the terminal palette to the text
        /// </summary>
        /// <returns>text with colors</returns>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AddColor(NKConsoleColor color) => 
            $"{color.ControlChar()}{s}{TEXT_COLOR_RESET}";

        /// <summary>
        /// adds a color of the terminal palette to the text's background
        /// </summary>
        /// <returns>text with colored background</returns>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AddColorB(NKConsoleColor color) => 
            $"{color.ControlCharB()}{s}{BCKG_COLOR_RESET}";

        /// <summary>
        /// adds a color of the terminal palette to the text
        /// </summary>
        /// <returns>text with colors</returns>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AddColor(ConsoleColor color) => 
            $"{color.ControlChar()}{s}{TEXT_COLOR_RESET}";

        /// <summary>
        /// adds a color of the terminal palette to the text's background
        /// </summary>
        /// <returns>text with colored background</returns>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AddColorB(ConsoleColor color) => 
            $"{color.ControlCharB()}{s}{BCKG_COLOR_RESET}";

        /// <summary>
        /// adds a color to the text
        /// </summary>
        /// <returns>colored text</returns>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AddColor(NKColor color) => 
            $"{color.Text}{s}{TEXT_COLOR_RESET}";

        /// <summary>
        /// adds a colored background to the text
        /// </summary>
        /// <returns>text with colored background</returns>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AddColorB(NKColor color) => 
            $"{color.Bckg}{s}{BCKG_COLOR_RESET}";

        /// <summary>
        /// adds a color to the text
        /// </summary>
        /// <returns>colored text</returns>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AddColor(System.Drawing.Color color) => 
            s.AddColor(color.R, color.G, color.B);

        /// <summary>
        /// adds a colored background to the text
        /// </summary>
        /// <returns>text with colored background</returns>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AddColorB(System.Drawing.Color color) => 
            s.AddColorB(color.R, color.G, color.B);

        /// <summary>
        /// adds color to both the text and the background
        /// </summary>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AddColor(NKColor text, NKColor background) => 
            s.AddColor(text).AddColorB(background);
    }


    /// <param name="s">The input string containing format items to be replaced.</param>
    extension([StringSyntax(COMPOSITE)] string s) {
        
        /// <summary>
        /// Replaces format items in the input string with the corresponding color values provided in the array of colors.
        /// </summary>
        /// <param name="colors">An array of NKColor instances to format and replace in the input string.</param>
        /// <returns>A string with the format items replaced by the corresponding color representations.</returns>
        [System.Diagnostics.Contracts.Pure]
        [StringFormatMethod(nameof(s))]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string AddColors(params NKColor[] colors) => 
            string.Format(s, colors.Cast<object?>().ToArray());

    }


    /// <param name="s">input string with style and color tags</param>
    extension(string s) {
        /// <summary>
        /// applies styles (using the <see cref="ApplyStyles"/>) and colors
        /// (using the <see cref="ApplyColors"/>) to the string
        /// </summary>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ApplyEffects() => 
            s.ApplyStyles().ApplyColors();

        /// <summary>
        /// adds styles to text using html-like tags
        /// </summary>
        /// <returns>text that when printed to console has styles</returns>
        [System.Diagnostics.Contracts.Pure]
        public string ApplyStyles() {
            s = s.Replace("<b>", BOLD_START);
            s = s.Replace("</b>", BOLD_END);
            s = s.Replace("<i>", ITALIC_START);
            s = s.Replace("</i>", ITALIC_END);
            s = s.Replace("<u>", UNDERLINE_START);
            s = s.Replace("</u>", UNDERLINE_END);
            s = s.Replace("<f>", FAINT_START);
            s = s.Replace("</f>", FAINT_END);
            s = s.Replace("<l>", BLINK_START);
            s = s.Replace("</l>", BLINK_END);
            s = s.Replace("<n>", NEGATIVE_START);
            s = s.Replace("</n>", NEGATIVE_END);
            s = s.Replace("<v>", INVISIBLE_START);
            s = s.Replace("</v>", INVISIBLE_END);
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
        /// <returns>colored string</returns>
        [System.Diagnostics.Contracts.Pure]
        public string ApplyColors() {
        
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
            s = s.Replace("</f-color>", TEXT_COLOR_RESET);
            s = s.Replace("</f#>", TEXT_COLOR_RESET);
        
            s = s.Replace("<b-black>", PALETTE_BCKG_COLOR_BLACK);
            s = s.Replace("<b-dark-red>", PALETTE_BCKG_COLOR_DARK_RED);
            s = s.Replace("<b-dark-green>", PALETTE_BCKG_COLOR_DARK_GREEN);
            s = s.Replace("<b-dark-yellow>", PALETTE_BCKG_COLOR_DARK_YELLOW);
            s = s.Replace("<b-dark-blue>", PALETTE_BCKG_COLOR_DARK_BLUE);
            s = s.Replace("<b-dark-magenta>", PALETTE_BCKG_COLOR_DARK_MAGENTA);
            s = s.Replace("<b-dark-cyan>", PALETTE_BCKG_COLOR_DARK_CYAN);
            s = s.Replace("<b-gray>", PALETTE_BCKG_COLOR_GRAY);
            s = s.Replace("<b-dark-gray>", PALETTE_BCKG_COLOR_DARK_GRAY);
            s = s.Replace("<b-red>", PALETTE_BCKG_COLOR_RED);
            s = s.Replace("<b-green>", PALETTE_BCKG_COLOR_GREEN);
            s = s.Replace("<b-yellow>", PALETTE_BCKG_COLOR_YELLOW);
            s = s.Replace("<b-blue>", PALETTE_BCKG_COLOR_BLUE);
            s = s.Replace("<b-magenta>", PALETTE_BCKG_COLOR_MAGENTA);
            s = s.Replace("<b-cyan>", PALETTE_BCKG_COLOR_CYAN);
            s = s.Replace("<b-white>", PALETTE_BCKG_COLOR_WHITE);
            s = s.Replace("</b-color>", BCKG_COLOR_RESET);
            s = s.Replace("</b#>", BCKG_COLOR_RESET);
        
            return s;
        }

        /// <summary>
        /// adds a single text style to the input string
        /// </summary>
        /// <param name="style">style applied to the text</param>
        /// <returns>string with the style applied</returns>
        /// <exception cref="ArgumentOutOfRangeException">an invalid style was inputted</exception>
        [System.Diagnostics.Contracts.Pure]
        public string AddStyle(TextStyles style) => $"{style.GetEscSeq()}{s}{style.GetNegEscSeq()}";
    }


    /// <summary>
    /// adds styles to single character string
    /// </summary>
    /// <param name="c">input single-character string</param>
    /// <param name="styles">styles applied to text</param>
    /// <returns>styled string</returns>
    [System.Diagnostics.Contracts.Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddStyle(this char c, TextStyles styles) => (c + "").AddStyle(styles);


    extension(NKConsoleColor color) {
        
        /// <summary>
        /// returns string containing ansi escape sequence coloring the text
        /// </summary>    
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ControlChar() => GetPaletteFColor((byte)color);

        /// <summary>
        /// returns string containing ansi escape sequence coloring background
        /// </summary>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ControlCharB() => GetPaletteBColor((byte)color);
        
        /// <summary>
        /// returns string containing ansi escape sequence coloring background
        /// </summary>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ControlCharU() => GetPaletteUColor((byte)color);
    }


    extension(ConsoleColor color) {
        
        /// <summary>
        /// returns string containing ansi escape sequence coloring the text
        /// </summary>    
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ControlChar() => 
            GetPaletteFColor((byte)ColorFormat.SystemToNK(color));

        /// <summary>
        /// returns string containing ansi escape sequence coloring background
        /// </summary>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ControlCharB() => 
            GetPaletteBColor((byte)ColorFormat.SystemToNK(color));
        
        /// <summary>
        /// returns string containing ansi escape sequence coloring background
        /// </summary>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ControlCharU() => 
            GetPaletteUColor((byte)ColorFormat.SystemToNK(color));
    }


    extension(int color) {
        /// <summary>
        /// returns string containing ansi escape sequence coloring the text
        /// </summary>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ControlChar() =>
            CUSTOM_TEXT_COLOR_FORMAT.Format((byte)(color >> 16), (byte)(color >> 8), (byte)color);

        /// <summary>
        /// returns string containing ansi escape sequence coloring background
        /// </summary>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ControlCharB() =>
            CUSTOM_BCKG_COLOR_FORMAT.Format((byte)(color >> 16), (byte)(color >> 8), (byte)color);
        
        /// <summary>
        /// returns string containing ansi escape sequence coloring background
        /// </summary>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ControlCharU() =>
            CUSTOM_UNDERLINE_COLOR_FORMAT.Format((byte)(color >> 16), (byte)(color >> 8), (byte)color);
    }

    extension(uint color) {
        /// <summary>
        /// returns string containing ansi escape sequence coloring the text
        /// </summary>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ControlChar() =>
            CUSTOM_TEXT_COLOR_FORMAT.Format((byte)(color >> 16), (byte)(color >> 8), (byte)color);

        /// <summary>
        /// returns string containing ansi escape sequence coloring background
        /// </summary>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ControlCharB() =>
            CUSTOM_BCKG_COLOR_FORMAT.Format((byte)(color >> 16), (byte)(color >> 8), (byte)color);

        /// <summary>
        /// returns string containing ansi escape sequence coloring background
        /// </summary>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ControlCharU() =>
            CUSTOM_UNDERLINE_COLOR_FORMAT.Format((byte)(color >> 16), (byte)(color >> 8), (byte)color);
    }


    /// <summary>
    /// adds styles to a string using the <see cref="NKStyle"/> structure
    /// </summary>
    [System.Diagnostics.Contracts.Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddStyle(this string s, NKStyle style) => 
        s.AddStyle(style.Styles).AddColor(style.FColor, style.BColor);
    
    
    /// <summary>
    /// adds styles to a string using the <see cref="NKStyle"/> structure
    /// </summary>
    [System.Diagnostics.Contracts.Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddStyle(this char s, NKStyle style) => 
        s.AddStyle(style.Styles).AddColor(style.FColor, style.BColor);
}