//
// NeoKolors
// Copyright (c) 2025 KryKom
//

#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Collections;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

#pragma warning disable CS0618 // Type or member is obsolete

namespace NeoKolors.Common.Util;

/// <summary>
/// contains useful methods for working with strings
/// </summary>
public static class StringUtils {
    
    /// <summary>
    /// returns the total count of the printable / visible characters contained by the string
    /// (for example, ansi escape characters are not counted)
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int VisibleLength(this string text) =>
        Regex.Replace(text, @"\e([^m]*)m", "").Length;

    
    /// <summary>
    /// capitalizes all the words within the string
    /// </summary>
    /// <exception cref="ArgumentNullException">the string is null</exception>
    [Pure]
    public static string Capitalize(this string s) {
        if (s is null) throw new ArgumentNullException(nameof(s));
        if (s.Length == 0) return "";
        if (s.Length == 1) return s.ToUpper();

        string[] words = s.Split(' '); 

        for (int i = 0; i < words.Length; i++) {
            if (string.IsNullOrEmpty(words[i])) continue;
            
            char[] letters = words[i].ToCharArray();

            if (letters.Length <= 0) continue;
            
            letters[0] = char.ToUpper(letters[0]);
            words[i] = new string(letters);
        }

        return string.Join(" ", words);
    }
    
    
    /// <summary>
    /// makes the first letter of the string capital
    /// </summary>
    /// <exception cref="ArgumentNullException">the input string is null</exception>
    [Pure]
    public static string CapitalizeFirst(this string input, CultureInfo? cultureInfo = null) =>
        input switch {
            null => throw new ArgumentNullException(nameof(input)),
            "" => "",
            _ => string.Concat(
                input[0].ToString().ToUpper(cultureInfo ?? CultureInfo.InvariantCulture), input.Substring(1))
        };
    
    
    /// <summary>
    /// makes the first letter of the string lowercase
    /// </summary>
    /// <exception cref="ArgumentNullException">the input string is null</exception>
    [Pure]
    public static string DecapitalizeFirst(this string input, CultureInfo? cultureInfo = null) =>
        input switch {
            null => throw new ArgumentNullException(nameof(input)),
            "" => "",
            _ => string.Concat(
                input[0].ToString().ToLower(cultureInfo ?? CultureInfo.InvariantCulture), input.Substring(1))
        };

    
    /// <summary>
    /// formats the string using the <see cref="string.Format(System.IFormatProvider?,string,object?)"/>
    /// </summary>
    /// <param name="format">the string to format</param>
    /// <param name="args">the arguments for the string</param>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format(
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] this string format, 
        params object[] args) =>
        string.Format(CultureInfo.InvariantCulture, format, args);

    
    /// <summary>
    /// creates a substring between the indices, including startIndex, excluding endIndex
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// startIndex or endIndex is out of range, startIndex is greater than endIndex
    /// </exception>
    #if NET5_0_OR_GREATER
    [Obsolete("This method can be substituted with range indexer.")]
    #endif
    public static string InRange(this string s, int startIndex, int endIndex) {
        if (startIndex < 0 || startIndex >= s.Length) 
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index is out of range.");
        if (endIndex < 0 || endIndex >= s.Length) 
            throw new ArgumentOutOfRangeException(nameof(endIndex), "End index is out of range.");
        if (startIndex > endIndex) 
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index is larger than the end index");
        return s.Substring(startIndex, endIndex - startIndex);
    }

    
    /// <summary>
    /// Extracts a range of elements from the given enumerable collection starting from the specified
    /// start index to the specified end index, inclusive of the start index but exclusive of the end index.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the enumerable collection.</typeparam>
    /// <param name="enumerable">The enumerable collection from which to extract the range of elements.</param>
    /// <param name="startIndex">The zero-based starting index of the range to extract.</param>
    /// <param name="endIndex">The zero-based ending index of the range to extract (exclusive).</param>
    /// <returns>An enumerable containing the elements from the specified range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="startIndex"/> or <paramref name="endIndex"/> is out of the array's bounds,
    /// or if <paramref name="startIndex"/> is greater than <paramref name="endIndex"/>.
    /// </exception>
    #if NET5_0_OR_GREATER
    [Obsolete("This method can be substituted with range indexer.")]
    #endif
    public static IEnumerable<T> InRange<T>(this IEnumerable<T> enumerable, int startIndex, int endIndex) {
        var array = enumerable as T[] ?? enumerable.ToArray();
        if (startIndex < 0 || startIndex >= array.Length)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index is out of range.");
        if (endIndex < 0 || endIndex >= array.Length)  
            throw new ArgumentOutOfRangeException(nameof(endIndex), "End index is out of range.");
        if (startIndex > endIndex) 
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index is larger than the end index");

        for (int i = startIndex; i < endIndex; i++) {
            yield return array[i];
        }
    }

    
    /// <summary>
    /// chops the string into multiple string with a maximum length
    /// </summary>
    /// <param name="s">the string to be chopped</param>
    /// <param name="maxLength">the maximum length of a single string</param>
    [Pure]
    public static string[] Chop(this string s, int maxLength) {
        if (string.IsNullOrWhiteSpace(s)) return [];
        ArgOutOfRange.ThrowIf(maxLength <= 0, nameof(maxLength), "Maximum length must be positive");

        List<string> output = [];
        int currentPosition = 0;

        while (currentPosition < s.Length) {
            int remainingLength = s.Length - currentPosition;
            int chunkSize = Math.Min(maxLength, remainingLength);

            int nextNewline = s.IndexOfAny(['\n', '\r'], currentPosition, chunkSize);
        
            if (nextNewline != -1) {
                chunkSize = nextNewline - currentPosition;
            }
            else if (chunkSize < remainingLength && !char.IsWhiteSpace(s[currentPosition + chunkSize])) {
                int lastSpace = s.LastIndexOf(' ', currentPosition + chunkSize - 1, chunkSize);
            
                if (lastSpace > currentPosition) {
                    chunkSize = lastSpace - currentPosition;
                }
            }

            string chunk = s.Substring(currentPosition, chunkSize).Trim();
            if (!string.IsNullOrEmpty(chunk)) {
                output.Add(chunk);
            }

            currentPosition += chunkSize;
            while (currentPosition < s.Length && 
                   (char.IsWhiteSpace(s[currentPosition]) || s[currentPosition] is '\n' or '\r')) 
            {
                if (currentPosition + 1 < s.Length && s[currentPosition] == '\r' && s[currentPosition + 1] == '\n') {
                    currentPosition++;
                }
                
                currentPosition++;
            }
        }


        return output.ToArray();
    }

    
    /// <summary>
    /// joins the stringified objects from the collection using the separator
    /// </summary>
    public static string Join(this IEnumerable collection, string separator) {
        List<string?> strings = [];
        foreach (var c in collection) {
            if (c is null) {
                strings.Add("");
                continue;
            }
            
            string? s = c.ToString();
            strings.Add(s ?? "");
        }
        
        return string.Join(separator, strings);
    }

    
    /// <summary>
    /// Concatenates the elements of a specified collection using the specified separator.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
    /// <param name="collection">The collection whose elements will be concatenated.</param>
    /// <param name="separator">The string separator to insert between each element of the collection.</param>
    /// <param name="stringifier">A function to convert each element of the collection to a string.</param>
    /// <returns>A string consisting of the elements of the collection concatenated using the specified separator.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the collection or stringifier is null.</exception>
    public static string Join<TSource>(
        this IEnumerable<TSource> collection,
        string separator,
        Func<TSource?, string?> stringifier) 
    {
        List<string?> strings = [];
        foreach (var c in collection) {
            string? s = stringifier(c);
            strings.Add(s ?? "");
        }
        
        return string.Join(separator, strings);
    }

    
    /// <summary>
    /// Converts an integer to its Roman numeral representation.
    /// Supports values from 1 to 3999.
    /// </summary>
    /// <param name="number">The integer to be converted to Roman numerals.</param>
    /// <param name="lowercase">Specifies whether to return the result in the lowercase. Default is false.</param>
    /// <returns>A string representing the Roman numeral equivalent of the given number.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the supplied number is lower than 1 or greater than 3999.
    /// </exception>
    [Pure]
    public static string ToRoman(this int number, bool lowercase = false) {
        ArgOutOfRange.ThrowIf(number is < 1 or > 3999, nameof(number));

        string output = "";

        int units = number % 10;
        int tens = number / 10 % 10;
        int hundreds = number / 100 % 10;
        int thousands = number / 1000 % 10;

        switch (thousands) {
            case 0: break;
            case 1: output += "M"; break;
            case 2: output += "MM"; break;
            case 3: output += "MMM"; break;
        }
        
        switch (hundreds) {
            case 0: break;
            case 1: output += "C"; break;
            case 2: output += "CC"; break;
            case 3: output += "CCC"; break;
            case 4: output += "CD"; break;
            case 5: output += "D"; break;
            case 6: output += "DC"; break;
            case 7: output += "DCC"; break;
            case 8: output += "DCCC"; break;
            case 9: output += "CM"; break;
        }
        
        switch (tens) {
            case 0: break;
            case 1: output += "X"; break;
            case 2: output += "XX"; break;
            case 3: output += "XXX"; break;
            case 4: output += "XL"; break;
            case 5: output += "L"; break;
            case 6: output += "LX"; break;
            case 7: output += "LXX"; break;
            case 8: output += "LXXX"; break;
            case 9: output += "XC"; break;
        }
        
        switch (units) {
            case 0: break;
            case 1: output += "I"; break;
            case 2: output += "II"; break;
            case 3: output += "III"; break;
            case 4: output += "IV"; break;
            case 5: output += "V"; break;
            case 6: output += "VI"; break;
            case 7: output += "VII"; break;
            case 8: output += "VIII"; break;
            case 9: output += "IX"; break;
        }
        
        return lowercase ? output.ToLower() : output;
    }

    /// <summary>
    /// Error message indicating that the specified padding length must be non-negative
    /// and greater than or equal to the length of the string.
    /// </summary>
    private const string PAD_INV_MSG = "Padding length must be non-negative and greater or equal to the string length.";
    
    /// <summary>
    /// Pads the specified string on the right with spaces until the total
    /// visible length of the string reaches the given padding length.
    /// ANSI escape characters or other non-printable elements are not considered in the length.
    /// </summary>
    /// <param name="s">The string to pad on the right.</param>
    /// <param name="pad">The total visible length to achieve after padding.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the padding length is less than the string length.
    /// </exception>
    /// <returns>The input string padded with spaces on the right to meet the specified visible length.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string VisiblePadRight(this string s, int pad) => 
        pad >= s.VisibleLength() 
            ? s + new string(' ', pad - s.VisibleLength()) 
            : throw new ArgumentOutOfRangeException(nameof(pad), PAD_INV_MSG);

    
    /// <summary>
    /// Pads the specified string on the left with spaces until the total
    /// visible length of the string reaches the given padding length.
    /// ANSI escape characters or other non-printable elements are not considered in the length.
    /// </summary>
    /// <param name="s">The string to pad on the left.</param>
    /// <param name="pad">The total visible length to achieve after padding.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the padding length is less than the string length.
    /// </exception>
    /// <returns>The input string padded with spaces on the left to meet the specified visible length.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string VisiblePadLeft(this string s, int pad) => 
        pad >= s.VisibleLength() 
            ? new string(' ', pad - s.VisibleLength()) + s
            : throw new ArgumentOutOfRangeException(nameof(pad), PAD_INV_MSG);

    
    /// <summary>
    /// Pads the specified string in the center with spaces until the total
    /// visible length of the string reaches the given padding length.
    /// ANSI escape characters or other non-printable elements are not considered in the length.
    /// </summary>
    /// <param name="s">The string to pad in the center.</param>
    /// <param name="pad">The total visible length to achieve after padding.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the padding length is less than the string length.
    /// </exception>
    /// <returns>The input string padded with spaces in the center to meet the specified visible length.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string VisiblePadCenter(this string s, int pad) =>
        pad >= s.VisibleLength() 
            ? new string(' ', (int)(Math.Floor((pad - s.VisibleLength()) / 2f))) + s +
              new string(' ', (int)(Math.Ceiling((pad - s.VisibleLength()) / 2f)))
            : throw new ArgumentOutOfRangeException(nameof(pad), PAD_INV_MSG);
    
    
    /// <summary>
    /// Pads the specified string in the center with spaces until the total
    /// length of the string reaches the given padding length.
    /// </summary>
    /// <param name="s">The string to pad in the center.</param>
    /// <param name="pad">The total visible length to achieve after padding.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the padding length is less than the string length.
    /// </exception>
    /// <returns>The input string padded with spaces in the center to meet the specified visible length.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string PadCenter(this string s, int pad) =>
        pad >= s.Length 
            ? new string(' ', (int)(Math.Floor((pad - s.Length) / 2f))) + s +
              new string(' ', (int)(Math.Ceiling((pad - s.Length) / 2f)))
            : throw new ArgumentOutOfRangeException(nameof(pad), PAD_INV_MSG);

    /// <summary>
    /// Returns a substring from the specified string, ensuring safe boundary handling
    /// by adjusting the start and count parameters to fit within the valid range of the string.
    /// </summary>
    /// <param name="s">The input string from which the substring is extracted.</param>
    /// <param name="start">The zero-based starting character position of the substring.</param>
    /// <param name="count">The number of characters to retrieve from the starting position.</param>
    /// <returns>A substring based on the specified start and count parameters, adjusted for safe boundary conditions.</returns>
    [Pure]
    public static string SafeSubstring(this string s, int start, int count) {
        start = start > s.Length - 1 ? s.Length - 1 : start < 0 ? 0 : start;
        count = count < 0 ? 0 : count + start >= s.Length ? s.Length - start : count;
        return s.Substring(start, count);
    }

    /// <summary>
    /// Determines whether the specified string contains any special characters.
    /// Special characters are defined as characters that are neither letters nor digits.
    /// </summary>
    /// <param name="s">The string to evaluate for the presence of special characters.</param>
    /// <returns>
    /// <c>true</c> if the string contains special characters; otherwise, <c>false</c>.
    /// </returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsSpecial(this string s) =>
        !string.IsNullOrEmpty(s) && s.Any(c => !(char.IsLetter(c) || char.IsDigit(c)));
    
    /// <summary>
    /// Determines whether the specified string contains any numeric characters.
    /// </summary>
    /// <param name="s">The string to check for numeric characters.</param>
    /// <returns>True if the string contains at least one numeric character; otherwise, false.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsNumber(this string s) =>
        !string.IsNullOrEmpty(s) && s.Any(char.IsDigit);

    /// <summary>
    /// Determines if the given string contains at least one letter.
    /// </summary>
    /// <param name="s">The string to check for the presence of letters.</param>
    /// <returns>True if the string contains at least one letter; otherwise, false.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsLetter(this string s) =>
        !string.IsNullOrEmpty(s) && s.Any(char.IsLetter);

    /// <summary>
    /// Determines whether the provided string contains at least one uppercase character.
    /// </summary>
    /// <param name="s">The string to evaluate.</param>
    /// <returns>True if the string contains one or more uppercase characters; otherwise, false.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsUpper(this string s) =>
        !string.IsNullOrEmpty(s) && s.Any(char.IsUpper);

    /// <summary>
    /// Determines whether the specified string contains any lowercase characters.
    /// </summary>
    /// <param name="s">The string to check for lowercase characters.</param>
    /// <returns>
    /// <c>true</c> if the specified string contains at least one lowercase character; otherwise, <c>false</c>.
    /// </returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsLower(this string s) =>
        !string.IsNullOrEmpty(s) && s.Any(char.IsLower);

    /// <summary>
    /// Determines whether the specified string is a valid identifier.
    /// A valid identifier is non-empty, contains only letters, digits, underscores, or '@',
    /// and does not start with a digit.
    /// </summary>
    /// <param name="s">The string to validate as an identifier.</param>
    /// <returns>True if the string is a valid identifier; otherwise, false.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidIdentifier(this string s) =>
        !string.IsNullOrEmpty(s) && s.All(c => char.IsLetterOrDigit(c) || c is '_' or '@') && !char.IsDigit(s[0]);

    /// <summary>
    /// Determines whether the given string consists solely of alphabetic characters.
    /// </summary>
    /// <param name="s">The string to evaluate.</param>
    /// <returns>True if the string is not null, not empty, and contains only letters; otherwise, false.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWord(this string s) =>
        !string.IsNullOrEmpty(s) && s.All(char.IsLetter);

    /// <summary>
    /// Determines whether a string is non-empty and begins with an uppercase character,
    /// followed by only lowercase characters.
    /// </summary>
    /// <param name="s">The string to evaluate.</param>
    /// <returns>True if the string is a capital word, otherwise false.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCapitalWord(this string s) =>
        !s.IsWord() && s.FirstAndAll(char.IsUpper, char.IsLower);

    /// <summary>
    /// Pads each line of the given string to the left by the specified number of spaces.
    /// Optionally skips padding the first line.
    /// </summary>
    /// <param name="s">The input string containing lines to pad.</param>
    /// <param name="pad">The number of spaces to pad to the left of each line.</param>
    /// <param name="padFirst">A boolean indicating whether to pad the first line or not.</param>
    /// <returns>The input string with each line padded to the left as specified.</returns>
    [Pure]
    public static string PadLinesLeft(this string s, int pad, bool padFirst = false) {
        string[] lines = s.Split('\n');
        for (int i = padFirst ? 0 : 1; i < lines.Length; i++)
            lines[i] = new string(' ', pad) + lines[i];
        return lines.Join("\n");
    }
}