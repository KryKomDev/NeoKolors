//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.Contracts;
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

    /// <summary>
    /// Restricts an integer value to be within the specified minimum and maximum range.
    /// </summary>
    /// <param name="value">The integer value to be clamped.</param>
    /// <param name="min">The minimum allowable value.</param>
    /// <param name="max">The maximum allowable value.</param>
    /// <returns>
    /// The clamped value, which will be equal to <paramref name="min"/> if <paramref name="value"/> is less than <paramref name="min"/>,
    /// or equal to <paramref name="max"/> if <paramref name="value"/> is greater than <paramref name="max"/>.
    /// Otherwise, the original <paramref name="value"/> is returned.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static int Clamp(this int value, int min, int max) => 
        value < min ? min : value > max ? max : value;

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
        $"{(key.Modifiers.HasFlag(ConsoleModifiers.Control) ? "Ctrl + " : "")}" +
        $"{(key.Modifiers.HasFlag(ConsoleModifiers.Alt) ? "Alt + " : "")}" +
        $"{(key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? "Shift + " : "")}" + 
        $"{key.Key.ToString()}";

    /// <summary>
    /// Gets the length of the first dimension of a two-dimensional array.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="arr">The two-dimensional array whose first dimension length is to be retrieved.</param>
    /// <returns>The length of the first dimension of the array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static int Len0<T>(this T[,] arr) => arr.GetLength(0);

    /// <summary>
    /// Retrieves the number of elements in the second dimension of a two-dimensional array.
    /// </summary>
    /// <param name="arr">The two-dimensional array to retrieve the length of the second dimension from.</param>
    /// <typeparam name="T">The type of elements in the two-dimensional array.</typeparam>
    /// <returns>The number of elements in the second dimension of the array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public static int Len1<T>(this T[,] arr) => arr.GetLength(1);

    /// <summary>
    /// Converts the provided array of <see cref="NKColor"/> instances into a collection
    /// of <see cref="NKBckg"/> instances.
    /// </summary>
    /// <param name="colors">An array of <see cref="NKColor"/> instances to be converted.</param>
    /// <returns>An enumerable collection of <see cref="NKBckg"/> instances corresponding to the input colors.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<NKBckg> ToBckg(this NKColor[] colors) => colors.Select(c => new NKBckg(c));

    /// <summary>
    /// Converts the specified <see cref="NKColor"/> to its corresponding
    /// <see cref="NKBckg"/> representation.
    /// </summary>
    /// <param name="color">The <see cref="NKColor"/> instance to convert.</param>
    /// <returns>A new <see cref="NKBckg"/> instance representing the background color.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NKBckg ToBckg(this NKColor color) => new(color);

    /// <summary>
    /// Determines whether all elements, except the first one, in the provided collection
    /// satisfy a specified condition.
    /// </summary>
    /// <param name="strings">The collection of elements to evaluate.</param>
    /// <param name="predicate">A function that defines the condition each element, except the first one, must satisfy.</param>
    /// <typeparam name="TSource">The type of elements in the collection.</typeparam>
    /// <returns>
    /// <c>true</c> if all elements, excluding the first one, satisfy the condition; otherwise, <c>false</c>.
    /// </returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AllButFirst<TSource>(this IEnumerable<TSource> strings, Func<TSource, bool> predicate) =>
        strings.Skip(1).All(predicate);

    /// <summary>
    /// Determines whether all elements, except the first one, in the provided collection
    /// satisfy a specified condition.
    /// </summary>
    /// <param name="strings">The collection of elements to evaluate.</param>
    /// <param name="first">A function that defines the condition the first element must satisfy.</param>
    /// <param name="all">A function that defines the condition each element, except the first one, must satisfy.</param>
    /// <typeparam name="TSource">The type of elements in the collection.</typeparam>
    /// <returns>
    /// <c>true</c> if all elements satisfy their respective condition; otherwise, <c>false</c>.
    /// </returns>
    [Pure]
    public static bool FirstAndAll<TSource>(
        this IEnumerable<TSource> strings,
        Func<TSource, bool> first, 
        Func<TSource, bool> all) 
    {
        var e = strings as TSource[] ?? strings.ToArray();
        return e.Skip(1).All(all) && first(e.First());
    }

    public static IEnumerable<TResult> Select<TSource, TResult>(
        this IEnumerable<TSource> source, 
        Func<TSource, TResult> allSelector,
        Func<TSource, TResult>? firstSelector = null,
        Func<TSource, TResult>? lastSelector = null)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (allSelector is null) throw new ArgumentNullException(nameof(allSelector));
        var enumerable = source as TSource[] ?? source.ToArray();
        if (!enumerable.Any()) {
            yield break;
        }

        // first element
        if (firstSelector is null)
            yield return allSelector(enumerable.First());
        else
            yield return firstSelector(enumerable.First());

        // middle elements
        for (int i = 1; i < enumerable.Length - 1; i++)
            yield return allSelector(enumerable[i]);
        
        // last element
        if (lastSelector is null)
            yield return allSelector(enumerable.Last());
        else
            yield return lastSelector(enumerable.Last());
    }
}