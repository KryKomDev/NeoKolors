// NeoKolors
// Copyright (c) 2025 KryKom

using System.Collections;
using System.Diagnostics.Contracts;
using System.Text;

namespace NeoKolors.Common;

public sealed class AnsiString :
    IEnumerable<AnsiChar>,
    IEquatable<AnsiString>,
    ICloneable {
    #region Fields and Properties

    private readonly string _value;
    private readonly HashSet<StyleMarker> _styles;

    public int Length => _value.Length;
    public string String => _value;

    public char this[int index] => _value[index];

    #endregion

    #region Constructors

    public AnsiString() {
        _value = string.Empty;
        _styles = [];
    }


    public AnsiString(string value) {
        _value = value.Replace("\n\r", "\n");
        _styles = [];
    }

    public AnsiString(IEnumerable<AnsiChar> chars) {
        var ansiChars = chars as AnsiChar[] ?? chars.ToArray();

        var val = new char[ansiChars.Count()];
        _styles = [];

        var prevStyle = NKStyle.Default;

        for (var i = 0; i < ansiChars.Length; i++) {
            var c = ansiChars[i];

            if (prevStyle != c.Style) {
                prevStyle = c.Style;
                _styles.Add(new StyleMarker(i, c.Style));
            }

            val[i] = c.Char;
        }

        _value = new string(val);
    }

    public AnsiString(string value, NKStyle style) {
        _value = value;
        _styles = [new StyleMarker(0, style)];
    }

    private AnsiString(string value, HashSet<StyleMarker> styles) {
        _value = value;
        _styles = styles;
    }

    #endregion

    #region Styling

    /// <summary>
    /// Applies the specified style to the entire <see cref="AnsiString"/> and combines it with existing styles.
    /// </summary>
    /// <param name="style">The <see cref="NKStyle"/> to apply to the string.</param>
    /// <returns>A new <see cref="AnsiString"/> instance with the applied style.</returns>
    public AnsiString Style(NKStyle style) => new(_value, [.._styles, new StyleMarker(0, style)]);

    /// <summary>
    /// Applies a specified style to the <see cref="AnsiString"/> starting at the given index
    /// and clears any styles already applied at or beyond that index.
    /// </summary>
    /// <param name="style">The <see cref="NKStyle"/> to be applied.</param>
    /// <param name="index">The position in the string, specified as an <see cref="Index"/>,
    /// where the style application begins.</param>
    [Pure]
    public AnsiString Style(NKStyle style, Index index) {
        var styles = new HashSet<StyleMarker>(_styles);

        var i = index.GetOffset(_value.Length);
        styles.RemoveWhere(s => s.Index >= i);
        styles.Add(new StyleMarker(i, style));

        return new AnsiString(_value, styles);
    }

    /// <summary>
    /// Applies a specified style to a subset of the <see cref="AnsiString"/> defined by the given range.
    /// This method clears any previously applied styles within the specified range and replaces them
    /// with the provided style, maintaining styles outside the range.
    /// </summary>
    /// <param name="style">The <see cref="NKStyle"/> to be applied to the specified range of the string.</param>
    /// <param name="range">The range within the <see cref="AnsiString"/> to which the style should be applied.</param>
    [Pure]
    public AnsiString Style(NKStyle style, Range range) {
        var start = range.Start.GetOffset(_value.Length);
        var end = range.End.GetOffset(_value.Length);

        var styles = _styles.ToArray();
        Array.Sort(styles);
        var last = NKStyle.Default;

        for (int i = 0; i < styles.Length; i++) {
            if (styles[i].Index >= end)
                break;

            last = styles[i].Style;
        }

        var newStyles = new HashSet<StyleMarker>(_styles);

        newStyles.RemoveWhere(s => s.Index >= start && s.Index <= end);
        newStyles.Add(new StyleMarker(start, style));
        newStyles.Add(new StyleMarker(end, last));

        return new AnsiString(_value, newStyles);
    }

    /// <summary>
    /// Updates the style applied to the entire <see cref="AnsiString"/>.
    /// This method clears all previously applied styles and replaces them with the specified style.
    /// </summary>
    /// <param name="style">The new <see cref="NKStyle"/> to be applied to the string.</param>
    [Pure]
    public AnsiString Restyle(NKStyle style) => new(_value, [new StyleMarker(0, style)]);

    #endregion

    public override string ToString() {
        var markers = _styles.ToArray();
        Array.Sort(markers);

        if (markers.Length == 0) {
            return _value;
        }

        var sb = new StringBuilder();
        var mi = 0;
        var nextMarker = markers.Length > 0 ? markers[0] : StyleMarker.Default;
        var currentMarker = StyleMarker.Default;
        var currentStyle = currentMarker.Style;

        for (int i = 0; i < _value.Length; i++) {
            if (nextMarker.Index == i) {
                currentMarker = nextMarker;
                var lastStyle = currentStyle;
                currentStyle = currentMarker.Style;
                nextMarker = mi + 1 < markers.Length ? markers[++mi] : new StyleMarker(int.MaxValue, NKStyle.Default);
                sb.Append(NKStyle.GetEscSeq(lastStyle, currentStyle));
            }

            sb.Append(_value[i]);
        }

        return sb.ToString();
    }

    public AnsiString Clone() => new(_value, [.._styles]);

    object ICloneable.Clone() => Clone();

    #region IEnumerable

    public IEnumerator<AnsiChar> GetEnumerator() {
        var markers = _styles.ToArray();
        Array.Sort(markers);

        if (markers.Length == 0) {
            for (int i = 0; i < _value.Length; i++)
                yield return new AnsiChar(_value[i], NKStyle.Default);

            yield break;
        }

        var mi = 0;
        var nextMarker = markers.Length > 0 ? markers[0] : StyleMarker.Default;
        var currentStyle = NKStyle.Default;

        for (int i = 0; i < _value.Length; i++) {
            if (nextMarker.Index == i) {
                currentStyle = currentStyle.Override(nextMarker.Style);
                nextMarker = mi < markers.Length - 1 ? markers[++mi] : nextMarker;
            }

            yield return new AnsiChar(_value[i], currentStyle);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region Ops

    public static implicit operator AnsiString(string value) => new(value);
    public static implicit operator string(AnsiString value) => value.ToString();

    public static bool operator ==(AnsiString left, AnsiString right) => left.Equals(right);
    public static bool operator !=(AnsiString left, AnsiString right) => !left.Equals(right);

    public bool Equals(AnsiString? other) =>
        other is not null && _value == other._value && _styles.SetEquals(other._styles);

    public override bool Equals(object? obj) => obj is AnsiString other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(_value, _styles);

    #endregion

    #region Manipulation

    /// <summary>
    /// Splits the <see cref="AnsiString"/> into an array of substrings, each with a maximum specified length.
    /// </summary>
    /// <param name="maxWidth">The maximum length of each substring. Must be a positive integer.</param>
    /// <returns>An array of <see cref="AnsiString"/> instances, each representing a portion of the original string.</returns>
    public AnsiString[] Chop(int maxWidth) {
        if (maxWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxWidth), "Maximum length must be positive");
        
        if (string.IsNullOrWhiteSpace(_value))
            return [];

        var lines = new List<AnsiString>();
        var chars = this.ToArray();

        int? lastSpace = null;
        int lastBreak = 0;
        int w = 0;
        
        for (int i = 0; i < _value.Length; i++) {
            if (_value[i] == '\n') {
                lines.Add(new AnsiString(chars[lastBreak..i]));
                lastBreak = i + 1;
                w = 0;
                lastSpace = null;
            }

            if (_value[i] == ' ') {
                lastSpace = i;
            }
            
            if (w < maxWidth) {
                w++;
                continue;
            }

            if (lastSpace.HasValue) {
                lines.Add(new AnsiString(chars[lastBreak..(lastSpace.Value)]));
                i = lastSpace.Value + 1;
                lastBreak = lastSpace.Value + 1;
                lastSpace = null;
            }
            else {
                lines.Add(new AnsiString(chars[lastBreak..(i - 1)]));
                lastBreak = i - 1;
            }

            w = 0;
        }

        if (lastBreak != _value.Length) {
            lines.Add(new AnsiString(chars[lastBreak..]));
        }
        
        return lines.ToArray();
    }

    public AnsiString Trim(int length) {
        return length <= 0
            ? throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than or equal to zero.")
            : _value.Length <= length
                ? this
                : new AnsiString(this.ToArray());
    }
    
    #endregion

    private readonly struct StyleMarker : IEquatable<StyleMarker>, IComparable<StyleMarker> {
        public int Index { get; }
        public NKStyle Style { get; }

        public StyleMarker(int index, NKStyle style) {
            Index = index;
            Style = style;
        }

        public static StyleMarker Default => new(-1, NKStyle.Default);

        public bool Equals(StyleMarker other) => Index == other.Index;
        public override bool Equals(object? obj) => obj is StyleMarker other && Equals(other);
        public override int GetHashCode() => Index;
        public static bool operator ==(StyleMarker left, StyleMarker right) => left.Equals(right);
        public static bool operator !=(StyleMarker left, StyleMarker right) => !left.Equals(right);

        public int CompareTo(StyleMarker other) => Index.CompareTo(other.Index);
    }
}