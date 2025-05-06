//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using static NeoKolors.Common.TextStyles;
using static NeoKolors.Common.EscapeCodes;

namespace NeoKolors.Tui;

/// <summary>
/// Provides extension methods for applying styles and colors to strings and characters
/// using the NeoKolors framework.
/// </summary>
internal static class Extensions {
    
    /// <summary>
    /// Applies the specified text style, foreground color, and background color
    /// from the provided <see cref="NKStyle"/> to the given string.
    /// </summary>
    /// <param name="s">The string to which the style and colors will be applied.</param>
    /// <param name="style">The <see cref="NKStyle"/>
    /// containing the styles, foreground color, and background color to apply.
    /// </param>
    /// <returns>A new string with the applied styles and colors.</returns>
    public static string AddCStyle(this string s, NKStyle style) =>
        s.AddCStyle(style.Styles).AddCColorF(style.FColor).AddCColorB(style.BColor);

    /// <summary>
    /// Applies the specified text style, foreground color, and background color
    /// from the provided <see cref="NKStyle"/> to the given character.
    /// </summary>
    /// <param name="c">The character to which the style and colors will be applied.</param>
    /// <param name="style">The <see cref="NKStyle"/> containing the styles, foreground color,
    /// and background color to apply.</param>
    /// <returns>A new string derived from the character with the applied styles and colors.</returns>
    public static string AddCStyle(this char c, NKStyle style) =>
        c.ToString().AddCStyle(style.Styles).AddCColorF(style.FColor).AddCColorB(style.BColor);

    /// <summary>
    /// Applies the specified foreground color from the provided <see cref="NKColor"/> to the given string.
    /// </summary>
    /// <param name="s">The string to which the foreground color will be applied.</param>
    /// <param name="color">The <see cref="NKColor"/> specifying the foreground color to apply.</param>
    /// <returns>A new string with the applied foreground color.</returns>
    public static string AddCColorF(this string s, NKColor color) => $"{color.Text}{s}";
    
    /// <summary>
    /// Applies the specified background color from the provided <see cref="NKColor"/> to the given string.
    /// </summary>
    /// <param name="s">The string to which the background color will be applied.</param>
    /// <param name="color">The <see cref="NKColor"/> containing the background color to apply.</param>
    /// <returns>A new string with the applied background color.</returns>
    public static string AddCColorB(this string s, NKColor color) => $"{color.Bckg}{s}";

    /// <summary>
    /// Applies the specified text style to the given string.
    /// </summary>
    /// <param name="s">The string to which the text style will be applied.</param>
    /// <param name="style">The <see cref="TextStyles"/> value specifying the styles
    /// to apply, such as bold, italic, or underline.</param>
    /// <returns>A new string with the specified text styles applied.</returns>
    public static string AddCStyle(this string s, TextStyles style) {
        if (style.HasFlag(BOLD)) s = $"{BOLD_START}{s}";
        if (style.HasFlag(ITALIC)) s = $"{ITALIC_START}{s}";
        if (style.HasFlag(UNDERLINE)) s = $"{UNDERLINE_START}{s}";
        if (style.HasFlag(FAINT)) s = $"{FAINT_START}{s}";
        if (style.HasFlag(NEGATIVE)) s = $"{NEGATIVE_START}{s}";
        if (style.HasFlag(STRIKETHROUGH)) s = $"{STRIKETHROUGH_START}{s}";
        return s;
    }

    /// <summary>
    /// Applies the specified text style to the given string.
    /// </summary>
    /// <param name="c">The string to which the text style will be applied.</param>
    /// <param name="style">The <see cref="TextStyles"/> value specifying the styles
    /// to apply, such as bold, italic, or underline.</param>
    /// <returns>A new string with the specified text styles applied.</returns>
    public static string AddCStyle(this char c, TextStyles style) => 
        c.ToString().AddCStyle(style);

    public static int Between(this int value, int min, int max) => value < min ? min : value > max ? max : value;

    /// <summary>
    /// Calculates the distance between the specified value and a given range defined by minimum and maximum bounds.
    /// </summary>
    /// <param name="value">The integer value to evaluate against the range.</param>
    /// <param name="min">The minimum boundary of the range.</param>
    /// <param name="max">The maximum boundary of the range.</param>
    /// <returns>
    /// A non-negative integer representing the distance between the value and the range.
    /// Returns 0 if the value is within the range.
    /// </returns>
    public static int RangeDist(this int value, int min, int max) =>
        value < min ? min - value : value > max ? value - max : 0;

    public static int InRangeLength(this int value, int offset, int min, int max) =>
        offset < min ? min - value : offset > max ? 0 : value;
}