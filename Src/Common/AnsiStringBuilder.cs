// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Common;

/// <summary>
/// Represents a mutable string of ANSI styled characters.
/// </summary>
public sealed class AnsiStringBuilder {
    private readonly List<AnsiChar> _chars;

    /// <summary>
    /// Gets or sets the default style to be applied to newly appended unstyled text.
    /// </summary>
    public NKStyle CurrentStyle { get; set; } = NKStyle.Default;

    /// <summary>
    /// Gets the number of characters in the builder.
    /// </summary>
    public int Length {
        get => _chars.Count;
        set {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Length cannot be less than zero.");

            if (value < _chars.Count) {
                _chars.RemoveRange(value, _chars.Count - value);
            }
            else if (value > _chars.Count) {
                int needed = value - _chars.Count;

                for (int i = 0; i < needed; i++) {
                    _chars.Add(new AnsiChar('\0', NKStyle.Default));
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the capacity of the builder.
    /// </summary>
    public int Capacity {
        get => _chars.Capacity;
        set => _chars.Capacity = value;
    }

    /// <summary>
    /// Gets or sets the <see cref="AnsiChar"/> at the specified index.
    /// </summary>
    public AnsiChar this[int index] {
        get => _chars[index];
        set => _chars[index] = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiStringBuilder"/> class.
    /// </summary>
    public AnsiStringBuilder() {
        _chars = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiStringBuilder"/> class with the specified capacity.
    /// </summary>
    public AnsiStringBuilder(int capacity) {
        _chars = new List<AnsiChar>(capacity);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiStringBuilder"/> class containing the specified string.
    /// </summary>
    public AnsiStringBuilder(string? value) {
        _chars = [];

        if (value != null) {
            Append(value);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiStringBuilder"/> class containing the specified string with the specified capacity.
    /// </summary>
    public AnsiStringBuilder(string? value, int capacity) {
        _chars = new List<AnsiChar>(capacity);

        if (value != null) {
            Append(value);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiStringBuilder"/> class containing the specified <see cref="AnsiString"/>.
    /// </summary>
    public AnsiStringBuilder(AnsiString? value) {
        _chars = [];

        if (value != null) {
            Append(value);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiStringBuilder"/> class containing the specified <see cref="AnsiString"/> with the specified capacity.
    /// </summary>
    public AnsiStringBuilder(AnsiString? value, int capacity) {
        _chars = new List<AnsiChar>(capacity);

        if (value != null) {
            Append(value);
        }
    }

    // ============================ Append Methods ============================ //

    /// <summary>
    /// Appends a character using the current style.
    /// </summary>
    public AnsiStringBuilder Append(char value) {
        _chars.Add(new AnsiChar(value, CurrentStyle));

        return this;
    }

    /// <summary>
    /// Appends a character using the specified style.
    /// </summary>
    public AnsiStringBuilder Append(char value, NKStyle style) {
        _chars.Add(new AnsiChar(value, style));

        return this;
    }

    /// <summary>
    /// Appends a character repeated a specified number of times using the current style.
    /// </summary>
    public AnsiStringBuilder Append(char value, int repeatCount) {
        if (repeatCount < 0)
            throw new ArgumentOutOfRangeException(nameof(repeatCount), "Repeat count cannot be less than zero.");

        for (int i = 0; i < repeatCount; i++) {
            _chars.Add(new AnsiChar(value, CurrentStyle));
        }

        return this;
    }

    /// <summary>
    /// Appends a character repeated a specified number of times using the specified style.
    /// </summary>
    public AnsiStringBuilder Append(char value, int repeatCount, NKStyle style) {
        if (repeatCount < 0)
            throw new ArgumentOutOfRangeException(nameof(repeatCount), "Repeat count cannot be less than zero.");

        for (int i = 0; i < repeatCount; i++) {
            _chars.Add(new AnsiChar(value, style));
        }

        return this;
    }

    /// <summary>
    /// Appends a string using the current style.
    /// </summary>
    public AnsiStringBuilder Append(string? value) {
        if (value == null)
            return this;

        foreach (char c in value) {
            _chars.Add(new AnsiChar(c, CurrentStyle));
        }

        return this;
    }

    /// <summary>
    /// Appends a string using the specified style.
    /// </summary>
    public AnsiStringBuilder Append(string? value, NKStyle style) {
        if (value == null)
            return this;

        foreach (char c in value) {
            _chars.Add(new AnsiChar(c, style));
        }

        return this;
    }

    /// <summary>
    /// Appends a substring using the current style.
    /// </summary>
    public AnsiStringBuilder Append(string? value, int startIndex, int count) {
        if (value == null) {
            if (startIndex == 0 && count == 0)
                return this;

            throw new ArgumentNullException(nameof(value));
        }

        if (startIndex < 0 || startIndex > value.Length)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if (count < 0 || startIndex + count > value.Length)
            throw new ArgumentOutOfRangeException(nameof(count));

        for (int i = startIndex; i < startIndex + count; i++) {
            _chars.Add(new AnsiChar(value[i], CurrentStyle));
        }

        return this;
    }

    /// <summary>
    /// Appends a substring using the specified style.
    /// </summary>
    public AnsiStringBuilder Append(string? value, int startIndex, int count, NKStyle style) {
        if (value == null) {
            if (startIndex == 0 && count == 0)
                return this;

            throw new ArgumentNullException(nameof(value));
        }

        if (startIndex < 0 || startIndex > value.Length)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if (count < 0 || startIndex + count > value.Length)
            throw new ArgumentOutOfRangeException(nameof(count));

        for (int i = startIndex; i < startIndex + count; i++) {
            _chars.Add(new AnsiChar(value[i], style));
        }

        return this;
    }

    /// <summary>
    /// Appends an <see cref="AnsiString"/>.
    /// </summary>
    public AnsiStringBuilder Append(AnsiString? value) {
        if (value == null)
            return this;

        for (int i = 0; i < value.Length; i++) {
            _chars.Add(value[i]);
        }

        return this;
    }

    /// <summary>
    /// Appends a portion of an <see cref="AnsiString"/>.
    /// </summary>
    public AnsiStringBuilder Append(AnsiString? value, int startIndex, int count) {
        if (value == null) {
            if (startIndex == 0 && count == 0)
                return this;

            throw new ArgumentNullException(nameof(value));
        }

        if (startIndex < 0 || startIndex > value.Length)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if (count < 0 || startIndex + count > value.Length)
            throw new ArgumentOutOfRangeException(nameof(count));

        for (int i = startIndex; i < startIndex + count; i++) {
            _chars.Add(value[i]);
        }

        return this;
    }

    /// <summary>
    /// Appends an <see cref="AnsiChar"/>.
    /// </summary>
    public AnsiStringBuilder Append(AnsiChar value) {
        _chars.Add(value);

        return this;
    }

    /// <summary>
    /// Appends an <see cref="AnsiChar"/> repeated a specified number of times.
    /// </summary>
    public AnsiStringBuilder Append(AnsiChar value, int repeatCount) {
        if (repeatCount < 0)
            throw new ArgumentOutOfRangeException(nameof(repeatCount), "Repeat count cannot be less than zero.");

        for (int i = 0; i < repeatCount; i++) {
            _chars.Add(value);
        }

        return this;
    }

    /// <summary>
    /// Appends a collection of <see cref="AnsiChar"/>.
    /// </summary>
    public AnsiStringBuilder Append(IEnumerable<AnsiChar> chars) {
        if (chars == null)
            throw new ArgumentNullException(nameof(chars));

        _chars.AddRange(chars);

        return this;
    }

    /// <summary>
    /// Appends the string representation of an object using the current style.
    /// </summary>
    public AnsiStringBuilder Append(object? value) {
        if (value == null)
            return this;

        return Append(value.ToString());
    }

    /// <summary>
    /// Appends the string representation of an object using the specified style.
    /// </summary>
    public AnsiStringBuilder Append(object? value, NKStyle style) {
        if (value == null)
            return this;

        return Append(value.ToString(), style);
    }

    public AnsiStringBuilder Append(bool    value)                => Append(value.ToString());
    public AnsiStringBuilder Append(bool    value, NKStyle style) => Append(value.ToString(), style);
    public AnsiStringBuilder Append(int     value)                => Append(value.ToString(CultureInfo.InvariantCulture));
    public AnsiStringBuilder Append(int     value, NKStyle style) => Append(value.ToString(CultureInfo.InvariantCulture), style);
    public AnsiStringBuilder Append(long    value)                => Append(value.ToString(CultureInfo.InvariantCulture));
    public AnsiStringBuilder Append(long    value, NKStyle style) => Append(value.ToString(CultureInfo.InvariantCulture), style);
    public AnsiStringBuilder Append(float   value)                => Append(value.ToString(CultureInfo.InvariantCulture));
    public AnsiStringBuilder Append(float   value, NKStyle style) => Append(value.ToString(CultureInfo.InvariantCulture), style);
    public AnsiStringBuilder Append(double  value)                => Append(value.ToString(CultureInfo.InvariantCulture));
    public AnsiStringBuilder Append(double  value, NKStyle style) => Append(value.ToString(CultureInfo.InvariantCulture), style);
    public AnsiStringBuilder Append(decimal value)                => Append(value.ToString(CultureInfo.InvariantCulture));
    public AnsiStringBuilder Append(decimal value, NKStyle style) => Append(value.ToString(CultureInfo.InvariantCulture), style);

    // ============================ AppendLine Methods ============================ //

    /// <summary>
    /// Appends a line terminator using the current style.
    /// </summary>
    public AnsiStringBuilder AppendLine() {
        return Append(Environment.NewLine);
    }

    /// <summary>
    /// Appends a line terminator using the specified style.
    /// </summary>
    public AnsiStringBuilder AppendLine(NKStyle style) {
        return Append(Environment.NewLine, style);
    }

    /// <summary>
    /// Appends a character followed by a line terminator using the current style.
    /// </summary>
    public AnsiStringBuilder AppendLine(char value) {
        Append(value);

        return AppendLine();
    }

    /// <summary>
    /// Appends a character followed by a line terminator using the specified style.
    /// </summary>
    public AnsiStringBuilder AppendLine(char value, NKStyle style) {
        Append(value, style);

        return AppendLine(style);
    }

    /// <summary>
    /// Appends a string followed by a line terminator using the current style.
    /// </summary>
    public AnsiStringBuilder AppendLine(string? value) {
        Append(value);

        return AppendLine();
    }

    /// <summary>
    /// Appends a string followed by a line terminator using the specified style.
    /// </summary>
    public AnsiStringBuilder AppendLine(string? value, NKStyle style) {
        Append(value, style);

        return AppendLine(style);
    }

    /// <summary>
    /// Appends an <see cref="AnsiString"/> followed by a line terminator.
    /// </summary>
    public AnsiStringBuilder AppendLine(AnsiString? value) {
        Append(value);

        return AppendLine();
    }

    /// <summary>
    /// Appends an <see cref="AnsiChar"/> followed by a line terminator.
    /// </summary>
    public AnsiStringBuilder AppendLine(AnsiChar value) {
        Append(value);

        return AppendLine();
    }

    // ============================ Insert Methods ============================ //

    /// <summary>
    /// Inserts a character at the specified index using the current style.
    /// </summary>
    public AnsiStringBuilder Insert(int index, char value) {
        if (index < 0 || index > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        _chars.Insert(index, new AnsiChar(value, CurrentStyle));

        return this;
    }

    /// <summary>
    /// Inserts a character at the specified index using the specified style.
    /// </summary>
    public AnsiStringBuilder Insert(int index, char value, NKStyle style) {
        if (index < 0 || index > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        _chars.Insert(index, new AnsiChar(value, style));

        return this;
    }

    /// <summary>
    /// Inserts a character repeated a specified number of times at the specified index using the current style.
    /// </summary>
    public AnsiStringBuilder Insert(int index, char value, int repeatCount) {
        if (index < 0 || index > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (repeatCount < 0)
            throw new ArgumentOutOfRangeException(nameof(repeatCount), "Repeat count cannot be less than zero.");

        var items = new AnsiChar[repeatCount];

        for (int i = 0; i < repeatCount; i++) {
            items[i] = new AnsiChar(value, CurrentStyle);
        }

        _chars.InsertRange(index, items);

        return this;
    }

    /// <summary>
    /// Inserts a character repeated a specified number of times at the specified index using the specified style.
    /// </summary>
    public AnsiStringBuilder Insert(int index, char value, int repeatCount, NKStyle style) {
        if (index < 0 || index > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (repeatCount < 0)
            throw new ArgumentOutOfRangeException(nameof(repeatCount), "Repeat count cannot be less than zero.");

        var items = new AnsiChar[repeatCount];

        for (int i = 0; i < repeatCount; i++) {
            items[i] = new AnsiChar(value, style);
        }

        _chars.InsertRange(index, items);

        return this;
    }

    /// <summary>
    /// Inserts a string at the specified index using the current style.
    /// </summary>
    public AnsiStringBuilder Insert(int index, string? value) {
        if (index < 0 || index > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (value == null)
            return this;

        var items = new AnsiChar[value.Length];

        for (int i = 0; i < value.Length; i++) {
            items[i] = new AnsiChar(value[i], CurrentStyle);
        }

        _chars.InsertRange(index, items);

        return this;
    }

    /// <summary>
    /// Inserts a string at the specified index using the specified style.
    /// </summary>
    public AnsiStringBuilder Insert(int index, string? value, NKStyle style) {
        if (index < 0 || index > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (value == null)
            return this;

        var items = new AnsiChar[value.Length];

        for (int i = 0; i < value.Length; i++) {
            items[i] = new AnsiChar(value[i], style);
        }

        _chars.InsertRange(index, items);

        return this;
    }

    /// <summary>
    /// Inserts a string repeated a specified number of times at the specified index using the current style.
    /// </summary>
    public AnsiStringBuilder Insert(int index, string? value, int repeatCount) {
        if (index < 0 || index > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (repeatCount < 0)
            throw new ArgumentOutOfRangeException(nameof(repeatCount), "Repeat count cannot be less than zero.");

        if (value == null || repeatCount == 0)
            return this;

        var items = new AnsiChar[value.Length * repeatCount];
        int k     = 0;

        for (int r = 0; r < repeatCount; r++) {
            for (int i = 0; i < value.Length; i++) {
                items[k++] = new AnsiChar(value[i], CurrentStyle);
            }
        }

        _chars.InsertRange(index, items);

        return this;
    }

    /// <summary>
    /// Inserts a string repeated a specified number of times at the specified index using the specified style.
    /// </summary>
    public AnsiStringBuilder Insert(int index, string? value, int repeatCount, NKStyle style) {
        if (index < 0 || index > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (repeatCount < 0)
            throw new ArgumentOutOfRangeException(nameof(repeatCount), "Repeat count cannot be less than zero.");

        if (value == null || repeatCount == 0)
            return this;

        var items = new AnsiChar[value.Length * repeatCount];
        int k     = 0;

        for (int r = 0; r < repeatCount; r++) {
            for (int i = 0; i < value.Length; i++) {
                items[k++] = new AnsiChar(value[i], style);
            }
        }

        _chars.InsertRange(index, items);

        return this;
    }

    /// <summary>
    /// Inserts an <see cref="AnsiString"/> at the specified index.
    /// </summary>
    public AnsiStringBuilder Insert(int index, AnsiString? value) {
        if (index < 0 || index > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (value == null)
            return this;

        var items = new AnsiChar[value.Length];

        for (int i = 0; i < value.Length; i++) {
            items[i] = value[i];
        }

        _chars.InsertRange(index, items);

        return this;
    }

    /// <summary>
    /// Inserts an <see cref="AnsiString"/> repeated a specified number of times at the specified index.
    /// </summary>
    public AnsiStringBuilder Insert(int index, AnsiString? value, int repeatCount) {
        if (index < 0 || index > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (repeatCount < 0)
            throw new ArgumentOutOfRangeException(nameof(repeatCount), "Repeat count cannot be less than zero.");

        if (value == null || repeatCount == 0)
            return this;

        var items = new AnsiChar[value.Length * repeatCount];
        int k     = 0;

        for (int r = 0; r < repeatCount; r++) {
            for (int i = 0; i < value.Length; i++) {
                items[k++] = value[i];
            }
        }

        _chars.InsertRange(index, items);

        return this;
    }

    /// <summary>
    /// Inserts an <see cref="AnsiChar"/> at the specified index.
    /// </summary>
    public AnsiStringBuilder Insert(int index, AnsiChar value) {
        if (index < 0 || index > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        _chars.Insert(index, value);

        return this;
    }

    /// <summary>
    /// Inserts an <see cref="AnsiChar"/> repeated a specified number of times at the specified index.
    /// </summary>
    public AnsiStringBuilder Insert(int index, AnsiChar value, int repeatCount) {
        if (index < 0 || index > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (repeatCount < 0)
            throw new ArgumentOutOfRangeException(nameof(repeatCount), "Repeat count cannot be less than zero.");

        var items = new AnsiChar[repeatCount];

        for (int i = 0; i < repeatCount; i++) {
            items[i] = value;
        }

        _chars.InsertRange(index, items);

        return this;
    }

    // ============================ Remove / Clear ============================ //

    /// <summary>
    /// Removes the specified range of characters.
    /// </summary>
    public AnsiStringBuilder Remove(int startIndex, int length) {
        if (startIndex < 0 || startIndex > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if (length < 0 || startIndex + length > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(length));

        _chars.RemoveRange(startIndex, length);

        return this;
    }

    /// <summary>
    /// Clears all characters and styles from the builder.
    /// </summary>
    public AnsiStringBuilder Clear() {
        _chars.Clear();

        return this;
    }

    // ============================ Replace Methods ============================ //

    /// <summary>
    /// Replaces all occurrences of a specified character with another specified character.
    /// The style of each character is preserved.
    /// </summary>
    public AnsiStringBuilder Replace(char oldChar, char newChar) {
        return Replace(oldChar, newChar, 0, _chars.Count);
    }

    /// <summary>
    /// Replaces all occurrences of a specified character in a range with another specified character.
    /// The style of each character is preserved.
    /// </summary>
    public AnsiStringBuilder Replace(char oldChar, char newChar, int startIndex, int count) {
        if (startIndex < 0 || startIndex > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if (count < 0 || startIndex + count > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(count));

        for (int i = startIndex; i < startIndex + count; i++) {
            if (_chars[i].Char == oldChar) {
                _chars[i] = new AnsiChar(newChar, _chars[i].Style);
            }
        }

        return this;
    }

    /// <summary>
    /// Replaces all occurrences of a specified string with another specified string.
    /// New characters inherit the style of the first character of each match.
    /// </summary>
    public AnsiStringBuilder Replace(string oldValue, string? newValue) {
        return Replace(oldValue, newValue, 0, _chars.Count);
    }

    /// <summary>
    /// Replaces all occurrences of a specified string in a range with another specified string.
    /// New characters inherit the style of the first character of each match.
    /// </summary>
    public AnsiStringBuilder Replace(string oldValue, string? newValue, int startIndex, int count) {
        if (oldValue == null)
            throw new ArgumentNullException(nameof(oldValue));

        if (oldValue.Length == 0)
            throw new ArgumentException("String cannot be of zero length.", nameof(oldValue));

        if (startIndex < 0 || startIndex > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if (count < 0 || startIndex + count > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(count));

        newValue ??= string.Empty;

        int limit = startIndex + count;
        int i     = startIndex;

        while (i <= limit - oldValue.Length) {
            bool match = true;

            for (int j = 0; j < oldValue.Length; j++) {
                if (_chars[i + j].Char != oldValue[j]) {
                    match = false;

                    break;
                }
            }

            if (match) {
                var matchStyle = _chars[i].Style;
                _chars.RemoveRange(i, oldValue.Length);

                var newChars = new AnsiChar[newValue.Length];

                for (int j = 0; j < newValue.Length; j++) {
                    newChars[j] = new AnsiChar(newValue[j], matchStyle);
                }

                _chars.InsertRange(i, newChars);

                int diff = newValue.Length - oldValue.Length;
                limit += diff;
                i     += newValue.Length;
            }
            else {
                i++;
            }
        }

        return this;
    }

    // ============================ Styling Methods ============================ //

    /// <summary>
    /// Overwrites the style of the entire builder content with the specified style.
    /// </summary>
    public AnsiStringBuilder ApplyStyle(NKStyle style) {
        return ApplyStyle(style, 0, _chars.Count);
    }

    /// <summary>
    /// Overwrites the style of a range with the specified style.
    /// </summary>
    public AnsiStringBuilder ApplyStyle(NKStyle style, int startIndex, int length) {
        if (startIndex < 0 || startIndex > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if (length < 0 || startIndex + length > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(length));

        for (int i = startIndex; i < startIndex + length; i++) {
            _chars[i] = new AnsiChar(_chars[i].Char, style);
        }

        return this;
    }

    /// <summary>
    /// Overwrites the style of the specified range with the specified style.
    /// </summary>
    public AnsiStringBuilder ApplyStyle(NKStyle style, Range range) {
        var (offset, length) = range.GetOffsetAndLength(_chars.Count);

        return ApplyStyle(style, offset, length);
    }

    /// <summary>
    /// Composes (layers) the specified style with existing styles for the entire builder content.
    /// Non-default properties of <paramref name="style"/> will override existing ones.
    /// </summary>
    public AnsiStringBuilder AddStyle(NKStyle style) {
        return AddStyle(style, 0, _chars.Count);
    }

    /// <summary>
    /// Composes (layers) the specified style with existing styles in the specified range.
    /// Non-default properties of <paramref name="style"/> will override existing ones.
    /// </summary>
    public AnsiStringBuilder AddStyle(NKStyle style, int startIndex, int length) {
        if (startIndex < 0 || startIndex > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if (length < 0 || startIndex + length > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(length));

        for (int i = startIndex; i < startIndex + length; i++) {
            var existing = _chars[i].Style;

            var composed = new NKStyle(
                style.IsFColorInherit || style.IsFColorDefault ? existing.FColor : style.FColor,
                style.IsBColorInherit || style.IsBColorDefault ? existing.BColor : style.BColor,
                existing.Styles | style.Styles
            );

            _chars[i] = new AnsiChar(_chars[i].Char, composed);
        }

        return this;
    }

    /// <summary>
    /// Composes (layers) the specified style with existing styles in the specified range.
    /// Non-default properties of <paramref name="style"/> will override existing ones.
    /// </summary>
    public AnsiStringBuilder AddStyle(NKStyle style, Range range) {
        var (offset, length) = range.GetOffsetAndLength(_chars.Count);

        return AddStyle(style, offset, length);
    }

    /// <summary>
    /// Modifies the style of each character in the builder using the specified delegate.
    /// </summary>
    public AnsiStringBuilder ModifyStyle(Func<NKStyle, NKStyle> modifier) => ModifyStyle(modifier, 0, _chars.Count);

    /// <summary>
    /// Modifies the style of each character from <paramref name="startIndex"/> to the end using the specified delegate.
    /// </summary>
    public AnsiStringBuilder ModifyStyle(Func<NKStyle, NKStyle> modifier, int startIndex) => ModifyStyle(modifier, startIndex, _chars.Count - startIndex);

    /// <summary>
    /// Modifies the style of each character in the specified range using the specified delegate.
    /// </summary>
    public AnsiStringBuilder ModifyStyle(Func<NKStyle, NKStyle> modifier, int startIndex, int length) {
        if (modifier == null)
            throw new ArgumentNullException(nameof(modifier));

        return ModifyStyle((style, _) => modifier(style), startIndex, length);
    }

    /// <summary>
    /// Modifies the style of each character in the specified range using the specified delegate.
    /// </summary>
    public AnsiStringBuilder ModifyStyle(Func<NKStyle, NKStyle> modifier, Range range) {
        var (offset, length) = range.GetOffsetAndLength(_chars.Count);

        return ModifyStyle(modifier, offset, length);
    }

    /// <summary>
    /// Modifies the style of each character in the builder using the specified delegate, passing the character index.
    /// </summary>
    public AnsiStringBuilder ModifyStyle(Func<NKStyle, int, NKStyle> modifier) => ModifyStyle(modifier, 0, _chars.Count);

    /// <summary>
    /// Modifies the style of each character from <paramref name="startIndex"/> to the end using the specified delegate, passing the character index.
    /// </summary>
    public AnsiStringBuilder ModifyStyle(Func<NKStyle, int, NKStyle> modifier, int startIndex) => ModifyStyle(modifier, startIndex, _chars.Count - startIndex);

    /// <summary>
    /// Modifies the style of each character in the specified range using the specified delegate, passing the character index.
    /// </summary>
    public AnsiStringBuilder ModifyStyle(Func<NKStyle, int, NKStyle> modifier, int startIndex, int length) {
        if (modifier == null)
            throw new ArgumentNullException(nameof(modifier));

        if (startIndex < 0 || startIndex > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if (length < 0 || startIndex + length > _chars.Count)
            throw new ArgumentOutOfRangeException(nameof(length));

        for (int i = startIndex; i < startIndex + length; i++) {
            _chars[i] = new AnsiChar(_chars[i].Char, modifier(_chars[i].Style, i));
        }

        return this;
    }

    /// <summary>
    /// Modifies the style of each character in the specified range using the specified delegate, passing the character index.
    /// </summary>
    public AnsiStringBuilder ModifyStyle(Func<NKStyle, int, NKStyle> modifier, Range range) {
        var (offset, length) = range.GetOffsetAndLength(_chars.Count);

        return ModifyStyle(modifier, offset, length);
    }

    /// <summary>
    /// Updates only the foreground color for the entire builder content, preserving background color and text styles.
    /// </summary>
    public AnsiStringBuilder SetFColor(NKColor color) => SetFColor(color, 0, _chars.Count);

    /// <summary>
    /// Updates only the foreground color from <paramref name="startIndex"/> to the end, preserving background color and text styles.
    /// </summary>
    public AnsiStringBuilder SetFColor(NKColor color, int startIndex) => SetFColor(color, startIndex, _chars.Count - startIndex);

    /// <summary>
    /// Updates only the foreground color in the specified range, preserving background color and text styles.
    /// </summary>
    public AnsiStringBuilder SetFColor(NKColor color, int startIndex, int length) => ModifyStyle(s => s.WithFColor(color), startIndex, length);

    /// <summary>
    /// Updates only the foreground color in the specified range, preserving background color and text styles.
    /// </summary>
    public AnsiStringBuilder SetFColor(NKColor color, Range range) => ModifyStyle(s => s.WithFColor(color), range);

    /// <summary>
    /// Updates only the background color for the entire builder content, preserving text color and text styles.
    /// </summary>
    public AnsiStringBuilder SetBColor(NKColor color) => SetBColor(color, 0, _chars.Count);

    /// <summary>
    /// Updates only the background color from <paramref name="startIndex"/> to the end, preserving text color and text styles.
    /// </summary>
    public AnsiStringBuilder SetBColor(NKColor color, int startIndex) => SetBColor(color, startIndex, _chars.Count - startIndex);

    /// <summary>
    /// Updates only the background color in the specified range, preserving text color and text styles.
    /// </summary>
    public AnsiStringBuilder SetBColor(NKColor color, int startIndex, int length) => ModifyStyle(s => s.WithBColor(color), startIndex, length);

    /// <summary>
    /// Updates only the background color in the specified range, preserving text color and text styles.
    /// </summary>
    public AnsiStringBuilder SetBColor(NKColor color, Range range) => ModifyStyle(s => s.WithBColor(color), range);

    /// <summary>
    /// Sets text style flags for the entire builder content, preserving text color and background color.
    /// </summary>
    public AnsiStringBuilder SetStyles(NKTextStyles styles) => SetStyles(styles, 0, _chars.Count);

    /// <summary>
    /// Sets text style flags from <paramref name="startIndex"/> to the end, preserving text color and background color.
    /// </summary>
    public AnsiStringBuilder SetStyles(NKTextStyles styles, int startIndex) => SetStyles(styles, startIndex, _chars.Count - startIndex);

    /// <summary>
    /// Sets text style flags in the specified range, preserving text color and background color.
    /// </summary>
    public AnsiStringBuilder SetStyles(NKTextStyles styles, int startIndex, int length) => ModifyStyle(s => s.WithStyles(styles), startIndex, length);

    /// <summary>
    /// Sets text style flags in the specified range, preserving text color and background color.
    /// </summary>
    public AnsiStringBuilder SetStyles(NKTextStyles styles, Range range) => ModifyStyle(s => s.WithStyles(styles), range);

    /// <summary>
    /// Adds specified text style flags (bitwise OR) for the entire builder content.
    /// </summary>
    public AnsiStringBuilder AddStyles(NKTextStyles styles) => AddStyles(styles, 0, _chars.Count);

    /// <summary>
    /// Adds specified text style flags (bitwise OR) from <paramref name="startIndex"/> to the end.
    /// </summary>
    public AnsiStringBuilder AddStyles(NKTextStyles styles, int startIndex) => AddStyles(styles, startIndex, _chars.Count - startIndex);

    /// <summary>
    /// Adds specified text style flags (bitwise OR) in the specified range.
    /// </summary>
    public AnsiStringBuilder AddStyles(NKTextStyles styles, int startIndex, int length) => ModifyStyle(s => s.WithStyles(s.Styles | styles), startIndex, length);

    /// <summary>
    /// Adds specified text style flags (bitwise OR) in the specified range.
    /// </summary>
    public AnsiStringBuilder AddStyles(NKTextStyles styles, Range range) => ModifyStyle(s => s.WithStyles(s.Styles | styles), range);

    /// <summary>
    /// Removes specified text style flags (bitwise AND NOT) for the entire builder content.
    /// </summary>
    public AnsiStringBuilder RemoveStyles(NKTextStyles styles) => RemoveStyles(styles, 0, _chars.Count);

    /// <summary>
    /// Removes specified text style flags (bitwise AND NOT) from <paramref name="startIndex"/> to the end.
    /// </summary>
    public AnsiStringBuilder RemoveStyles(NKTextStyles styles, int startIndex) => RemoveStyles(styles, startIndex, _chars.Count - startIndex);

    /// <summary>
    /// Removes specified text style flags (bitwise AND NOT) in the specified range.
    /// </summary>
    public AnsiStringBuilder RemoveStyles(NKTextStyles styles, int startIndex, int length) => ModifyStyle(s => s.WithStyles(s.Styles & ~styles), startIndex, length);

    /// <summary>
    /// Removes specified text style flags (bitwise AND NOT) in the specified range.
    /// </summary>
    public AnsiStringBuilder RemoveStyles(NKTextStyles styles, Range range) => ModifyStyle(s => s.WithStyles(s.Styles & ~styles), range);

    /// <summary>
    /// Toggles specified text style flags (bitwise XOR) for the entire builder content.
    /// </summary>
    public AnsiStringBuilder ToggleStyles(NKTextStyles styles) => ToggleStyles(styles, 0, _chars.Count);

    /// <summary>
    /// Toggles specified text style flags (bitwise XOR) from <paramref name="startIndex"/> to the end.
    /// </summary>
    public AnsiStringBuilder ToggleStyles(NKTextStyles styles, int startIndex) => ToggleStyles(styles, startIndex, _chars.Count - startIndex);

    /// <summary>
    /// Toggles specified text style flags (bitwise XOR) in the specified range.
    /// </summary>
    public AnsiStringBuilder ToggleStyles(NKTextStyles styles, int startIndex, int length) => ModifyStyle(s => s.WithStyles(s.Styles ^ styles), startIndex, length);

    /// <summary>
    /// Toggles specified text style flags (bitwise XOR) in the specified range.
    /// </summary>
    public AnsiStringBuilder ToggleStyles(NKTextStyles styles, Range range) => ModifyStyle(s => s.WithStyles(s.Styles ^ styles), range);

    /// <summary>
    /// Overrides non-inherit attributes of existing styles for the entire builder content with the specified style.
    /// </summary>
    public AnsiStringBuilder OverrideStyle(NKStyle style) => OverrideStyle(style, 0, _chars.Count);

    /// <summary>
    /// Overrides non-inherit attributes of existing styles from <paramref name="startIndex"/> to the end with the specified style.
    /// </summary>
    public AnsiStringBuilder OverrideStyle(NKStyle style, int startIndex) => OverrideStyle(style, startIndex, _chars.Count - startIndex);

    /// <summary>
    /// Overrides non-inherit attributes of existing styles in the specified range with the specified style.
    /// </summary>
    public AnsiStringBuilder OverrideStyle(NKStyle style, int startIndex, int length) => ModifyStyle(s => s.With(style), startIndex, length);

    /// <summary>
    /// Overrides non-inherit attributes of existing styles in the specified range with the specified style.
    /// </summary>
    public AnsiStringBuilder OverrideStyle(NKStyle style, Range range) => ModifyStyle(s => s.With(style), range);

    // ============================ Conversion / Rendering ============================ //

    /// <summary>
    /// Creates and returns a new <see cref="AnsiString"/> from the content of the builder.
    /// </summary>
    public AnsiString ToAnsiString() {
        return new AnsiString(_chars);
    }

    /// <summary>
    /// Returns the raw ANSI escaped string representation of the builder content.
    /// </summary>
    public override string ToString() {
        return ToAnsiString().ToString();
    }

    public static implicit operator AnsiString?(AnsiStringBuilder? builder) => builder?.ToAnsiString();
}