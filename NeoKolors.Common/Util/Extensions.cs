//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Text;
using static NeoKolors.Common.EscapeCodes;
using static NeoKolors.Common.TextStyles;

namespace NeoKolors.Common.Util;

/// <summary>
/// Provides extension methods for applying styles and colors to strings and characters
/// using the NeoKolors framework.
/// </summary>
public static class Extensions {
    
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
        if (style.IsBold) s = $"{BOLD_START}{s}";
        if (style.IsItalic) s = $"{ITALIC_START}{s}";
        if (style.IsUnderline) s = $"{UNDERLINE_START}{s}";
        if (style.IsFaint) s = $"{FAINT_START}{s}";
        if (style.IsNegative) s = $"{NEGATIVE_START}{s}";
        if (style.IsStrikethrough) s = $"{STRIKETHROUGH_START}{s}";
        return s;
    }

    extension(TextStyles styles) {
        public bool IsBold =>          styles.HasFlag(BOLD);
        public bool IsItalic =>        styles.HasFlag(ITALIC);
        public bool IsUnderline =>     styles.HasFlag(UNDERLINE);
        public bool IsFaint =>         styles.HasFlag(FAINT);
        public bool IsNegative =>      styles.HasFlag(NEGATIVE);
        public bool IsStrikethrough => styles.HasFlag(STRIKETHROUGH);
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

    public static string ToString(ConsoleKeyInfo key) => 
        $"{(key.HasCtrl ? "Ctrl + " : "")}" +
        $"{(key.HasAlt ? "Alt + " : "")}" +
        $"{(key.HasShift ? "Shift + " : "")}" + 
        $"{key.Key.ToString()} => '{key.KeyChar}'";

    extension(ConsoleModifiers mods) {
        public bool HasShift => mods.HasFlag(ConsoleModifiers.Shift);
        public bool HasAlt => mods.HasFlag(ConsoleModifiers.Alt);
        public bool HasCtrl => mods.HasFlag(ConsoleModifiers.Control);
    }

    extension(ConsoleKeyInfo key) {
        public bool HasShift => key.Modifiers.HasFlag(ConsoleModifiers.Shift);
        public bool HasAlt => key.Modifiers.HasFlag(ConsoleModifiers.Alt);
        public bool HasCtrl => key.Modifiers.HasFlag(ConsoleModifiers.Control);
    }

    public static string GetEscSeq(this TextStyles styles) {
        if (styles == NONE) return string.Empty;
        
        var sb = new StringBuilder();
        sb.Append("\e[");
        sb.Append(styles.GetHasBold()          ? "1;" : "");
        sb.Append(styles.GetHasFaint()         ? "2;" : "");
        sb.Append(styles.GetHasItalic()        ? "3;" : "");
        sb.Append(styles.GetHasUnderline()     ? "4;" : "");
        sb.Append(styles.GetHasBlink()         ? "5;" : "");
        sb.Append(styles.GetHasNegative()      ? "6;" : "");
        sb.Append(styles.GetHasInvisible()     ? "7;" : "");
        sb.Append(styles.GetHasStrikethrough() ? "8;" : "");

        sb.Remove(sb.Length - 1, 1);
        sb.Append('m');
        
        return sb.ToString();
    }
    
    public static string GetOvrEscSeq(this TextStyles styles) {
        if (styles == NONE) return string.Empty;
        
        var sb = new StringBuilder();
        sb.Append("\e[0;");
        sb.Append(styles.GetHasBold()          ? "1;" : "");
        sb.Append(styles.GetHasFaint()         ? "2;" : "");
        sb.Append(styles.GetHasItalic()        ? "3;" : "");
        sb.Append(styles.GetHasUnderline()     ? "4;" : "");
        sb.Append(styles.GetHasBlink()         ? "5;" : "");
        sb.Append(styles.GetHasNegative()      ? "6;" : "");
        sb.Append(styles.GetHasInvisible()     ? "7;" : "");
        sb.Append(styles.GetHasStrikethrough() ? "8;" : "");

        sb.Remove(sb.Length - 1, 1);
        sb.Append('m');
        
        return sb.ToString();
    }

    public static string GetNegEscSeq(this TextStyles styles) {
        if (styles == NONE) return string.Empty;
        
        var sb = new StringBuilder();
        sb.Append("\e[");
        sb.Append(styles.GetHasBold()          ? "22;" : "");
        sb.Append(styles.GetHasFaint()         ? "22;" : "");
        sb.Append(styles.GetHasItalic()        ? "23;" : "");
        sb.Append(styles.GetHasUnderline()     ? "24;" : "");
        sb.Append(styles.GetHasBlink()         ? "25;" : "");
        sb.Append(styles.GetHasNegative()      ? "26;" : "");
        sb.Append(styles.GetHasInvisible()     ? "27;" : "");
        sb.Append(styles.GetHasStrikethrough() ? "28;" : "");

        sb.Remove(sb.Length - 1, 1);
        sb.Append('m');
        
        return sb.ToString();
    }
}