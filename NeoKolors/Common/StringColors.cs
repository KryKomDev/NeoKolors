//
// NeoKolors
// Copyright (c) 2024 KryKom
//

namespace NeoKolors.Common;

public class StringColors {
    
    /// <summary>
    /// adds a color to the characters of a string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="hex">the hexadecimal representation of the color</param>
    /// <returns>string with colored characters</returns>
    public static string AddColor(string text, int hex) {
        return $"\e[38;2;{(byte)(hex >> 16)};{(byte)(hex >> 8)};{(byte)hex}m{text}\e[0m";
    }

    /// <summary>
    /// adds a color to the background of a string
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="hex">the hexadecimal representation of the color</param>
    /// <returns>string with colored characters</returns>
    public static string AddColorB(string text, int hex) {
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
        return $"\e[48;2;{red};{green};{blue}m{text}\e[0m";
    }
}