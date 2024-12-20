//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using System.Text.RegularExpressions;

namespace NeoKolors.Common;

public static partial class StringEffects {
    
    /// <summary>
    /// adds a color to the characters of a string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="hex">the hexadecimal representation of the color</param>
    /// <returns>string with colored characters</returns>
    public static string AddColor(this string text, int hex) {
        text = AddTextStyles(text);
        return $"\e[38;2;{(byte)(hex >> 16)};{(byte)(hex >> 8)};{(byte)hex}m{text}\e[0m";
    }

    /// <summary>
    /// adds a color to the background of a string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="hex">the hexadecimal representation of the color</param>
    /// <returns>string with colored characters</returns>
    public static string AddColorB(this string text, int hex) {
        text = AddTextStyles(text);
        return $"\e[48;2;{(byte)(hex >> 16)};{(byte)(hex >> 8)};{(byte)hex}m{text}\e[0m";
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
        return $"\e\u005b\u00338;2;{red};{green};{blue}m{text}\e[0m";
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
        return $"\e[48;2;{red};{green};{blue}m{text}\e[0m";
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
        return color switch {
            ConsoleColor.Black => $"\e[38;5;0m{text}\e[38;5;7m",
            ConsoleColor.DarkRed => $"\e[38;5;1m{text}\e[38;5;7m",
            ConsoleColor.DarkGreen => $"\e[38;5;2m{text}\e[38;5;7m",
            ConsoleColor.DarkYellow => $"\e[38;5;3m{text}\e[38;5;7m",
            ConsoleColor.DarkBlue => $"\e[38;5;4m{text}\e[38;5;7m",
            ConsoleColor.DarkMagenta => $"\e[38;5;5m{text}\e[38;5;7m",
            ConsoleColor.DarkCyan => $"\e[38;5;6m{text}\e[38;5;7m",
            ConsoleColor.Gray => $"\e[38;5;7m{text}\e[38;5;7m",
            ConsoleColor.DarkGray => $"\e[38;5;8m{text}\e[38;5;7m",
            ConsoleColor.Red => $"\e[38;5;9m{text}\e[38;5;7m",
            ConsoleColor.Green => $"\e[38;5;10m{text}\e[38;5;7m",
            ConsoleColor.Yellow => $"\e[38;5;11m{text}\e[38;5;7m",
            ConsoleColor.Blue => $"\e[38;5;12m{text}\e[38;5;7m",
            ConsoleColor.Magenta => $"\e[38;5;13m{text}\e[38;5;7m",
            ConsoleColor.Cyan => $"\e[38;5;14m{text}\e[38;5;7m",
            ConsoleColor.White => $"\e[38;5;15m{text}\e[38;5;7m",
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };
    }
    
    /// <summary>
    /// adds a color of the terminal palette to the text's background
    /// </summary>
    /// <returns>text with colored background</returns>
    public static string AddColorB(this string text, ConsoleColor color) {
        text = AddTextStyles(text);
        return color switch {
            ConsoleColor.Black => $"\e[48;5;0m{text}\e[48;5;7m",
            ConsoleColor.DarkRed => $"\e[48;5;1m{text}\e[48;5;7m",
            ConsoleColor.DarkGreen => $"\e[48;5;2m{text}\e[48;5;7m",
            ConsoleColor.DarkYellow => $"\e[48;5;3m{text}\e[48;5;7m",
            ConsoleColor.DarkBlue => $"\e[48;5;4m{text}\e[48;5;7m",
            ConsoleColor.DarkMagenta => $"\e[48;5;5m{text}\e[48;5;7m",
            ConsoleColor.DarkCyan => $"\e[48;5;6m{text}\e[48;5;7m",
            ConsoleColor.Gray => $"\e[48;5;7m{text}\e[48;5;7m",
            ConsoleColor.DarkGray => $"\e[48;5;8m{text}\e[48;5;7m",
            ConsoleColor.Red => $"\e[48;5;9m{text}\e[48;5;7m",
            ConsoleColor.Green => $"\e[48;5;10m{text}\e[48;5;7m",
            ConsoleColor.Yellow => $"\e[48;5;11m{text}\e[48;5;7m",
            ConsoleColor.Blue => $"\e[48;5;12m{text}\e[48;5;7m",
            ConsoleColor.Magenta => $"\e[48;5;13m{text}\e[48;5;7m",
            ConsoleColor.Cyan => $"\e[48;5;14m{text}\e[48;5;7m",
            ConsoleColor.White => $"\e[48;5;15m{text}\e[48;5;7m",
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };
    }

    /// <summary>
    /// adds a color to the text
    /// </summary>
    /// <returns>colored text</returns>
    public static string AddColor(this string text, Color color) {
        return color.IsPaletteSafe 
            ? AddColor(text, (ConsoleColor)color.ConsoleColor!) 
            : AddColor(text, (int)color.CustomColor!);
    }
    
    /// <summary>
    /// adds a colored background to the text
    /// </summary>
    /// <returns>text with colored background</returns>
    public static string AddColorB(this string text, Color color) {
        return color.IsPaletteSafe 
            ? AddColorB(text, (ConsoleColor)color.ConsoleColor!) 
            : AddColorB(text, (int)color.CustomColor!);
    }

    public static int VisibleLength(this string text) {
        text = CONTROL_FINDER_REGEX().Replace(text, "");
        
        return text.Length;
    }

    [GeneratedRegex(@"\e([^m]*)m")]
    private static partial Regex CONTROL_FINDER_REGEX();
}