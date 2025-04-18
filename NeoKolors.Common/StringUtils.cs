//
// NeoKolors
// Copyright (c) 2025 KryKom
//

#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace NeoKolors.Common;

/// <summary>
/// contains useful methods for working with strings
/// </summary>
public static class StringUtils {
    
    /// <summary>
    /// returns the total count of the printable / visible characters contained by the string
    /// (for example, ansi escape characters are not counted)
    /// </summary>
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
}