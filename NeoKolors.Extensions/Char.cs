// NeoKolors
// Copyright (c) 2025 KryKom

using System.Globalization;
using System.Text;

namespace NeoKolors.Extensions;

public static class CharExtensions {
    
    private static readonly HashSet<char> VOWELS_L = [
        'a', 'e', 'i', 'o', 'u', 'y',
        'á', 'à', 'â', 'ä', 'ã', 'å', 'æ', 'ą', 'ă',
        'é', 'è', 'ê', 'ë', 'ě', 'ę', 'ĕ',
        'í', 'ì', 'î', 'ï', 'ĭ',
        'ó', 'ò', 'ô', 'ö', 'õ', 'ø', 'ő',
        'ú', 'ù', 'û', 'ü', 'ů', 'ű',
        'ý', 'ÿ'
    ];

    private static readonly HashSet<char> VOWELS_U = VOWELS_L.Select(char.ToUpper).ToHashSet();

    private static readonly Dictionary<char, char> COMBINING_SPACING_MAP = new() {
        ['\u0300'] = '`', // grave accent
        ['\u0301'] = '´', // acute accent
        ['\u0302'] = '^', // circumflex
        ['\u0308'] = '¨', // diaeresis (umlaut)
        ['\u030C'] = 'ˇ', // caron
        ['\u0303'] = '~', // tilde
        ['\u0327'] = '¸', // cedilla
        ['\u030A'] = '˚', // ring above
    };

    private static readonly string[] UNPRINTABLE = [
        "NUL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL", "BS" , "HT", "LF" , "VT" , "FF", "CR", "SO", "SI",
        "DLE", "DC1", "DC2", "DC3", "DC4", "NAK", "SYN", "ETB", "CAN", "EN", "SUB", "ESC", "FS", "GS", "RS", "US",
        "SPC"
    ];

    extension(char) {
        
        /// <summary>
        /// Determines whether the specified character is a vowel, including both uppercase and lowercase characters.
        /// Supports accented and special vowel characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a vowel; otherwise, false.</returns>
        public static bool IsVowel(char c) => IsVowelLower(c) | IsVowelUpper(c);

        /// <summary>
        /// Determines whether the specified character is an uppercase vowel.
        /// Supports accented and special vowel characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is an uppercase vowel; otherwise, false.</returns>
        public static bool IsVowelUpper(char c) => VOWELS_U.Contains(c);

        /// <summary>
        /// Determines whether the specified character is a lowercase vowel.
        /// Supports accented and special vowel characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a lowercase vowel; otherwise, false.</returns>
        public static bool IsVowelLower(char c) => VOWELS_L.Contains(c);

        /// <summary>
        /// Determines whether the specified character is a consonant, including both uppercase and lowercase characters.
        /// Excludes vowels and non-alphabetic characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a consonant; otherwise, false.</returns>
        public static bool IsConsonant(char c) => char.IsLetter(c) && !IsVowel(c);

        /// <summary>
        /// Determines whether the specified character is an uppercase consonant.
        /// Excludes vowels and non-alphabetic characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is an uppercase consonant; otherwise, false.</returns>
        public static bool IsConsonantUpper(char c) => char.IsLetter(c) && char.IsUpper(c) && !IsVowel(c);

        /// <summary>
        /// Determines whether the specified character is a lowercase consonant.
        /// Excludes vowels and non-alphabetic characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a lowercase consonant; otherwise, false.</returns>
        public static bool IsConsonantLower(char c) => char.IsLetter(c) && char.IsLower(c) && !IsVowel(c);

        /// <summary>
        /// Separates a character into its base character and its diacritic mark, if present.
        /// The base character is the primary alphabetical or numeric character,
        /// while the diacritic mark represents additional accent or tonal information.
        /// </summary>
        /// <param name="input">The input character to be separated.</param>
        /// <param name="remapToSpacing">If true, remaps the combining diacritics characters to their corresponding
        /// spacing variant. NOTE: this may not work for all diacritic characters.</param>
        /// <returns>A tuple containing the base character and the diacritic mark.
        /// If no diacritic is present, the diacritic mark in the tuple will be '\0'.</returns>
        public static (char BaseChar, char? Diacritics) SeparateDiacritics(char input, bool remapToSpacing = true) {
            string s = (input + string.Empty).Normalize(NormalizationForm.FormD);
            char baseChar = '\0';
            char? diacritics = null;
        
            foreach (char c in s) {
                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark) {
                    // we currently only support one diacritic in the TUI, 
                    // so we take the last one (or first, but let's stick to simple logic for now)
                    diacritics = c;
                }
                else {
                    baseChar = c;
                }
            }

            // remap diacritics if requested
            if (diacritics is not null && remapToSpacing) 
                diacritics = RemapToSpacing(diacritics.Value);
            
            return (baseChar, diacritics);
        }

        
        /// <summary>
        /// Converts the specified character to its spacing equivalent, if applicable.
        /// This is typically used for converting diacritic or special characters to their spacing counterparts.
        /// </summary>
        /// <param name="c">The character to remap to its spacing equivalent.</param>
        /// <returns>The spacing equivalent of the character if one exists; otherwise, the original character.</returns>
        public static char RemapToSpacing(char c) => COMBINING_SPACING_MAP.GetValueOrDefault(c, c);

        /// <summary>
        /// Converts the specified character to its human-readable representation.
        /// For unprintable characters, a descriptive name is returned.
        /// For printable characters, the character itself is returned as a string.
        /// </summary>
        /// <param name="c">The character to convert to a displayable string.</param>
        /// <returns>
        /// A string representation of the character, or a descriptive name for unprintable characters.
        /// </returns>
        public static string ToDisplay(char c) => 
            (int)c == ..' ' 
                ? UNPRINTABLE[c] 
                : c == '\xff' 
                    ? "DEL" 
                    : c.ToString();
    }
}