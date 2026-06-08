//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;

namespace NeoKolors.Common;

/// <summary>
/// contains information about console styles (bg / fg color, bold, italic, etc.)
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong))]
[SuppressMessage("ReSharper", "ShiftExpressionZeroLeftOperand")]
public record struct NKStyle : IFormattable {
    
    //
    // Layout (0 is most significant):
    // (this may not be up to date, check offset constants for more info)
    //
    // _raw:
    // | 0 - 23 | 24 - 47 | 48 - 55 | 56   | 57   |
    // |--------|---------|---------|------|------|
    // | FColor | BColor  | Styles  | FCSw | BCSw |
    //
    // FColor (when FCSw == 0):
    // | 14   | 15   | 16 - 23 |
    // |------|------|---------|
    // | FISw | FDSw | Color   |
    //
    // BColor (when BCSw == 0):
    // | 38   | 39   | 40 - 47 |
    // |------|------|---------|
    // | BISw | BDSw | Color   |
    //
    // CSw == 1             -> color is Custom.
    // DSw == 0             -> color is Default.
    // ISw == 1             -> color is Inherit.
    // DSw == 1 && ISw == 0 -> color is Console.
    //


    // ----- OFFSET CONSTANTS ----- //

    private const byte FORG_COL_SIZE = 24;
    private const byte BCKG_COL_SIZE = 24;
    private const byte STYLES_SIZE = 8;
    private const byte FORG_CSW_SIZE = 1;
    private const byte BCKG_CSW_SIZE = 1;

    // 0-based offsets from the left
    private const byte FORG_COL_OFFSET = 0;
    private const byte BCKG_COL_OFFSET = FORG_COL_OFFSET + FORG_COL_SIZE;
    private const byte STYLES_OFFSET = BCKG_COL_OFFSET + BCKG_COL_SIZE;
    private const byte FORG_CSW_OFFSET = STYLES_OFFSET + STYLES_SIZE;
    private const byte BCKG_CSW_OFFSET = FORG_CSW_OFFSET + FORG_CSW_SIZE;

    private const byte FORG_USW_OFFSET = FORG_COL_OFFSET + 14;
    private const byte FORG_USW_SIZE = 2;
    private const byte BCKG_USW_OFFSET = BCKG_COL_OFFSET + 14;
    private const byte BCKG_USW_SIZE = 2;

    private const ulong COL_TYP_DEFAULT = 0b00;
    private const ulong COL_TYP_CONSOLE = 0b01;
    private const ulong COL_TYP_INHERIT = 0b10;

    private const byte TOTAL_SIZE = FORG_COL_SIZE + BCKG_COL_SIZE + STYLES_SIZE + FORG_CSW_SIZE + BCKG_CSW_SIZE;
    private const int UNUSED_SIZE = sizeof(ulong) * 8 - TOTAL_SIZE;

    private const ulong BMP_24 = 0x00_00_00_00_00_ff_ff_fful;
    private const ulong BMP_08 = 0x00_00_00_00_00_00_00_fful;

    [FieldOffset(0)] private ulong _raw;

    // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
    [FieldOffset(1)] private readonly byte _styles;
    [FieldOffset(4)] private readonly byte _bCol_red;
    [FieldOffset(3)] private readonly byte _bCol_green;
    [FieldOffset(2)] private readonly byte _bCol_blue;
    [FieldOffset(3)] private readonly byte _bCol_switches;
    [FieldOffset(2)] private readonly byte _bCol_console;
    [FieldOffset(7)] private readonly byte _fCol_red;
    [FieldOffset(6)] private readonly byte _fCol_green;
    [FieldOffset(5)] private readonly byte _fCol_blue;
    [FieldOffset(6)] private readonly byte _fCol_switches;
    [FieldOffset(5)] private readonly byte _fCol_console;
    // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

    /// <summary>
    /// The actual compressed style
    /// Bit 0 represents the most significant bit and bit 64 the least significant bit.
    /// </summary>
    public ulong Raw => _raw;

    /// <summary>
    /// represents the color of the text
    /// </summary>
    public NKColor FColor {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetFColor();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => SetFColor(value);
    }

    /// <summary>
    /// represents the background color
    /// </summary>
    public NKColor BColor {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetBColor();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => SetBColor(value);
    }

    /// <summary>
    /// bitmap of the individual text styles (see <see cref="TextStyles"/>)
    /// </summary>
    /// <seealso cref="TextStyles"/>
    public TextStyles Styles {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetStyles();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => SetStyles(value);
    }

    /// <summary>
    /// returns whether the text color is in palette mode or custom mode
    /// </summary>
    public bool IsFColorCustom {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_raw >> (64 - FORG_CSW_OFFSET - FORG_CSW_SIZE) & 1ul) == 1;
    }

    /// <summary>
    /// returns whether the background color is in palette mode or custom mode
    /// </summary>
    public bool IsBColorCustom {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_raw >> (64 - BCKG_CSW_OFFSET - BCKG_CSW_SIZE) & 1ul) == 1;
    }

    /// <summary>
    /// returns whether the text color is the default color
    /// </summary>
    public bool IsFColorDefault {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsFColorCustom && (_raw >> (64 - FORG_USW_OFFSET - FORG_USW_SIZE) & 0b11ul) == COL_TYP_DEFAULT;
    }

    /// <summary>
    /// returns whether the background color is the default color
    /// </summary>
    public bool IsBColorDefault {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsBColorCustom && (_raw >> (64 - BCKG_USW_OFFSET - BCKG_USW_SIZE) & 0b11ul) == COL_TYP_DEFAULT;
    }

    /// <summary>
    /// returns whether the text color should be inherited
    /// </summary>
    public bool IsFColorInherit {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsFColorCustom && (_raw >> (64 - FORG_USW_OFFSET - FORG_USW_SIZE) & 0b11ul) == COL_TYP_INHERIT;
    }

    /// <summary>
    /// returns whether the background color should be inherited
    /// </summary>
    public bool IsBColorInherit {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsBColorCustom && (_raw >> (64 - BCKG_USW_OFFSET - BCKG_USW_SIZE) & 0b11ul) == COL_TYP_INHERIT;
    }

    /// <summary>
    /// returns whether the text color is a console color
    /// </summary>
    public bool IsFColorConsole {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsFColorCustom && (_raw >> (64 - FORG_USW_OFFSET - FORG_USW_SIZE) & 0b11ul) == COL_TYP_CONSOLE;
    }

    /// <summary>
    /// returns whether the background color is a console color
    /// </summary>
    public bool IsBColorConsole {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsBColorCustom && (_raw >> (64 - BCKG_USW_OFFSET - BCKG_USW_SIZE) & 0b11ul) == COL_TYP_CONSOLE;
    }

    /// <summary>
    /// sets the color of the text to a custom color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NKStyle SetFColor(uint color) {
        _raw = _raw & ~(BMP_24  << (64 - FORG_COL_OFFSET - FORG_COL_SIZE))
            | ((color & BMP_24) << (64 - FORG_COL_OFFSET - FORG_COL_SIZE))
            | (1ul              << (64 - FORG_CSW_OFFSET - FORG_CSW_SIZE));

        return this;
    }

    /// <summary>
    /// sets the color of the text to a console color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NKStyle SetFColor(NKConsoleColor color) {
        _raw = _raw & ~(BMP_24  << (64 - FORG_COL_OFFSET - FORG_COL_SIZE))
                    & ~(1ul     << (64 - FORG_CSW_OFFSET - FORG_CSW_SIZE))
            | (((ulong)color & BMP_24) << (64 - FORG_COL_OFFSET - FORG_COL_SIZE))
            & ~(0b11ul         << (64 - FORG_USW_OFFSET - FORG_USW_SIZE))
            | (COL_TYP_CONSOLE << (64 - FORG_USW_OFFSET - FORG_USW_SIZE));

        return this;
    }

    /// <summary>
    /// sets the color to be the default color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NKStyle SetFColor() {
        _raw = _raw & ~(BMP_24 << (64 - FORG_COL_OFFSET - FORG_COL_SIZE))
                    & ~(1ul    << (64 - FORG_CSW_OFFSET - FORG_CSW_SIZE))
            & ~(0b11ul         << (64 - FORG_USW_OFFSET - FORG_USW_SIZE))
            | (COL_TYP_DEFAULT << (64 - FORG_USW_OFFSET - FORG_USW_SIZE));

        return this;
    }

    /// <summary>
    /// sets the color to be the default color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NKStyle SetFColor(DefaultColor _) => SetFColor();

    /// <summary>
    /// sets the color to inherit mode
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NKStyle SetFColor(InheritColor _) {
        _raw = _raw & ~(BMP_24 << (64 - FORG_COL_OFFSET - FORG_COL_SIZE))
                    & ~(1ul    << (64 - FORG_CSW_OFFSET - FORG_CSW_SIZE))
            | (0b11ul          << (64 - FORG_USW_OFFSET - FORG_USW_SIZE))
            & (COL_TYP_INHERIT << (64 - FORG_USW_OFFSET - FORG_USW_SIZE));

        return this;
    }

    /// <summary>
    /// sets the color of the text to a color
    /// </summary>
    public NKStyle SetFColor(NKColor color) {
        return color.Type switch {
            NKColor.ColorType.DEFAULT       => SetFColor(),
            NKColor.ColorType.CONSOLE_COLOR => SetFColor(color.AsPalette),
            NKColor.ColorType.RGB           => SetFColor(color.AsRgb),
            NKColor.ColorType.INHERIT       => SetFColor(new InheritColor()),
            _                               => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// safely sets the color of the text to a color
    /// </summary>
    public NKStyle SafeSetFColor(NKColor color) {
        return color.Type switch {
            NKColor.ColorType.DEFAULT       => SetFColor(),
            NKColor.ColorType.CONSOLE_COLOR => SetFColor(color.AsPalette),
            NKColor.ColorType.RGB           => SetFColor(color.AsRgb),
            NKColor.ColorType.INHERIT       => this,
            _                               => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// sets the color of the background to a custom color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NKStyle SetBColor(uint color) {
        _raw = _raw & ~(BMP_24  << (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE))
            | ((color & BMP_24) << (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE))
            | (1ul              << (64 - BCKG_CSW_OFFSET - BCKG_CSW_SIZE));

        return this;
    }

    /// <summary>
    /// sets the color of the background to a console color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NKStyle SetBColor(NKConsoleColor color) {
        _raw = _raw & ~(BMP_24         << (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE))
                    & ~(1ul            << (64 - BCKG_CSW_OFFSET - BCKG_CSW_SIZE))
            | (((ulong)color & BMP_24) << (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE))
            & ~(0b11ul         << (64 - BCKG_USW_OFFSET - BCKG_USW_SIZE))
            | (COL_TYP_CONSOLE << (64 - BCKG_USW_OFFSET - BCKG_USW_SIZE));

        return this;
    }

    /// <summary>
    /// sets the color of the background to be the default color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NKStyle SetBColor() {
        _raw = _raw & ~(BMP_24 << (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE))
                    & ~(1ul    << (64 - BCKG_CSW_OFFSET - BCKG_CSW_SIZE))
            & ~(0b11ul         << (64 - BCKG_USW_OFFSET - BCKG_USW_SIZE))
            | (COL_TYP_DEFAULT << (64 - BCKG_USW_OFFSET - BCKG_USW_SIZE));

        return this;
    }

    /// <summary>
    /// sets the color of the background to be the default color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NKStyle SetBColor(DefaultColor _) => SetBColor();

    /// <summary>
    /// sets the background color to inherit mode
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NKStyle SetBColor(InheritColor _) {
        _raw = _raw & ~(BMP_24 << (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE))
                    & ~(1ul    << (64 - BCKG_CSW_OFFSET - BCKG_CSW_SIZE))
            | (0b11ul          << (64 - BCKG_USW_OFFSET - BCKG_USW_SIZE))
            & (COL_TYP_INHERIT << (64 - BCKG_USW_OFFSET - BCKG_USW_SIZE));

        return this;
    }

    /// <summary>
    /// sets the background color to a color
    /// </summary>
    public NKStyle SetBColor(NKColor color) {
        return color.Type switch {
            NKColor.ColorType.DEFAULT       => SetBColor(),
            NKColor.ColorType.CONSOLE_COLOR => SetBColor(color.AsPalette),
            NKColor.ColorType.RGB           => SetBColor(color.AsRgb),
            NKColor.ColorType.INHERIT       => SetBColor(new InheritColor()),
            _                               => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// safely sets the background color to a color
    /// </summary>
    [Pure]
    public NKStyle SafeSetBColor(NKColor color) {
        return color.Type switch {
            NKColor.ColorType.DEFAULT       => SetBColor(),
            NKColor.ColorType.CONSOLE_COLOR => SetBColor(color.AsPalette),
            NKColor.ColorType.RGB           => SetBColor(color.AsRgb),
            NKColor.ColorType.INHERIT       => this,
            _                               => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// returns the text color of the specified field
    /// </summary>
    [Pure]
    public NKColor GetFColor() {
        if (IsFColorCustom)
            return new NKColor((int)(_raw >> (64 - FORG_COL_OFFSET - FORG_COL_SIZE) & BMP_24));

        return (ulong)_fCol_switches switch {
            COL_TYP_INHERIT => NKColor.Inherit,
            COL_TYP_DEFAULT => NKColor.Default,
            COL_TYP_CONSOLE => new NKColor((NKConsoleColor)_fCol_console),
            _               => throw new InvalidColorCastException($"Unknown text color switch '{_fCol_switches}'.")
        };
    }

    /// <summary>
    /// returns the text color of the specified field
    /// </summary>
    [Pure]
    public NKColor GetBColor() {
        if (IsBColorCustom)
            return new NKColor((int)(_raw >> (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE) & BMP_24));

        return (ulong)_bCol_switches switch {
            COL_TYP_INHERIT => NKColor.Inherit,
            COL_TYP_DEFAULT => NKColor.Default,
            COL_TYP_CONSOLE => new NKColor((NKConsoleColor)_bCol_console),
            _ => throw new InvalidColorCastException($"Unknown background color switch '{_bCol_switches}'.")
        };
    }

    /// <summary>
    /// sets the text styles
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NKStyle SetStyles(TextStyles styles) {
        _raw = _raw & ~(BMP_08 << (64 - STYLES_OFFSET - STYLES_SIZE)) |
            ((ulong)styles     << (64 - STYLES_OFFSET - STYLES_SIZE));

        return this;
    }

    /// <summary>
    /// returns the text styles
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    public TextStyles GetStyles() =>
        (TextStyles)(_raw >> (64 - STYLES_OFFSET - STYLES_SIZE) & BMP_08);

    /// <summary>
    /// safely overwrites the contents of this instance with the contents of the other instance
    /// </summary>
    public NKStyle OverrideWith(NKStyle other) {
        var n = new NKStyle(_raw);
        if (!other.IsFColorInherit) n = n.SetFColor(other.GetFColor());
        if (!other.IsBColorInherit && !other.IsBColorDefault) n = n.SetBColor(other.GetBColor());
        n = n.SetStyles(other.GetStyles());

        return n;
    }

    [Pure]
    private static ulong Init(NKColor f, NKColor b, TextStyles s) => InitS(InitB(InitF(0, f), b), s);

    private static ulong InitF(ulong v, NKColor f) {
        return f.Type switch {
            NKColor.ColorType.DEFAULT => v,
            NKColor.ColorType.RGB => v
                | (1ul            << (64 - FORG_CSW_OFFSET - FORG_CSW_SIZE))
                | ((ulong)f.AsRgb << (64 - FORG_COL_OFFSET - FORG_COL_SIZE)),
            NKColor.ColorType.CONSOLE_COLOR => v
                | (COL_TYP_CONSOLE    << (64 - FORG_USW_OFFSET - FORG_USW_SIZE))
                | ((ulong)f.AsPalette << (64 - FORG_COL_OFFSET - FORG_COL_SIZE)),
            NKColor.ColorType.INHERIT => v
                | (COL_TYP_INHERIT << (64 - FORG_USW_OFFSET - FORG_USW_SIZE)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static ulong InitB(ulong v, NKColor b) {
        return b.Type switch {
            NKColor.ColorType.DEFAULT => v,
            NKColor.ColorType.RGB => v
                | (1ul            << (64 - BCKG_CSW_OFFSET - BCKG_CSW_SIZE))
                | ((ulong)b.AsRgb << (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE)),
            NKColor.ColorType.CONSOLE_COLOR => v
                | (COL_TYP_CONSOLE    << (64 - BCKG_USW_OFFSET - BCKG_USW_SIZE))
                | ((ulong)b.AsPalette << (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE)),
            NKColor.ColorType.INHERIT => v
                | (COL_TYP_INHERIT << (64 - BCKG_USW_OFFSET - BCKG_USW_SIZE)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static ulong InitS(ulong v, TextStyles s) => v | ((ulong)s << (64 - STYLES_OFFSET - STYLES_SIZE));

    private NKStyle(ulong raw) => _raw = raw;

    public NKStyle(NKColor f = default, NKColor b = default, TextStyles s = TextStyles.NONE)
        => _raw = Init(f, b, s);

    public NKStyle() {
        _raw = 0;
    }

    public string ToDbgString() => $"FColor: {FColor:p}, BColor: {BColor:p}{StylesToString()}";

    public override string ToString() => ToString(null, null);
    
    public string ToString(string? format, IFormatProvider? formatProvider) {
        if (string.IsNullOrEmpty(format)) return ToAnsi();
        return format switch {
            "p" or "P"     => ToDbgString(),
            "bmp" or "BMP" => ToBitmapString(),
            _              => ToAnsi()
        };
    }

    public string ToBitmapString() {
        var sb = new StringBuilder();

        sb.Append($"{$"{_raw >> 56 & 0xFF:b8}"    .AddColor(NKConsoleColor.RED)}_");
        sb.Append($"{$"{_raw >> 50 & 0b111111:b6}".AddColor(NKConsoleColor.RED)}");
        sb.Append($"{$"{_raw >> 49 & 0b1:b1}"     .AddColor(NKConsoleColor.YELLOW)}");
        sb.Append($"{$"{_raw >> 48 & 0b1:b1}"     .AddColor(NKConsoleColor.GREEN)}_");
        sb.Append($"{$"{_raw >> 40 & 0xFF:b8}"    .AddColor(NKConsoleColor.RED)}_");

        sb.Append($"{$"{_raw >> 32 & 0xFF:b8}"    .AddColor(NKConsoleColor.BLUE)}_");
        sb.Append($"{$"{_raw >> 26 & 0b111111:b6}".AddColor(NKConsoleColor.BLUE)}");
        sb.Append($"{$"{_raw >> 25 & 0b1:b1}"     .AddColor(NKConsoleColor.MAGENTA)}");
        sb.Append($"{$"{_raw >> 24 & 0b1:b1}"     .AddColor(NKConsoleColor.CYAN)}_");
        sb.Append($"{$"{_raw >> 16 & 0xFF:b8}"    .AddColor(NKConsoleColor.BLUE)}_");

        sb.Append($"{$"{_raw >> 8 & 0xFF:b8}".AddColor(NKConsoleColor.WHITE)}_");
        sb.Append($"{$"{_raw >> 0 & 0xFF:b8}".AddColor(NKConsoleColor.DARK_GRAY)}");

        return sb.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToAnsi() => "".AddStyle(this);

    private string StylesToString() {
        var output = new List<string>();
        var styles = GetStyles();

        if (styles.GetIsBold())          output.Add("Bold");
        if (styles.GetIsItalic())        output.Add("Italic");
        if (styles.GetIsUnderline())     output.Add("Underline");
        if (styles.GetIsStrikethrough()) output.Add("Strikethrough");
        if (styles.GetIsFaint())         output.Add("Faint");
        if (styles.GetIsNegative())      output.Add("Negative");
        if (styles.GetIsInvisible())     output.Add("Invisible");
        if (styles.GetIsBlink())         output.Add("Blink");

        return output.Count != 0 ? $", {string.Join(", ", output.ToArray())}" : "";
    }

    public bool Equals(NKStyle other) => _raw == other._raw;
    public override int GetHashCode() => _raw.GetHashCode();

    /// <summary>
    /// Overrides the properties of the first NKStyle instance with those of the second NKStyle instance and returns the updated instance.
    /// </summary>
    /// <param name="overriden">The NKStyle instance to be overridden.</param>
    /// <param name="overrider">The NKStyle instance providing the overriding properties.</param>
    /// <returns>The resulting NKStyle instance after applying the overrides.</returns>
    public static NKStyle operator <<(NKStyle overriden, NKStyle overrider) {
        return overriden.OverrideWith(overrider);
    }

    public static NKStyle Default => new(NKColor.Default, NKColor.Default);

    [JBPure]
    public static string GetEscSeq(NKStyle prev, NKStyle next) {
        if (prev == next) return string.Empty;

        var off = prev.Styles & ~next.Styles;
        var on = ~prev.Styles & next.Styles;

        var sb = new StringBuilder("\e[");

        off.AppendNegModes(sb);
        on.AppendPosModes(sb);
        NKColor.AppendInnerF(sb, prev.FColor, next.FColor);
        NKColor.AppendInnerB(sb, prev.BColor, next.BColor);

        sb.Remove(sb.Length - 1, 1);
        sb.Append('m');

        return sb.ToString();
    }
    
    public static explicit operator NKStyle(TextStyles s) => new(NKColor.Default, NKColor.Default, s);
}