//
// NeoKolors
// Copyright (c) 2025 KryKom
//

#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Collections;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

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
        if (maxLength <= 0) throw new ArgumentException("Maximum length must be positive", nameof(maxLength));

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
        Func<TSource?, string?> stringifier) {
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
    /// <param name="lowercase">Specifies whether to return the result in lowercase. Default is false.</param>
    /// <returns>A string representing the Roman numeral equivalent of the given number.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the supplied number is less than 1 or greater than 3999.
    /// </exception>
    [Pure]
    public static string ToRoman(this int number, bool lowercase = false) {
        if (number is < 1 or > 3999) throw new ArgumentOutOfRangeException(nameof(number));

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
}