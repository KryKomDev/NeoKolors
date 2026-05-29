// NeoKolors
// Copyright (c) 2025 KryKom

using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;

namespace NeoKolors.Extensions;

public static class CharExtensions {

    // basic vowels, lower
    private static readonly ImmutableHashSet<char> VOWELS_LB = [
        'a', 'e', 'i', 'o', 'u', 'y'
    ];

    // extended vowels, lower
    private static readonly ImmutableHashSet<char> VOWELS_LE = [
        'á', 'à', 'â', 'ä', 'ã', 'å', 'æ', 'ą', 'ă',
        'é', 'è', 'ê', 'ë', 'ě', 'ę', 'ĕ',
        'í', 'ì', 'î', 'ï', 'ĭ',
        'ó', 'ò', 'ô', 'ö', 'õ', 'ø', 'ő',
        'ú', 'ù', 'û', 'ü', 'ů', 'ű',
        'ý', 'ÿ'
    ];

    // basic consonants, lower
    private static readonly ImmutableHashSet<char> CONSONANTS_LB = [
        'b', 'c', 'd', 'f', 'g',
        'h', 'j', 'k', 'l', 'm', 
        'n', 'p', 'q', 'r', 's',
        't', 'v', 'w', 'x', 'z'
    ];

    // extended consonants, lower
    private static readonly ImmutableHashSet<char> CONSONANTS_LE = [ 
        'č', 'ć', 'ď', 'đ', 'ł', 'ń', 'ň', 'ř', 'ś', 'š', 'ť', 'ź', 'ż', 'ž',
        'ç', 'ñ',
        'ð', 'þ',
        'ġ', 'ħ'
    ];

    private static readonly ImmutableHashSet<char> DIGITS = [
        '0', '1', '2', '3', '4', 
        '5', '6', '7', '8', '9'
    ];
    
    private static readonly ImmutableHashSet<char> VOWELS_UB     = ToUpperImmutableHashSet(VOWELS_LB);
    private static readonly ImmutableHashSet<char> VOWELS_UE     = ToUpperImmutableHashSet(VOWELS_LE);
    private static readonly ImmutableHashSet<char> CONSONANTS_UB = ToUpperImmutableHashSet(CONSONANTS_LB);
    private static readonly ImmutableHashSet<char> CONSONANTS_UE = ToUpperImmutableHashSet(CONSONANTS_LE);
    
    private static ImmutableHashSet<char> ToUpperImmutableHashSet(ImmutableHashSet<char> immutableHashSet) =>
        immutableHashSet.Select(char.ToUpper).ToImmutableHashSet();
    
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
    
    private static readonly Dictionary<char, char> SPACING_COMBINING_MAP = 
        COMBINING_SPACING_MAP.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    private static readonly string[] UNPRINTABLE = [
        "NUL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL", "BS" , "HT", "LF" , "VT" , "FF", "CR", "SO", "SI",
        "DLE", "DC1", "DC2", "DC3", "DC4", "NAK", "SYN", "ETB", "CAN", "EN", "SUB", "ESC", "FS", "GS", "RS", "US",
        "SPC"
    ];

    private static readonly HashSet<char> ESCAPED =
        ['\a', '\b', '\e', '\f', '\n', '\r', '\t', '\v', '\\', '\'', '\"', '\0'];
    
    extension(char) {

        public static ImmutableHashSet<char> BasicVowelsLower        => VOWELS_LB;
        public static ImmutableHashSet<char> ExtendedVowelsLower     => VOWELS_LE;
        public static ImmutableHashSet<char> BasicConsonantsLower    => CONSONANTS_LB;
        public static ImmutableHashSet<char> ExtendedConsonantsLower => CONSONANTS_LE;
        public static ImmutableHashSet<char> BasicVowelsUpper        => VOWELS_UB;
        public static ImmutableHashSet<char> ExtendedVowelsUpper     => VOWELS_UE;
        public static ImmutableHashSet<char> BasicConsonantsUpper    => CONSONANTS_UB;
        public static ImmutableHashSet<char> ExtendedConsonantsUpper => CONSONANTS_UE;
        public static ImmutableHashSet<char> Digits                  => DIGITS;

        /// <summary>
        /// Determines whether the specified character is a vowel, including both uppercase and lowercase characters.
        /// Supports accented and special vowel characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a vowel; otherwise, false.</returns>
        public static bool IsVowel(char c) => 
            VOWELS_LB.Contains(char.ToLower(c)) || VOWELS_LE.Contains(char.ToLower(c));

        /// <summary>
        /// Determines whether the specified character is an uppercase vowel.
        /// Supports accented and special vowel characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is an uppercase vowel; otherwise, false.</returns>
        public static bool IsVowelUpper(char c) =>
            char.IsUpper(c) && char.IsVowel(c);

        /// <summary>
        /// Determines whether the specified character is a lowercase vowel.
        /// Supports accented and special vowel characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a lowercase vowel; otherwise, false.</returns>
        public static bool IsVowelLower(char c) => 
            char.IsLower(c) && char.IsVowel(c);

        /// <summary>
        /// Determines whether the specified character is a basic vowel (a, e, i, o, u),
        /// including both uppercase and lowercase characters.
        /// Does not include accented or special vowel characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a basic vowel; otherwise, false.</returns>
        public static bool IsBasicVowel(char c) => VOWELS_LB.Contains(char.ToLower(c));

        /// <summary>
        /// Determines whether the specified character is an uppercase basic vowel (A, E, I, O, U).
        /// Does not include accented or special vowel characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is an uppercase basic vowel; otherwise, false.</returns>
        public static bool IsBasicVowelUpper(char c) => char.IsUpper(c) && char.IsBasicVowel(c);

        /// <summary>
        /// Determines whether the specified character is a basic vowel (a, e, i, o, u)
        /// in lowercase form. Does not include uppercase characters, accented vowels, or special vowel characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a basic lowercase vowel; otherwise, false.</returns>
        public static bool IsBasicVowelLower(char c) => char.IsLower(c) && char.IsBasicVowel(c);

        /// <summary>
        /// Determines whether the specified character is an extended vowel, including accented and special vowel
        /// characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is an extended vowel; otherwise, false.</returns>
        public static bool IsExtendedVowel(char c) => VOWELS_LE.Contains(char.ToLower(c));

        /// <summary>
        /// Determines whether the specified character is an extended vowel in uppercase form, including accented and
        /// special vowel
        /// characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is an extended vowel in uppercase form; otherwise, false.</returns>
        public static bool IsExtendedVowelUpper(char c) => char.IsUpper(c) && char.IsExtendedVowel(c);

        /// <summary>
        /// Determines whether the specified character is a lowercase extended vowel,
        /// including accented and special vowel characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a lowercase extended vowel; otherwise, false.</returns>
        public static bool IsExtendedVowelLower(char c) => char.IsLower(c) && char.IsExtendedVowel(c);

        /// <summary>
        /// Determines whether the specified character is a consonant, including both uppercase and lowercase characters.
        /// Excludes vowels and non-alphabetic characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a consonant; otherwise, false.</returns>
        public static bool IsConsonant(char c) => 
            CONSONANTS_LB.Contains(char.ToLower(c)) || CONSONANTS_LE.Contains(char.ToLower(c));

        /// <summary>
        /// Determines whether the specified character is an uppercase consonant.
        /// Excludes vowels and non-alphabetic characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is an uppercase consonant; otherwise, false.</returns>
        public static bool IsConsonantUpper(char c) => char.IsUpper(c) && char.IsConsonant(c);

        /// <summary>
        /// Determines whether the specified character is a lowercase consonant.
        /// Excludes vowels and non-alphabetic characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a lowercase consonant; otherwise, false.</returns>
        public static bool IsConsonantLower(char c) => char.IsLower(c) && char.IsConsonant(c);

        /// <summary>
        /// Determines whether the specified character is a basic consonant, considering only lowercase and uppercase
        /// English alphabet characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a basic consonant; otherwise, false.</returns>
        public static bool IsBasicConsonant(char c) => CONSONANTS_LB.Contains(char.ToLower(c));

        /// <summary>
        /// Determines whether the specified character is a basic consonant and is an uppercase letter.
        /// Only evaluates basic consonants from the English alphabet.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a basic consonant and uppercase; otherwise, false.</returns>
        public static bool IsBasicConsonantUpper(char c) => char.IsUpper(c) && char.IsBasicConsonant(c);

        /// <summary>
        /// Determines whether the specified character is a basic consonant in lowercase,
        /// considering only English alphabet characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a basic consonant and in lowercase; otherwise, false.</returns>
        public static bool IsBasicConsonantLower(char c) => char.IsLower(c) && char.IsBasicConsonant(c);

        /// <summary>
        /// Determines whether the specified character is an extended consonant, including lowercase accented and
        /// special consonant characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is an extended consonant; otherwise, false.</returns>
        public static bool IsExtendedConsonant(char c) => CONSONANTS_LE.Contains(char.ToLower(c));

        /// <summary>
        /// Determines whether the specified character is an extended consonant and is in uppercase.
        /// Extended consonants include accented and special consonant characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is an extended consonant and is uppercase; otherwise, false.</returns>
        public static bool IsExtendedConsonantUpper(char c) => char.IsUpper(c) && char.IsExtendedConsonant(c);

        /// <summary>
        /// Determines whether the specified character is an extended consonant in lowercase,
        /// including accented and special consonant characters.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a lowercase extended consonant; otherwise, false.</returns>
        public static bool IsExtendedConsonantLower(char c) => char.IsLower(c) && char.IsExtendedConsonant(c);

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
                    diacritics = c;
                }
                else {
                    baseChar = c;
                }
            }

            // remap diacritics if requested
            if (diacritics is not null && remapToSpacing) 
                diacritics = char.ToSpacing(diacritics.Value);
            
            return (baseChar, diacritics);
        }

        
        /// <summary>
        /// Converts the specified character to its spacing equivalent, if applicable.
        /// This is typically used for converting diacritic or special characters to their spacing counterparts.
        /// </summary>
        /// <param name="c">The character to remap to its spacing equivalent.</param>
        /// <returns>The spacing equivalent of the character if one exists; otherwise, the original character.</returns>
        public static char ToSpacing(char c) => COMBINING_SPACING_MAP.GetValueOrDefault(c, c);

        /// <summary>
        /// Converts a given spacing character to its corresponding combining character, if a mapping exists.
        /// If no mapping exists, the input character is returned as is.
        /// </summary>
        /// <param name="c">The spacing character to be converted to a combining character.</param>
        /// <returns>The corresponding combining character if a mapping exists; otherwise, the original character.
        /// </returns>
        public static char ToCombining(char c) => SPACING_COMBINING_MAP.GetValueOrDefault(c, c);

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
            c <= ' ' 
                ? UNPRINTABLE[c] 
                : c == '\xff' 
                    ? "DEL" 
                    : c.ToString();

        /// <summary>
        /// Parses an escaped character sequence, such as escape sequences or hexadecimal representations,
        /// into its corresponding character.
        /// </summary>
        /// <param name="s">The string representing the escape sequence or character literal.
        /// Must not be null or empty.</param>
        /// <returns>The character represented by the escape sequence or literal.</returns>
        /// <exception cref="ArgumentException">Thrown when the input string is null or empty.</exception>
        /// <exception cref="FormatException">Thrown when the input string contains an invalid escape sequence
        /// or the hexadecimal value is out of the range of valid Unicode characters.</exception>
        [Pure]
        public static char ParseEsc(string s) => 
            char.TryParseEsc(s, out var c) ? c : throw new FormatException();

        /// <summary>
        /// Attempts to parse a string representing an escaped character and outputs the resulting character if
        /// parsing is successful.
        /// </summary>
        /// <param name="s">The input string to parse. The string can represent a single character or an escaped
        /// character sequence.</param>
        /// <param name="c">When this method returns, contains the parsed character if the parsing succeeds; otherwise,
        /// the default value of <see cref="char"/>.</param>
        /// <returns>True if the string was successfully parsed into a character; otherwise, false.</returns>
        [Pure]
        public static bool TryParseEsc(string s, out char c) {
            s = s.Normalize(NormalizationForm.FormC);
            
            if (string.IsNullOrEmpty(s)) {
                c = '\0';
                return false;
            }

            // Normal single character
            if (s.Length == 1) {
                c = s[0];
                return true;
            }

            if (s[0] != '\\') {
                c = '\0';
                return false;
            }

            if (s.Length == 2) {
                c = s[1] switch {
                    'a'  => '\a',
                    'b'  => '\b',
                    'e'  => '\e',
                    'f'  => '\f',
                    'n'  => '\n',
                    'r'  => '\r',
                    't'  => '\t',
                    'v'  => '\v',
                    '\\' => '\\',
                    '\'' => '\'',
                    '\"' => '\"',
                    '0'  => '\0',
                    _    => '\0'
                };

                return c != '\0' || s[1] == '0';
            }

            if (s[1] is not ('x' or 'u')) {
                c = '\0';
                return false;
            }

            int       expectedMinLength = s[1] == 'x' ? 3 : 6;
            const int expectedMaxLength = 6;

            if (s.Length < expectedMinLength || s.Length > expectedMaxLength) {
                c = '\0';
                return false;
            }

            string hex = s[1] == 'x' ? s[2..] : s[2..6];

            if (!int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int value) || 
                value is < char.MinValue or > char.MaxValue) 
            {
                c = '\0';
                return false;
            }

            c = (char)value;
            return true;
        }

        /// <summary>
        /// Converts the specified character to its escaped string representation based on standard escape sequences.
        /// Handles special characters such as newline, tab, and others.
        /// </summary>
        /// <param name="c">The character to convert to an escaped string representation.</param>
        /// <returns>A string containing the escaped representation of the character if it matches a supported
        /// escape sequence.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the character cannot be represented using
        /// an escape sequence.</exception>
        public static string ToStringEsc(char c) {
            if (ESCAPED.Contains(c)) {
                return c switch {
                    '\a' => "\\a",
                    '\b' => "\\b",
                    '\e' => "\\e",
                    '\f' => "\\f",
                    '\n' => "\\n",
                    '\r' => "\\r",
                    '\t' => "\\t",
                    '\v' => "\\v",
                    '\\' => "\\\\",
                    '\'' => "\\'",
                    '\"' => "\\\"",
                    '\0' => "\\0",
                    _    => throw new ArgumentOutOfRangeException(nameof(c), c, null)
                };
            }

            return c.ToString();
        }
    }
}