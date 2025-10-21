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
    
    /// <summary>
    /// Gets the length of the first dimension of a two-dimensional array.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="arr">The two-dimensional array whose first dimension length is to be retrieved.</param>
    /// <returns>The length of the first dimension of the array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    [Obsolete("Replaced by Len0 property.")]
    public static int GetLen0<T>(this T[,] arr) => arr.GetLength(0);

    /// <summary>
    /// Retrieves the number of elements in the second dimension of a two-dimensional array.
    /// </summary>
    /// <param name="arr">The two-dimensional array to retrieve the length of the second dimension from.</param>
    /// <typeparam name="T">The type of elements in the two-dimensional array.</typeparam>
    /// <returns>The number of elements in the second dimension of the array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]    
    [Obsolete("Replaced by Len1 property.")]
    public static int GetLen1<T>(this T[,] arr) => arr.GetLength(1);

    extension<T>(T[,] value) {
        
        /// <summary>
        /// Gets the length of the first dimension of a two-dimensional array.
        /// </summary>
        [Pure]
        [Obsolete("Use Metriks extension properties instead.")]
        public int Len0 => value.GetLength(0);
        
        /// <summary>
        /// Gets the length of the second dimension of a two-dimensional array.
        /// </summary>
        [Pure]
        [Obsolete("Use Metriks extension properties instead.")]
        public int Len1 => value.GetLength(1);
    }

    extension<T>(T[,,] value) {
        
        /// <summary>
        /// Gets the length of the first dimension of a three-dimensional array.
        /// </summary>
        [Pure]
        [Obsolete("Use Metriks extension properties instead.")]
        public int Len0 => value.GetLength(0);
        
        /// <summary>
        /// Gets the length of the second dimension of a three-dimensional array.
        /// </summary>
        [Pure]
        [Obsolete("Use Metriks extension properties instead.")]
        public int Len1 => value.GetLength(1);
        
        /// <summary>
        /// Gets the length of the third dimension of a three-dimensional array.
        /// </summary>
        [Pure]
        [Obsolete("Use Metriks extension properties instead.")]
        public int Len2 => value.GetLength(2);
    }
    
    extension<T>(T[,,,] value) {
        
        /// <summary>
        /// Gets the length of the first dimension of a four-dimensional array.
        /// </summary>
        [Pure]
        [Obsolete("Use Metriks extension properties instead.")]
        public int Len0 => value.GetLength(0);
        
        /// <summary>
        /// Gets the length of the second dimension of a four-dimensional array.
        /// </summary>
        [Pure]
        [Obsolete("Use Metriks extension properties instead.")]
        public int Len1 => value.GetLength(1);
        
        /// <summary>
        /// Gets the length of the third dimension of a four-dimensional array.
        /// </summary>
        [Pure]
        [Obsolete("Use Metriks extension properties instead.")]
        public int Len2 => value.GetLength(2);
        
        /// <summary>
        /// Gets the length of the fourth dimension of a four-dimensional array.
        /// </summary>
        [Pure]
        [Obsolete("Use Metriks extension properties instead.")]
        public int Len3 => value.GetLength(3);
    }
    
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

    /// <summary>
    /// Selects elements from the given source based on the provided selectors and their positions
    /// (first, middle, last). Allows customization of the selection logic based on position or default options.
    /// </summary>
    /// <param name="source">The source collection to enumerate elements from.</param>
    /// <param name="allSelector">A function to select elements applying to all positions except where overridden.</param>
    /// <param name="firstSelector">An optional function to select the first element. If null, the allSelector is used.</param>
    /// <param name="lastSelector">An optional function to select the last element. If null, the allSelector is used.</param>
    /// <param name="defaultTo">Used when the source enumerable has only 1 element. If -1 uses the firstSelector, if 0 uses the allSelector, if 1 uses the lastSelector.</param>
    /// <typeparam name="TSource">The type of elements in the source collection.</typeparam>
    /// <typeparam name="TResult">The type of elements in the resulting collection.</typeparam>
    /// <returns>An enumerable collection of selected elements based on the provided selectors and logic.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the source or allSelector parameters are null.</exception>
    public static IEnumerable<TResult> Select<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, TResult> allSelector,
        Func<TSource, TResult>? firstSelector = null,
        Func<TSource, TResult>? lastSelector = null,
        int defaultTo = 0) 
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (allSelector is null) throw new ArgumentNullException(nameof(allSelector));
        
        var enumerable = source as TSource[] ?? source.ToArray();
        
        if (!enumerable.Any()) {
            yield break;
        }
        
        if (enumerable.Length == 1) {
            yield return defaultTo switch {
                -1 when firstSelector is not null => firstSelector(enumerable.First()),
                1 when lastSelector is not null => lastSelector(enumerable.First()),
                _ => allSelector(enumerable.First())
            };
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

    /// <summary>
    /// Converts a character to a ConsoleKeyInfo instance.
    /// </summary>
    /// <param name="c">The character to convert.</param>
    /// <returns>A ConsoleKeyInfo instance representing the character.</returns>
    public static ConsoleKeyInfo ToConsoleKeyInfo(this char c) {
        ConsoleKey key;
        ConsoleModifiers modifiers = 0;

        // Handle special characters and keys
        switch (c) {
            case '\b':
                key = ConsoleKey.Backspace;
                break;
            case '\t':
                key = ConsoleKey.Tab;
                break;
            case '\r':
            case '\n':
                key = ConsoleKey.Enter;
                break;
            case '\u001b': // ESC
                key = ConsoleKey.Escape;
                break;
            case ' ':
                key = ConsoleKey.Spacebar;
                break;
            case '0': key = ConsoleKey.D0; break;
            case '1': key = ConsoleKey.D1; break;
            case '2': key = ConsoleKey.D2; break;
            case '3': key = ConsoleKey.D3; break;
            case '4': key = ConsoleKey.D4; break;
            case '5': key = ConsoleKey.D5; break;
            case '6': key = ConsoleKey.D6; break;
            case '7': key = ConsoleKey.D7; break;
            case '8': key = ConsoleKey.D8; break;
            case '9': key = ConsoleKey.D9; break;
            default:
                // Handle letters
                if (char.IsLetter(c)) {
                    char upperC = char.ToUpper(c);
                    if (char.IsUpper(c) && char.IsLower(c) == false) {
                        modifiers = ConsoleModifiers.Shift;
                    }

                    key = (ConsoleKey)Enum.Parse(typeof(ConsoleKey), upperC.ToString());
                }
                // Handle symbols that might require shift
                else if (char.IsSymbol(c) || char.IsPunctuation(c)) {
                    var ki = GetKeyForSymbol(c);
                    key = ki.Key;
                    if (ki.Shift) {
                        modifiers = ConsoleModifiers.Shift;
                    }
                }
                else {
                    // Default to the character's numeric value if no specific mapping exists
                    key = (ConsoleKey)c;
                }

                break;
        }

        return new ConsoleKeyInfo(c, key, modifiers.HasShift, modifiers.HasAlt, modifiers.HasCtrl);
    }

    /// <summary>
    /// Gets the ConsoleKey for common symbols and determines if Shift is required.
    /// </summary>
    /// <param name="symbol">The symbol character.</param>
    /// <returns>The corresponding ConsoleKey and whether shift was used.</returns>
    private static (ConsoleKey Key, bool Shift) GetKeyForSymbol(char symbol) {
        return symbol switch {
            '!' => (ConsoleKey.D1, true),
            '@' => (ConsoleKey.D2, true),
            '#' => (ConsoleKey.D3, true),
            '$' => (ConsoleKey.D4, true),
            '%' => (ConsoleKey.D5, true),
            '^' => (ConsoleKey.D6, true),
            '&' => (ConsoleKey.D7, true),
            '*' => (ConsoleKey.D8, true),
            '(' => (ConsoleKey.D9, true),
            ')' => (ConsoleKey.D0, true),
            '-' => (ConsoleKey.OemMinus, false),
            '_' => (ConsoleKey.OemMinus, true),
            '=' => (ConsoleKey.OemPlus, false),
            '+' => (ConsoleKey.OemPlus, true),
            ':' => (ConsoleKey.Oem1, true),
            ';' => (ConsoleKey.Oem1, false),
            '?' => (ConsoleKey.Oem2, true),
            '/' => (ConsoleKey.Oem2, false),
            '~' => (ConsoleKey.Oem3, true),
            '`' => (ConsoleKey.Oem3, false),
            '{' => (ConsoleKey.Oem4, true),
            '[' => (ConsoleKey.Oem4, false),
            '|' => (ConsoleKey.Oem5, true),
            '\\' => (ConsoleKey.Oem5, false),
            '}' => (ConsoleKey.Oem6, true),
            ']' => (ConsoleKey.Oem6, false),
            '"' => (ConsoleKey.Oem7, true),
            '\'' => (ConsoleKey.Oem7, false),
            ',' => (ConsoleKey.OemComma, false),
            '<' => (ConsoleKey.OemComma, true),
            '.' => (ConsoleKey.OemPeriod, false),
            '>' => (ConsoleKey.OemPeriod, true),
            _ => ((ConsoleKey)symbol, false)
        };
    }

    extension(Uri) {
        public static bool IsLocal(string path) {
            if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
                return uri.IsFile;
    
            // If it's not a valid absolute URI, check if it's a local path
            return Path.IsPathRooted(path) || !path.Contains("://");
        }
    }
}