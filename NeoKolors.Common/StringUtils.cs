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
    /// returns the total count of the printable / visible characters of a string
    /// (for example ansi escape characters are not counted)
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
    #if NET8_0_OR_GREATER
    [Pure]
    public static string Format([StringSyntax(StringSyntaxAttribute.CompositeFormat)] this string format, 
        params object[] args) =>
        string.Format(CultureInfo.InvariantCulture, format, args);
    #else
    [Pure]
        public static string Format(this string format, params object[] args) =>
            string.Format(CultureInfo.InvariantCulture, format, args);
    #endif
    
    
}