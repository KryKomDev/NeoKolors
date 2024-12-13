//
// NeoKolors
// Copyright (c) 2024 KryKom
//

namespace NeoKolors.Common;

public static class StringEffects {
    
    /// <summary>
    /// adds a color to the characters of a string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="hex">the hexadecimal representation of the color</param>
    /// <returns>string with colored characters</returns>
    public static string AddColor(string text, int hex) {
        text = AddTextStyles(text);
        return $"\e[38;2;{(byte)(hex >> 16)};{(byte)(hex >> 8)};{(byte)hex}m{text}\e[0m";
    }

    /// <summary>
    /// adds a color to the background of a string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="hex">the hexadecimal representation of the color</param>
    /// <returns>string with colored characters</returns>
    public static string AddColorB(string text, int hex) {
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
    public static string AddColor(string text, byte red, byte green, byte blue) {
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
    public static string AddColorB(string text, byte red, byte green, byte blue) {
        text = AddTextStyles(text);
        return $"\e[48;2;{red};{green};{blue}m{text}\e[0m";
    }

    /// <summary>
    /// adds styles to text using html-like tags
    /// </summary>
    /// <param name="text">input text with tags</param>
    /// <returns>text that when printed to console has styles</returns>
    public static string AddTextStyles(string text) {
        text = text.Replace("<b>", "\e[1m");
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
}