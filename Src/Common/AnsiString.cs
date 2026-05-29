// NeoKolors
// Copyright (c) 2025 KryKom

using System.Collections;
using System.Text;

namespace NeoKolors.Common;

/// <summary>
/// Represents a string with ANSI style attributes, allowing for styling of individual
/// characters or ranges of characters. This class is immutable and supports various
/// operations for applying and combining styles.
/// </summary>
public sealed class AnsiString :
    IEnumerable<AnsiChar>,
    IEquatable<AnsiString>,
    ICloneable 
{

    // ============================ Fields and Props ============================ // 
    
    #region Fields and Properties

    private readonly string            _text;
    private readonly List<StyleMarker> _styles;

    /// <summary>
    /// Gets the plain text value without ANSI escape sequences.
    /// </summary>
    public string Plain => _text;

    /// <summary>
    /// Gets the number of characters in the string.
    /// </summary>
    public int Length => _text.Length;

    /// <summary>
    /// Gets the <see cref="AnsiChar"/> at the specified index, containing both the character and its style.
    /// </summary>
    /// <param name="index">The zero-based index of the character to get.</param>
    /// <returns>The <see cref="AnsiChar"/> at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when index is outside the bounds of the string.</exception>
    public AnsiChar this[int index] {
        get {
            if (index < 0 || index >= _text.Length)
                throw new IndexOutOfRangeException();
            
            return new AnsiChar(_text[index], GetStyleAt(index));
        }
    }

    #endregion

    
    // ============================ Constructors ============================ // 
    
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiString"/> class that is empty.
    /// </summary>
    public AnsiString() {
        _text = string.Empty;
        _styles = [];
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiString"/> class with a plain string and no styles.
    /// </summary>
    /// <param name="text">The plain string value.</param>
    public AnsiString(string text) {
        _text  = text;
        _styles = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiString"/> class from a collection of <see cref="AnsiChar"/>.
    /// </summary>
    /// <param name="chars">The collection of styled characters.</param>
    public AnsiString(IEnumerable<AnsiChar> chars) {
        var sb = new StringBuilder();
        _styles = [];
        NKStyle? lastStyle = null;
        int index = 0;

        foreach (var c in chars) {
            sb.Append(c.Char);
            
            if (c.Style != lastStyle) {
                _styles.Add(new StyleMarker(index, c.Style));
                lastStyle = c.Style;
            }
            
            index++;
        }

        _text = sb.ToString();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiString"/> class with a plain string and a uniform style.
    /// </summary>
    /// <param name="text">The plain string value.</param>
    /// <param name="style">The style to apply to the entire string.</param>
    public AnsiString(string text, NKStyle style) {
        _text   = text;
        _styles = [new StyleMarker(0, style)];
    }

    private AnsiString(string text, List<StyleMarker> styles) {
        _text   = text;
        _styles = styles;
    }

    #endregion
    
    #region Styling

    /// <summary>
    /// Retrieves the style applied to the character at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the character.</param>
    /// <returns>The <see cref="NKStyle"/> at the given index, or <see cref="NKStyle.Default"/> if no style is found.</returns>
    public NKStyle GetStyleAt(int index) {
        if (_styles.Count == 0) return NKStyle.Default;
        
        var marker = new StyleMarker(index, NKStyle.Default);
        int pos = _styles.BinarySearch(marker, StyleMarkerComparer.Instance);
        
        if (pos >= 0) return _styles[pos].Style;
        
        pos = ~pos;
        return pos == 0 
            ? NKStyle.Default 
            : _styles[pos - 1].Style;
    }

    /// <summary>
    /// Returns a new <see cref="AnsiString"/> with the style of the entire string overwritten by the specified style.
    /// </summary>
    /// <param name="style">The new style to apply.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString ApplyStyle(NKStyle style) => new(_text, style);

    /// <summary>
    /// Returns a new <see cref="AnsiString"/> where the style of a range starting from <paramref name="startIndex"/> 
    /// to the end is overwritten by the specified style.
    /// </summary>
    /// <param name="style">The new style to apply.</param>
    /// <param name="startIndex">The zero-based starting index of the range.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString ApplyStyle(NKStyle style, int startIndex) => 
        ApplyStyle(style, startIndex, _text.Length - startIndex);

    /// <summary>
    /// Returns a new <see cref="AnsiString"/> where the style of a specific range is overwritten by the specified style.
    /// </summary>
    /// <param name="style">The new style to apply.</param>
    /// <param name="startIndex">The zero-based starting index of the range.</param>
    /// <param name="length">The number of characters in the range.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when range is outside the bounds of the string.</exception>
    public AnsiString ApplyStyle(NKStyle style, int startIndex, int length) {
        if (startIndex < 0 || startIndex >= _text.Length)     throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (length < 0 || startIndex + length > _text.Length) throw new ArgumentOutOfRangeException(nameof(length));
        
        if (length == 0) 
            return this;

        var chars = this.ToList();
        
        for (int i = startIndex; i < startIndex + length; i++) {
            // Overwrite with the new style
            chars[i] = new AnsiChar(chars[i].Char, style);
        }
        
        return new AnsiString(chars);
    }

    /// <summary>
    /// Returns a new <see cref="AnsiString"/> where the style of the specified range is overwritten by the specified style.
    /// </summary>
    /// <param name="style">The new style to apply.</param>
    /// <param name="range">The range of indices to apply the style to.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString ApplyStyle(NKStyle style, Range range) {
        var (offset, length) = range.GetOffsetAndLength(_text.Length);
        return ApplyStyle(style, offset, length);
    }

    /// <summary>
    /// Returns a new <see cref="AnsiString"/> where the specified style is composed (layered) with existing styles 
    /// for the entire string. Non-default properties of <paramref name="style"/> will override existing ones.
    /// </summary>
    /// <param name="style">The style attributes to add.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString AddStyle(NKStyle style) => AddStyle(style, 0, _text.Length);

    /// <summary>
    /// Returns a new <see cref="AnsiString"/> where the specified style is composed (layered) with existing styles 
    /// from <paramref name="startIndex"/> to the end of the string.
    /// </summary>
    /// <param name="style">The style attributes to add.</param>
    /// <param name="startIndex">The zero-based starting index of the range.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString AddStyle(NKStyle style, int startIndex) => 
        AddStyle(style, startIndex, _text.Length - startIndex);

    /// <summary>
    /// Returns a new <see cref="AnsiString"/> where the specified style is composed (layered) with existing styles 
    /// in the specified range.
    /// </summary>
    /// <param name="style">The style attributes to add.</param>
    /// <param name="startIndex">The zero-based starting index of the range.</param>
    /// <param name="length">The number of characters in the range.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString AddStyle(NKStyle style, int startIndex, int length) {
        if (startIndex < 0 || startIndex >= _text.Length)     throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (length < 0 || startIndex + length > _text.Length) throw new ArgumentOutOfRangeException(nameof(length));

        if (length == 0) 
            return this;

        var chars = this.ToList();

        for (int i = startIndex; i < startIndex + length; i++) {
            var existing = chars[i].Style;
            
            // Layering logic:
            // 1. Keep existing color if the new one is Inherit or Default
            // 2. Merge style flags
            var composed = new NKStyle(
                style.IsFColorInherit || style.IsFColorDefault ? existing.FColor : style.FColor,
                style.IsBColorInherit || style.IsBColorDefault ? existing.BColor : style.BColor,
                existing.Styles | style.Styles
            );
            
            chars[i] = new AnsiChar(chars[i].Char, composed);
        }

        return new AnsiString(chars);
    }

    /// <summary>
    /// Returns a new <see cref="AnsiString"/> where the specified style is composed (layered) with existing styles 
    /// in the specified range.
    /// </summary>
    /// <param name="style">The style attributes to add.</param>
    /// <param name="range">The range of indices to compose the style in.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString AddStyle(NKStyle style, Range range) {
        var (offset, length) = range.GetOffsetAndLength(_text.Length);
        return AddStyle(style, offset, length);
    }

    #endregion

    
    // ============================ Manipulation ============================ // 

    #region Manipulation

    /// <summary>
    /// Chops the string into multiple lines based on the specified width, attempting to wrap at whitespace.
    /// Styles are preserved across line breaks.
    /// </summary>
    /// <param name="width">The maximum number of characters per line.</param>
    /// <returns>An array of <see cref="AnsiString"/> instances representing the chopped lines.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when width is less than or equal to 0.</exception>
    public AnsiString[] Chop(int width) {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (string.IsNullOrEmpty(_text)) return [this];

        var lines = new List<AnsiString>();
        var current = new List<AnsiChar>();
        var lastSpaceIdx = -1;
        
        for (int i = 0; i < _text.Length; i++) {
            char c = _text[i];
            
            if (c == '\n') {
                lines.Add(new AnsiString(current));
                current.Clear();
                lastSpaceIdx = -1;
                continue;
            }
            
            if (char.IsWhiteSpace(c)) {
                lastSpaceIdx = current.Count;
            }
            
            current.Add(this[i]);

            if (current.Count <= width) continue;

            if (lastSpaceIdx != -1) {
                // Break at last space
                var lineChars = current.Take(lastSpaceIdx).ToList();
                lines.Add(new AnsiString(lineChars));
                    
                // Remove the space and everything before it from current
                current.RemoveRange(0, lastSpaceIdx + 1);
            }
            else {
                // No space, break at width
                var lineChars = current.Take(width).ToList();
                lines.Add(new AnsiString(lineChars));
                current.RemoveRange(0, width);
            }

            lastSpaceIdx = -1;
        }
        
        if (current.Count > 0) {
            lines.Add(new AnsiString(current));
        }
        
        return lines.ToArray();
    }

    /// <summary>
    /// Returns a new <see cref="AnsiString"/> that is a substring of this instance, starting at <paramref name="startIndex"/>.
    /// </summary>
    /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString Substring(int startIndex) => Substring(startIndex, _text.Length - startIndex);

    /// <summary>
    /// Returns a new <see cref="AnsiString"/> that is a substring of this instance, starting at <paramref name="startIndex"/>
    /// and has the specified <paramref name="length"/>.
    /// </summary>
    /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
    /// <param name="length">The number of characters in the substring.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when range is outside the bounds of the string.</exception>
    public AnsiString Substring(int startIndex, int length) {
        if (startIndex < 0 || startIndex > _text.Length)      throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (length < 0 || startIndex + length > _text.Length) throw new ArgumentOutOfRangeException(nameof(length));
        
        if (length == 0) return new AnsiString();
        
        string newValue = _text.Substring(startIndex, length);
        List<StyleMarker> newStyles = [];
        
        // Find initial style
        var currentStyle = GetStyleAt(startIndex);
        newStyles.Add(new StyleMarker(0, currentStyle));
        
        // Add markers that are within (startIndex, startIndex + length)
        foreach (var marker in _styles) {
            if (marker.Index > startIndex && marker.Index < startIndex + length) {
                newStyles.Add(new StyleMarker(marker.Index - startIndex, marker.Style));
            }
        }
        
        return new AnsiString(newValue, CleanupMarkers(newStyles));
    }

    /// <summary>
    /// Concatenates two <see cref="AnsiString"/> instances, preserving styles from both and preventing style bleeding.
    /// </summary>
    /// <param name="str0">The first string to concatenate.</param>
    /// <param name="str1">The second string to concatenate.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public static AnsiString Concat(AnsiString? str0, AnsiString? str1) {
        if (str0 == null) return str1 ?? new AnsiString();
        if (str1 == null) return str0;

        string newValue = str0._text + str1._text;
        var newStyles = new List<StyleMarker>(str0._styles);
        
        int offset = str0._text.Length;
        
        // Explicitly add the starting style of str1 at the offset to prevent bleeding
        // if str1 doesn't already have a marker at index 0.
        newStyles.Add(new StyleMarker(offset, str1.GetStyleAt(0)));

        foreach (var marker in str1._styles) {
            newStyles.Add(new StyleMarker(marker.Index + offset, marker.Style));
        }

        return new AnsiString(newValue, CleanupMarkers(newStyles));
    }

    /// <summary>
    /// Concatenates two <see cref="AnsiString"/> instances.
    /// </summary>
    public static AnsiString operator +(AnsiString? str0, AnsiString? str1) => Concat(str0, str1);
    /// <summary>
    /// Concatenates an <see cref="AnsiString"/> and a plain string.
    /// </summary>
    public static AnsiString operator +(AnsiString? str0, string? str1) =>
        Concat(str0, new AnsiString(str1 ?? string.Empty));
    
    /// <summary>
    /// Concatenates a plain string and an <see cref="AnsiString"/>.
    /// </summary>
    public static AnsiString operator +(string? str0, AnsiString? str1) => 
        Concat(new AnsiString(str0 ?? string.Empty), str1);

    /// <summary>
    /// Returns a copy of this string converted to uppercase, preserving all styles.
    /// </summary>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString ToUpper() => new(_text.ToUpper(), [.._styles]);

    /// <summary>
    /// Returns a copy of this string converted to lowercase, preserving all styles.
    /// </summary>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString ToLower() => new(_text.ToLower(), [.._styles]);

    /// <summary>
    /// Removes all leading and trailing white-space characters from the current string.
    /// </summary>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString Trim() => TrimStart().TrimEnd();

    /// <summary>
    /// Removes all leading and trailing occurrences of a set of characters specified in an array from the current string.
    /// </summary>
    /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString Trim(params char[] trimChars) => TrimStart(trimChars).TrimEnd(trimChars);
    
    /// <summary>
    /// Removes all leading white-space characters from the current string.
    /// </summary>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString TrimStart() {
        int i = 0;
        while (i < _text.Length && char.IsWhiteSpace(_text[i])) 
            i++;
        
        return Substring(i);
    }
    
    /// <summary>
    /// Removes all leading occurrences of a set of characters specified in an array from the current string.
    /// </summary>
    /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString TrimStart(params char[] trimChars) {
        if (trimChars.Length == 0) return TrimStart();
        
        int i = 0;
        while (i < _text.Length && trimChars.Contains(_text[i])) 
            i++;
        
        return Substring(i);
    }

    /// <summary>
    /// Removes all trailing white-space characters from the current string.
    /// </summary>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString TrimEnd() {
        int i = _text.Length - 1;
        while (i >= 0 && char.IsWhiteSpace(_text[i])) i--;
        return Substring(0, i + 1);
    }
    
    /// <summary>
    /// Removes all trailing occurrences of a set of characters specified in an array from the current string.
    /// </summary>
    /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString TrimEnd(params char[] trimChars) {
        if (trimChars.Length == 0) return TrimEnd();
        
        int i = _text.Length - 1;
        while (i >= 0 && trimChars.Contains(_text[i])) 
            i--;
        
        return Substring(0, i + 1);
    }

    /// <summary>
    /// Returns a value indicating whether a specified substring occurs within this string.
    /// </summary>
    public bool Contains(string value) => _text.Contains(value);
    
    /// <summary>
    /// Determines whether the beginning of this string instance matches the specified string.
    /// </summary>
    public bool StartsWith(string value) => _text.StartsWith(value);
    
    /// <summary>
    /// Determines whether the end of this string instance matches the specified string.
    /// </summary>
    public bool EndsWith(string value) => _text.EndsWith(value);
    
    /// <summary>
    /// Returns the zero-based index of the first occurrence of the specified string in this instance.
    /// </summary>
    public int IndexOf(string value) => _text.IndexOf(value, StringComparison.Ordinal);
    
    /// <summary>
    /// Returns the zero-based index of the first occurrence of the specified character in this instance.
    /// </summary>
    public int IndexOf(char value) => _text.IndexOf(value);
    
    /// <summary>
    /// Returns the zero-based index of the last occurrence of the specified string in this instance.
    /// </summary>
    public int LastIndexOf(string value) => _text.LastIndexOf(value, StringComparison.Ordinal);
    
    /// <summary>
    /// Returns the zero-based index of the last occurrence of the specified character in this instance.
    /// </summary>
    public int LastIndexOf(char value) => _text.LastIndexOf(value);

    /// <summary>
    /// Returns a new string in which all occurrences of a specified Unicode character in this instance 
    /// are replaced with another specified Unicode character. Styles are preserved.
    /// </summary>
    /// <param name="oldChar">The character to be replaced.</param>
    /// <param name="newChar">The character to replace all occurrences of <paramref name="oldChar"/>.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString Replace(char oldChar, char newChar) {
        return new AnsiString(_text.Replace(oldChar, newChar), [.._styles]);
    }

    /// <summary>
    /// Returns a new string in which all occurrences of a specified string in the current instance 
    /// are replaced with another specified string. New occurrences inherit the style of the first character of the match.
    /// </summary>
    /// <param name="oldValue">The string to be replaced.</param>
    /// <param name="newValue">The string to replace all occurrences of <paramref name="oldValue"/>.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString Replace(string oldValue, string? newValue) {
        newValue ??= string.Empty;
        
        if (string.IsNullOrEmpty(oldValue) || !_text.Contains(oldValue)) 
            return this;

        List<AnsiChar> result = [];
        int i = 0;
        
        while (i < _text.Length) {
            if (i <= _text.Length - oldValue.Length && _text.Substring(i, oldValue.Length) == oldValue) {
                var matchStyle = GetStyleAt(i);
                
                foreach (char c in newValue) {
                    result.Add(new AnsiChar(c, matchStyle));
                }
                
                i += oldValue.Length;
            } 
            else {
                result.Add(this[i]);
                i++;
            }
        }
        
        return new AnsiString(result);
    }

    /// <summary>
    /// Splits a string into substrings based on specified delimiting characters.
    /// </summary>
    /// <param name="separator">An array of Unicode characters that delimit the substrings in this instance.</param>
    /// <returns>An array whose elements contain the styled substrings in this instance.</returns>
    public AnsiString[] Split(params char[] separator) {
        List<AnsiString> parts     = [];
        int              lastIndex = 0;
        
        for (int i = 0; i < _text.Length; i++) {
            if (!separator.Contains(_text[i])) continue;

            parts.Add(Substring(lastIndex, i - lastIndex));
            lastIndex = i + 1;
        }
        
        parts.Add(Substring(lastIndex));
        
        return parts.ToArray();
    }

    /// <summary>
    /// Concatenates the members of a constructed <see cref="IEnumerable{T}"/> collection of type <see cref="AnsiString"/>, 
    /// using the specified separator between each member.
    /// </summary>
    /// <param name="separator">The string to use as a separator.</param>
    /// <param name="values">A collection that contains the strings to concatenate.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public static AnsiString Join(string separator, IEnumerable<AnsiString> values) {
        using var enumerator = values.GetEnumerator();
        if (!enumerator.MoveNext()) return new AnsiString();
        
        var result = enumerator.Current;
        var sep    = new AnsiString(separator);
        
        while (enumerator.MoveNext()) {
            result += sep;
            result += enumerator.Current;
        }
        
        return result ?? throw new InvalidOperationException();
    }

    /// <summary>
    /// Returns a new string in which a specified string is inserted at a specified index position in this instance.
    /// </summary>
    /// <param name="startIndex">The zero-based index position of the insertion.</param>
    /// <param name="value">The string to insert.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString Insert(int startIndex, string value) =>
        Concat(Substring(0, startIndex), new AnsiString(value)) + Substring(startIndex);
    
    /// <summary>
    /// Returns a new string in which all the characters in the current instance, beginning at a specified position 
    /// and continuing through the last position, have been deleted.
    /// </summary>
    /// <param name="startIndex">The zero-based position to begin deleting characters.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString Remove(int startIndex) => 
        Substring(0, startIndex);
    
    /// <summary>
    /// Returns a new string in which a specified number of characters in the current instance beginning 
    /// at a specified position have been deleted.
    /// </summary>
    /// <param name="startIndex">The zero-based position to begin deleting characters.</param>
    /// <param name="count">The number of characters to delete.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString Remove(int startIndex, int count) => 
        Substring(0, startIndex) + Substring(startIndex + count);

    /// <summary>
    /// Returns a new string that right-aligns the characters in this instance by padding them with spaces 
    /// on the left, for a specified total length.
    /// </summary>
    /// <param name="totalWidth">The number of characters in the resulting string.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString PadLeft(int totalWidth) => 
        PadLeft(totalWidth, ' ');
    
    /// <summary>
    /// Returns a new string that right-aligns the characters in this instance by padding them on the left 
    /// with a specified Unicode character, for a specified total length.
    /// </summary>
    /// <param name="totalWidth">The number of characters in the resulting string.</param>
    /// <param name="paddingChar">A Unicode padding character.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString PadLeft(int totalWidth, char paddingChar) {
        if (totalWidth <= _text.Length) 
            return this;
        
        return new AnsiString(new string(paddingChar, totalWidth - _text.Length), GetStyleAt(0)) + this;
    }
    
    /// <summary>
    /// Returns a new string that left-aligns the characters in this instance by padding them with spaces 
    /// on the right, for a specified total length.
    /// </summary>
    /// <param name="totalWidth">The number of characters in the resulting string.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString PadRight(int totalWidth) => 
        PadRight(totalWidth, ' ');
    
    /// <summary>
    /// Returns a new string that left-aligns the characters in this instance by padding them on the right 
    /// with a specified Unicode character, for a specified total length.
    /// </summary>
    /// <param name="totalWidth">The number of characters in the resulting string.</param>
    /// <param name="paddingChar">A Unicode padding character.</param>
    /// <returns>A new <see cref="AnsiString"/> instance.</returns>
    public AnsiString PadRight(int totalWidth, char paddingChar) {
        if (totalWidth <= _text.Length) 
            return this;
        
        var lastStyle = _text.Length > 0 
            ? GetStyleAt(_text.Length - 1) 
            : NKStyle.Default;
        
        return this + new AnsiString(new string(paddingChar, totalWidth - _text.Length), lastStyle);
    }

    #endregion

    // ============================ Parsing ============================ //

    #region Parsing

    /// <summary>
    /// Parses a stylized string containing markers into an <see cref="AnsiString"/>.
    /// </summary>
    /// <param name="input">The stylized string to parse.</param>
    /// <returns>An <see cref="AnsiString"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
    /// <exception cref="FormatException">Thrown when the stylized string has an invalid format.</exception>
    public static AnsiString Parse(string input) {
        if (input == null) throw new ArgumentNullException(nameof(input));
        if (!TryParse(input, out var result)) {
            throw new FormatException("The input string was not in a correct stylized format.");
        }
        return result;
    }

    /// <summary>
    /// Tries to parse a stylized string containing markers into an <see cref="AnsiString"/>.
    /// </summary>
    /// <param name="input">The stylized string to parse.</param>
    /// <param name="result">When this method returns, contains the parsed <see cref="AnsiString"/> if successful; otherwise, an empty <see cref="AnsiString"/>.</param>
    /// <returns><c>true</c> if the string was successfully parsed; otherwise, <c>false</c>.</returns>
    public static bool TryParse(string? input, out AnsiString result) {
        if (input == null) {
            result = new AnsiString();
            return false;
        }

        var plainBuilder     = new StringBuilder();
        var markers          = new List<StyleMarker>();
        var currentStyle     = NKStyle.Default;
        var lastAppliedStyle = NKStyle.Default;

        int i = 0;
        while (i < input.Length) {
            char c = input[i];

            switch (c) {
                case '{' when i + 1 < input.Length && input[i + 1] == '{':
                    EnsureStyleApplied();
                    plainBuilder.Append('{');
                    i += 2;

                    break;
                case '{': {
                    int closingIndex = input.IndexOf('}', i);
                    if (closingIndex == -1) {
                        result = new AnsiString();
                        return false;
                    }
                    
                    string markerText = input.Substring(i + 1, closingIndex - (i + 1));
                    
                    if (!markerText.StartsWith(':')) {
                        result = new AnsiString();
                    
                        return false;
                    }
                    
                    string content = markerText[1..];
                    
                    if (!TryParseMarker(content, ref currentStyle)) {
                        result = new AnsiString();
                        
                        return false;
                    }
                    
                    i = closingIndex + 1;

                    break;
                }
                case '}' when i + 1 < input.Length && input[i + 1] == '}': {
                    EnsureStyleApplied();
                    plainBuilder.Append('}');
                    i += 2;

                    break;
                }
                case '}': {
                    result = new AnsiString();
                    
                    return false;
                }
                default: {
                    EnsureStyleApplied();
                    plainBuilder.Append(c);
                    i++;

                    break;
                }
            }
        }

        result = new AnsiString(plainBuilder.ToString(), CleanupMarkers(markers));
        return true;

        void EnsureStyleApplied() {
            if (currentStyle == lastAppliedStyle) return;

            int currentIndex = plainBuilder.Length;

            if (markers.Count > 0 && markers[^1].Index == currentIndex)
                markers[^1] = new StyleMarker(currentIndex, currentStyle);
            else
                markers.Add(new StyleMarker(currentIndex, currentStyle));

            lastAppliedStyle = currentStyle;
        }
    }

    private static bool TryParseMarker(string content, ref NKStyle style) {
        if (content.StartsWith("f#")) {
            string colorStr = content[2..];
            
            if (!TryParseColor(colorStr, out var color))
                return false;
            
            style = style.SetFColor(color);
            return true;
        }
        
        if (content.StartsWith("b#")) {
            string colorStr = content[2..];
            
            if (!TryParseColor(colorStr, out var color))
                return false;
            
            style = style.SetBColor(color);
            return true;
        }
        
        var flags = TextStyles.NONE;
        foreach (char c in content) {
            switch (c) {
                case 'b': flags |= TextStyles.BOLD;          break;
                case 'i': flags |= TextStyles.ITALIC;        break;
                case 'u': flags |= TextStyles.UNDERLINE;     break;
                case 'f': flags |= TextStyles.FAINT;         break;
                case 'l': flags |= TextStyles.BLINK;         break;
                case 'n': flags |= TextStyles.NEGATIVE;      break;
                case 'v': flags |= TextStyles.INVISIBLE;     break;
                case 's': flags |= TextStyles.STRIKETHROUGH; break;
                default:
                    return false;
            }
        }
        
        style = style.SetStyles(flags);
        return true;
    }

    private static bool TryParseColor(string colorStr, out NKColor color) {
        color = NKColor.Default;
        if (string.IsNullOrEmpty(colorStr)) {
            return false;
        }
        
        string processed = colorStr.Replace('-', '_').ToUpper();
        
        if (processed.Length == 6 && 
            processed.All(c => c is >= '0' and <= '9' or >= 'A' and <= 'F' or >= 'a' and <= 'f')) 
        {
            if (!uint.TryParse(processed, NumberStyles.HexNumber, null, out uint rgb)) 
                return false;

            color = NKColor.FromRgb(rgb);
            return true;
        }
        
        if (string.Equals(processed, "DEFAULT", StringComparison.OrdinalIgnoreCase)) {
            color = NKColor.Default;
            return true;
        }
        
        if (string.Equals(processed, "INHERIT", StringComparison.OrdinalIgnoreCase)) {
            color = NKColor.Inherit;
            return true;
        }
        
        if (Enum.TryParse<NKConsoleColor>(processed, true, out var nkc)) {
            color = new NKColor(nkc);
            return true;
        }
        
        return false;
    }

    #endregion

    #region Helpers

    private static List<StyleMarker> CleanupMarkers(List<StyleMarker> markers) {
        if (markers.Count <= 1) 
            return markers;
        
        List<StyleMarker> cleaned = [];
        NKStyle? lastStyle = null;
        
        foreach (var marker in markers.Where(marker => lastStyle == null || marker.Style != lastStyle.Value)) {
            cleaned.Add(marker);
            lastStyle = marker.Style;
        }
        
        return cleaned;
    }

    #endregion
    
    // ============================ Interfaces ============================ // 

    #region Interface Implementation
    
    /// <summary>
    /// Returns an enumerator that iterates through the collection of <see cref="AnsiChar"/>.
    /// </summary>
    public IEnumerator<AnsiChar> GetEnumerator() {
        for (int i = 0; i < _text.Length; i++) {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Indicates whether the current object is equal to another <see cref="AnsiString"/>.
    /// Equality is based on both the text content and the exact style markers.
    /// </summary>
    public bool Equals(AnsiString? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        if (_text != other._text) return false;
        if (_styles.Count != other._styles.Count) return false;

        for (int i = 0; i < _styles.Count; i++) {
            if (_styles[i].Index != other._styles[i].Index || _styles[i].Style != other._styles[i].Style)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is AnsiString other && Equals(other);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    public override int GetHashCode() {
        unchecked {
            int hash = _text.GetHashCode();
            foreach (var marker in _styles) {
                hash = (hash * 397) ^ marker.Index;
                hash = (hash * 397) ^ marker.Style.GetHashCode();
            }
            return hash;
        }
    }

    /// <summary>
    /// Compares two <see cref="AnsiString"/> instances for equality.
    /// </summary>
    public static bool operator ==(AnsiString? left, AnsiString? right) => Equals(left, right);
    
    /// <summary>
    /// Compares two <see cref="AnsiString"/> instances for inequality.
    /// </summary>
    public static bool operator !=(AnsiString? left, AnsiString? right) => !Equals(left, right);

    /// <summary>
    /// Creates a shallow copy of the <see cref="AnsiString"/>. 
    /// Note that since the class is immutable, this is mostly for interface compliance.
    /// </summary>
    public AnsiString Clone() => new(_text, [.._styles]);
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// Returns a string that represents the current <see cref="AnsiString"/>, including all ANSI escape sequences for styling.
    /// </summary>
    /// <returns>A string with ANSI escape codes.</returns>
    public override string ToString() {
        if (_text.Length == 0) return string.Empty;
        if (_styles.Count == 0) return _text;

        var sb = new StringBuilder();
        var currentStyle = NKStyle.Default;
        int lastIndex = 0;

        foreach (var marker in _styles) {
            // Append text since last marker
            if (marker.Index > lastIndex) {
                sb.Append(_text, lastIndex, marker.Index - lastIndex);
            }

            // Append style change
            sb.Append(NKStyle.GetEscSeq(currentStyle, marker.Style));
            
            currentStyle = marker.Style;
            lastIndex = marker.Index;
        }

        // Append remaining text
        if (lastIndex < _text.Length) {
            sb.Append(_text, lastIndex, _text.Length - lastIndex);
        }

        // Reset style at the end
        if (currentStyle != NKStyle.Default) {
            sb.Append(NKStyle.GetEscSeq(currentStyle, NKStyle.Default));
        }

        return sb.ToString();
    }
    
    public static implicit operator string?(AnsiString? value) => value?.ToString();
    
    #endregion
    
    
    // ============================ Operators ============================ // 

    #region Operators

    /// <summary>
    /// Implicitly converts a plain string to an unstyled <see cref="AnsiString"/>.
    /// </summary>
    public static implicit operator AnsiString(string c) => new(c);

    #endregion
    
    
    // ============================ Markers ============================ // 

    #region Markers
    
    private class StyleMarker {
        public int     Index { get; }
        public NKStyle Style { get; }

        public StyleMarker(int index, NKStyle style) {
            Index = index;
            Style = style;
        }

        public static StyleMarker Default => new(-1, NKStyle.Default);
    }

    private class StyleMarkerComparer : IComparer<StyleMarker> {
        public static readonly StyleMarkerComparer Instance = new();

        public int Compare(StyleMarker? x, StyleMarker? y) {
            if (x is null && y is null) return 0;
            if (x is null) return -1;
            if (y is null) return +1;
            
            return x.Index.CompareTo(y.Index);
        }
    }

    #endregion
}