//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;
using static NeoKolors.Common.NKTextStyles;

namespace NeoKolors.Common;

/// <summary>
/// contains information about console styles (bg / fg color, bold, italic, etc.)
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong) * 2)]
[SuppressMessage("ReSharper", "ShiftExpressionZeroLeftOperand")]
public readonly record struct NKStyle : IFormattable, IParsablePolyfill.IParsable<NKStyle> {

    // field containing all the data for acceleration of some operations
    [FieldOffset(0)] private readonly ulong _raw0;
    [FieldOffset(8)] private readonly ulong _raw1;

    [FieldOffset(0)]  private readonly NKColor          _fColor;
    [FieldOffset(4)]  private readonly NKColor          _bColor;
    [FieldOffset(8)]  private readonly NKTextStyles     _styleData;
    [FieldOffset(9)]  private readonly NKTextStyles     _styleInherit;
    [FieldOffset(10)] private readonly NKUnderlineStyle _underlineStyle;

    /// <summary>
    /// The actual compressed style (lower 64 bits)
    /// </summary>
    public ulong Raw0 => _raw0;

    /// <summary>
    /// Additional style data (upper 64 bits)
    /// </summary>
    public ulong Raw1 => _raw1;

    /// <summary>
    /// represents the color of the text
    /// </summary>
    public NKColor FColor {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetFColor();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        init => _fColor = value;
    }

    /// <summary>
    /// represents the background color
    /// </summary>
    public NKColor BColor {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetBColor();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        init => _bColor = value;
    }

    public NKColor UColor {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Underline.Color;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        init => _underlineStyle = _underlineStyle with { Color = value };
    }

    /// <summary>
    /// bitmap of the individual text styles (see <see cref="NKTextStyles"/>)
    /// </summary>
    public NKTextStyles Styles {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetStyles();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        init => _styleData = value;
    }

    /// <summary>
    /// Text style mode flags reserved for style modifier behavior
    /// </summary>
    public NKTextStyles InheritedStyles {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _styleInherit;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        init => _styleInherit = value;
    }

    public NKUnderlineStyle Underline {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _underlineStyle;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        init => _underlineStyle = value;
    }

    /// <summary>
    /// returns whether the text color is in palette mode or custom mode
    /// </summary>
    public bool IsFColorCustom {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => FColor.IsRgb;
    }

    /// <summary>
    /// returns whether the background color is in palette mode or custom mode
    /// </summary>
    public bool IsBColorCustom {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => BColor.IsRgb;
    }

    /// <summary>
    /// returns whether the text color is the default color
    /// </summary>
    public bool IsFColorDefault {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => FColor.IsDefault;
    }

    /// <summary>
    /// returns whether the background color is the default color
    /// </summary>
    public bool IsBColorDefault {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => BColor.IsDefault;
    }

    /// <summary>
    /// returns whether the text color should be inherited
    /// </summary>
    public bool IsFColorInherit {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => FColor.IsInherit;
    }

    /// <summary>
    /// returns whether the background color should be inherited
    /// </summary>
    public bool IsBColorInherit {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => BColor.IsInherit;
    }

    /// <summary>
    /// returns whether the text color is a console color
    /// </summary>
    public bool IsFColorConsole {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => FColor.IsPalette;
    }

    /// <summary>
    /// returns whether the background color is a console color
    /// </summary>
    public bool IsBColorConsole {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => BColor.IsPalette;
    }
    
    public NKStyle(ulong raw0, ulong raw1) {
        _raw0 = raw0;
        _raw1 = raw1;
    }

    public NKStyle(
        NKColor          textColor       = default,
        NKColor          backgroundColor = default,
        NKTextStyles     styles          = NONE,
        NKTextStyles     inheritedStyles = NONE,
        NKUnderlineStyle underlineStyle  = default
    ) {
        _raw0           = 0;
        _raw1           = 0;
        _fColor         = textColor;
        _bColor         = backgroundColor;
        _styleData      = styles;
        _styleInherit   = inheritedStyles;
        _underlineStyle = underlineStyle;
    }

    public NKStyle() {
        _raw0 = 0;
        _raw1 = 0;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NKColor GetFColor() => _fColor;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NKColor GetBColor() => _bColor;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NKTextStyles GetStyles() => _styleData;

    /// <summary>
    /// Creates a new <see cref="NKStyle"/> instance by applying the specified style properties
    /// from the given <paramref name="other"/> instance.
    /// </summary>
    /// <param name="other">
    /// An instance of <see cref="NKStyle"/> containing the style properties to apply.
    /// </param>
    /// <return>
    /// A new <see cref="NKStyle"/> instance with properties combined from the current instance 
    /// and the <paramref name="other"/> instance.
    /// </return>
    [Pure]
    public NKStyle With(NKStyle other) {
        var f = other._fColor.IsInherit ? _fColor : other._fColor;
        var b = other._bColor.IsInherit ? _bColor : other._bColor;
        var s = (other._styleData & ~other._styleInherit) | (_styleData & other._styleInherit);
        var u = _underlineStyle.With(other._underlineStyle);

        return new NKStyle(f, b, s, other.InheritedStyles, u);
    }

    /// <summary>
    /// Creates a new <see cref="NKStyle"/> instance with the specified foreground color
    /// applied, provided the given color is not marked as inherited.
    /// </summary>
    /// <param name="color">
    /// An instance of <see cref="NKColor"/> representing the foreground color to apply.
    /// If the color is marked as inherited, the current instance is returned unchanged.
    /// </param>
    /// <returns>
    /// A new <see cref="NKStyle"/> instance with the foreground color set to the specified
    /// <paramref name="color"/>, or the current instance if the color is marked as inherited.
    /// </returns>
    [Pure]
    public NKStyle WithFColor(NKColor color) {
        return !color.IsInherit
            ? this with { FColor = color }
            : this;
    }

    /// <summary>
    /// Returns a new <see cref="NKStyle"/> instance with the specified background color applied.
    /// </summary>
    /// <param name="color">
    /// The <see cref="NKColor"/> to set as the background color. If the color is marked as inherited (<see cref="NKColor.IsInherit"/>),
    /// the current instance is returned unchanged.
    /// </param>
    /// <returns>
    /// A new <see cref="NKStyle"/> instance with the background color updated to the specified <paramref name="color"/>,
    /// or the current instance if the color is marked as inherited.
    /// </returns>
    [Pure]
    public NKStyle WithBColor(NKColor color) {
        return !color.IsInherit
            ? this with { BColor = color }
            : this;
    }

    /// <summary>
    /// Creates a new <see cref="NKStyle"/> instance by applying the specified style properties
    /// from the given <paramref name="styles"/> parameter, while also inheriting styles based
    /// on the <paramref name="inheritedStyles"/> parameter.
    /// </summary>
    /// <param name="styles">
    /// An instance of <see cref="NKTextStyles"/> containing the styles to apply to the new <see cref="NKStyle"/> instance.
    /// </param>
    /// <param name="inheritedStyles">
    /// An optional instance of <see cref="NKTextStyles"/> containing the styles to inherit. By default, no styles are inherited.
    /// </param>
    /// <returns>
    /// A new <see cref="NKStyle"/> instance with the specified styles applied, combined
    /// with styles inherited from the <paramref name="inheritedStyles"/> parameter.
    /// </returns>
    [Pure]
    public NKStyle WithStyles(NKTextStyles styles, NKTextStyles inheritedStyles = NONE) {
        return this with {
            Styles = (styles & ~inheritedStyles) | (_styleData & inheritedStyles)
        };
    }

    /// <summary>
    /// Creates a new <see cref="NKStyle"/> instance by applying the specified underline style
    /// from the given <paramref name="underline"/> instance.
    /// </summary>
    /// <param name="underline">
    /// An instance of <see cref="NKUnderlineStyle"/> containing the underline style to apply.
    /// </param>
    /// <return>
    /// A new <see cref="NKStyle"/> instance with the updated underline style based on the
    /// specified <paramref name="underline"/>.
    /// </return>
    [Pure]
    public NKStyle WithUnderline(NKUnderlineStyle underline) {
        return this with { Underline = Underline.With(underline) };
    }
    
    private string ToDbgString() => $"FColor: {FColor:p}, BColor: {BColor:p}{StylesToString()}";

    public override string ToString() => ToString(null, null);

    public string ToString(string? format, IFormatProvider? formatProvider) {
        if (string.IsNullOrEmpty(format))
            return ToAnsi();

        return format switch {
            "p" or "P"     => ToDbgString(),
            _              => ToAnsi()
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToAnsi() => "".AddStyle(this);

    private string StylesToString() {
        var output = new List<string>();
        var styles = GetStyles();

        if (styles.GetIsBold()) output.Add("Bold");
        if (styles.GetIsItalic()) output.Add("Italic");
        if (styles.GetIsUnderline()) output.Add("Underline");
        if (styles.GetIsStrikethrough()) output.Add("Strikethrough");
        if (styles.GetIsFaint()) output.Add("Faint");
        if (styles.GetIsNegative()) output.Add("Negative");
        if (styles.GetIsInvisible()) output.Add("Invisible");
        if (styles.GetIsBlink()) output.Add("Blink");

        return output.Count != 0 ? $", {string.Join(", ", output.ToArray())}" : "";
    }

    public          bool Equals(NKStyle other) => _raw0 == other._raw0 && _raw1 == other._raw1;
    public override int  GetHashCode()         => HashCode.Combine(_raw0, _raw1);

    public static NKStyle operator <<(NKStyle overriden, NKStyle overrider) {
        return overriden.With(overrider);
    }

    public static NKStyle Default => new(NKColor.Default, NKColor.Default);

    [JBPure]
    public static string GetEscSeq(NKStyle prev, NKStyle next) {
        if (prev == next)
            return string.Empty;

        var sb = new StringBuilder("\e[");

        sb.Append(NKTextStyles.GetEscSeq(prev._styleData, next._styleData, next.InheritedStyles, false));
        
        NKColor.AppendInnerF(sb, prev.FColor, next.FColor);
        NKColor.AppendInnerB(sb, prev.BColor, next.BColor);
        NKColor.AppendInnerU(sb, prev.UColor, next.UColor);

        NKUnderlineType.AppendEscSeq(sb, prev.Underline.Type, next.Underline.Type, false);

        if (sb[^1] != ';') {
            return string.Empty;
        }
        
        sb.Remove(sb.Length - 1, 1);
        sb.Append('m');

        return sb.ToString();
    }

    /// <summary>
    /// Generates the escape sequence for applying the specified <paramref name="style"/>
    /// to text output in a terminal, optionally forcing the generation of the sequence
    /// regardless of inheritable properties.
    /// </summary>
    /// <param name="style">
    /// The <see cref="NKStyle"/> instance representing the formatting and color properties
    /// to be applied.
    /// </param>
    /// <param name="force">
    /// A boolean value indicating whether to force the generation of the escape sequence,
    /// overriding inherited properties if <c>true</c>.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> containing the terminal escape sequence to apply the specified
    /// formatting and color properties.
    /// </returns>
    [Pure]
    public static string GetEscSeq(NKStyle style, bool force = false) {
        var sb = new StringBuilder();

        sb.Append("\e[");
        
        sb.Append(NKTextStyles.GetEscSeq(style.Styles, style.InheritedStyles, force, false));

        NKColor.AppendInnerF(sb, style.FColor);
        NKColor.AppendInnerB(sb, style.BColor);
        NKColor.AppendInnerU(sb, style.UColor);
        
        NKUnderlineType.AppendEscSeq(sb, style.Underline.Type, false);

        if (sb[^1] != ';') {
            return string.Empty;
        }
        
        sb.Remove(sb.Length - 1, 1);
        sb.Append('m');
        
        return sb.ToString();
    }

    public static explicit operator NKStyle(NKTextStyles s) => new(NKColor.Default, NKColor.Default, s);

    // ============================== PARSING ============================== //

    #region Parsing

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? formatProvider, out NKStyle result) {
        if (s == null) {
            result = default;

            return false;
        }

        var parts      = s.Split([';', ','], StringSplitOptions.RemoveEmptyEntries);
        var textStyles = NONE;
        var f          = NKColor.Default;
        var b          = NKColor.Default;
        var t          = NONE;

        foreach (var rawPart in parts) {
            var part = rawPart.Trim();

            if (part.Length == 0)
                continue;

            if (part.StartsWith("f#", StringComparison.OrdinalIgnoreCase)) {
                var colorStr = part[2..];

                if (!TryParseStyleColor(colorStr, out var color)) {
                    result = default;

                    return false;
                }

                f = color;
            }
            else if (part.StartsWith("b#", StringComparison.OrdinalIgnoreCase)) {
                var colorStr = part[2..];

                if (!TryParseStyleColor(colorStr, out var color)) {
                    result = default;

                    return false;
                }

                b = color;
            }
            else if (TryParseTextStyle(part, out var ts)) {
                textStyles |= ts;
            }
            else {
                result = default;

                return false;
            }
        }

        if (textStyles != NONE) {
            t = textStyles;
        }

        result = new NKStyle(f, b, t);

        return true;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, out NKStyle result) => TryParse(s, null, out result);
    
    public static NKStyle Parse(string? s, IFormatProvider? provider) {
        if (s == null)
            throw new ArgumentNullException(nameof(s));

        return TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"Invalid style format: '{s}'");
    }
    
    public static NKStyle Parse([NotNullWhen(true)] string? s) => Parse(s, null);

    private static bool TryParseStyleColor(string val, out NKColor color) {
        if (string.IsNullOrEmpty(val)) {
            color = default;

            return false;
        }

        val = val.Trim();
        string normalized = val.StartsWith('#') ? val : val.Replace('-', '_');

        if (NKColor.TryParse(normalized, null, out color)) {
            return true;
        }

        if (!normalized.StartsWith('#')                               &&
            normalized.Length > 0                                     &&
            normalized.All(c => "0123456789abcdefABCDEF".Contains(c)) &&
            NKColor.TryParse("#" + normalized, null, out color)) {
            return true;
        }

        color = default;

        return false;
    }

    private static bool TryParseTextStyle(string s, out NKTextStyles style) {
        var normalized = s.Replace('-', '_').ToLowerInvariant();

        (style, var ret) = normalized switch {
            "bold"          => (BOLD, true),
            "faint"         => (FAINT, true),
            "italic"        => (ITALIC, true),
            "underline"     => (UNDERLINE, true),
            "blink"         => (BLINK, true),
            "negative"      => (NEGATIVE, true),
            "invisible"     => (INVISIBLE, true),
            "strikethrough" => (STRIKETHROUGH, true),
            "none"          => (NONE, true),
            "all"           => (ALL, true),
            _               => (NONE, false)
        };

        return ret;
    }
    
    #endregion
}