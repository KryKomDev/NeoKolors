//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common.Util;

namespace NeoKolors.Common;

/// <summary>
/// contains information about console styles (bg / fg color, bold, italic, etc.)
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong))]
public struct NKStyle : ICloneable, IEquatable<NKStyle>, IFormattable {

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
    
    private const byte FORG_COL_SIZE   = 24;
    private const byte BCKG_COL_SIZE   = 24;
    private const byte STYLES_SIZE     = 8;
    private const byte FORG_CSW_SIZE   = 1;
    private const byte BCKG_CSW_SIZE   = 1;
    
    // 0-based offsets from the left
    private const byte FORG_COL_OFFSET = 0;
    private const byte BCKG_COL_OFFSET = FORG_COL_OFFSET + FORG_COL_SIZE;
    private const byte STYLES_OFFSET   = BCKG_COL_OFFSET + BCKG_COL_SIZE;
    private const byte FORG_CSW_OFFSET = STYLES_OFFSET   + STYLES_SIZE;
    private const byte BCKG_CSW_OFFSET = FORG_CSW_OFFSET + FORG_CSW_SIZE;
    private const byte FORG_DSW_OFFSET = FORG_COL_OFFSET + 15;
    private const byte BCKG_DSW_OFFSET = BCKG_COL_OFFSET + 15;
    private const byte FORG_ISW_OFFSET = FORG_COL_OFFSET + 14;
    private const byte BCKG_ISW_OFFSET = BCKG_COL_OFFSET + 14;
    
    private const byte TOTAL_SIZE = FORG_COL_SIZE + BCKG_COL_SIZE + STYLES_SIZE + FORG_CSW_SIZE + BCKG_CSW_SIZE;
    private const int UNUSED_SIZE = sizeof(ulong) * 8 - TOTAL_SIZE;

    private const ulong BMP_24 = 0x00_00_00_00_00_ff_ff_fful;
    private const ulong BMP_08 = 0x00_00_00_00_00_00_00_fful;
    
    [FieldOffset(0)] 
    private ulong _raw;

    /// <summary>
    /// The actual compressed style
    /// Bit 0 represents the most significant bit and bit 64 the least significant bit.
    /// </summary>
    public ulong Raw {
        readonly get => _raw;
        private set => _raw = value;
    }

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
        get => !IsFColorCustom && (_raw >> (64 - FORG_DSW_OFFSET - 1) & 1ul) == 0;
    }
    
    /// <summary>
    /// returns whether the background color is the default color
    /// </summary>
    public bool IsBColorDefault {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsBColorCustom && (_raw >> (64 - BCKG_DSW_OFFSET - 1) & 1ul) == 0;
    }
    
    /// <summary>
    /// returns whether the text color is a console color
    /// </summary>
    public bool IsFColorConsole {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsFColorDefault;
    }
    
    /// <summary>
    /// returns whether the background color is a console color
    /// </summary>
    public bool IsBColorConsole {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsBColorDefault;
    }
    
    /// <summary>
    /// returns whether the text color should be inherited
    /// </summary>
    public bool IsFColorInherit {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsFColorCustom && (_raw >> (64 - FORG_ISW_OFFSET - 1) & 1ul) == 1;
    }
    
    /// <summary>
    /// returns whether the background color should be inherited
    /// </summary>
    public bool IsBColorInherit {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsBColorCustom && (_raw >> (64 - BCKG_ISW_OFFSET - 1) & 1ul) == 1;
    }
    
    /// <summary>
    /// sets the color of the text to a custom color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFColor(uint color) =>
        _raw = _raw &  ~(BMP_24  << (64 - FORG_COL_OFFSET - FORG_COL_SIZE)) |
               ((color & BMP_24) << (64 - FORG_COL_OFFSET - FORG_COL_SIZE)) |
               (1ul << (64 - FORG_CSW_OFFSET - FORG_CSW_SIZE));

    /// <summary>
    /// sets the color of the text to a console color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFColor(NKConsoleColor color) => 
        _raw = _raw &         ~(BMP_24  << (64 - FORG_COL_OFFSET - FORG_COL_SIZE)) |
               (((ulong)color & BMP_24) << (64 - FORG_COL_OFFSET - FORG_COL_SIZE)) &
               ~(1ul << (64 - FORG_CSW_OFFSET - FORG_CSW_SIZE)) |
                (1ul << (64 - FORG_DSW_OFFSET - FORG_CSW_SIZE));

    /// <summary>
    /// sets the color to be the default color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFColor() => 
        _raw = _raw & ~(BMP_24 << (64 - FORG_COL_OFFSET - FORG_COL_SIZE)) &
               ~(1ul << (64 - FORG_CSW_OFFSET - FORG_CSW_SIZE));
    
    /// <summary>
    /// sets the color to be the default color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFColor(DefaultColor _) => SetFColor();
    
    /// <summary>
    /// sets the color to inherit mode
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFColor(InheritColor _) => 
        _raw = _raw & ~(BMP_24 << (64 - FORG_COL_OFFSET - FORG_COL_SIZE)) |
               (1ul  << (64 - FORG_ISW_OFFSET - FORG_CSW_SIZE)) &
               ~(1ul << (64 - FORG_CSW_OFFSET - FORG_CSW_SIZE));

    /// <summary>
    /// sets the color of the text to a color
    /// </summary>
    public NKStyle SetFColor(NKColor color) {
        if (color.IsRgb) 
            SetFColor(color.AsRgb);
        else if (color.IsPalette) 
            SetFColor(color.AsPalette);
        else if (color.IsInherit) 
            SetFColor(new InheritColor());
        else 
            SetFColor();
        
        return this;
    }
    
    /// <summary>
    /// safely sets the color of the text to a color
    /// </summary>
    public NKStyle SafeSetFColor(NKColor color) {
        if (color.IsRgb) {
            SetFColor(color.AsRgb);
        }
        else if (color.IsPalette) {
            SetFColor(color.AsPalette);
        }
        else if (color.IsInherit) {
            // nothing
        }
        else {
            SetFColor();
        }

        return this;
    }

    /// <summary>
    /// sets the background color to a custom color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBColor(uint color) => 
        _raw = _raw &  ~(BMP_24  << (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE)) |
               ((color & BMP_24) << (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE)) |
               (1ul << (64 - BCKG_CSW_OFFSET - BCKG_CSW_SIZE));

    /// <summary>
    /// sets the background color to a console color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBColor(NKConsoleColor color) => 
        _raw = _raw &         ~(BMP_24  << (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE)) |
               (((ulong)color & BMP_24) << (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE)) &
               ~(1ul << (64 - BCKG_CSW_OFFSET - BCKG_CSW_SIZE)) |
                (1ul << (64 - BCKG_DSW_OFFSET - BCKG_CSW_SIZE));
        
    /// <summary>
    /// sets the background color to the default color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBColor() => 
        _raw = _raw & ~(BMP_24 << (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE)) &
               ~(1ul << (64 - BCKG_CSW_OFFSET - BCKG_CSW_SIZE));

    /// <summary>
    /// sets the background color to the default color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBColor(DefaultColor _) => SetBColor();
    
    /// <summary>
    /// sets the background color to inherit mode
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBColor(InheritColor _) => 
        _raw = _raw & ~(BMP_24 << (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE)) |
               (1ul  << (64 - BCKG_ISW_OFFSET - BCKG_CSW_SIZE)) &
               ~(1ul << (64 - BCKG_CSW_OFFSET - BCKG_CSW_SIZE));
    
    /// <summary>
    /// sets the background color to a color
    /// </summary>
    public NKStyle SetBColor(NKColor color) {
        if (color.IsRgb)
            SetBColor(color.AsRgb);
        else if (color.IsPalette)
            SetBColor(color.AsPalette);
        else if (color.IsInherit)
            SetBColor(new InheritColor());
        else
            SetBColor();

        return this;
    }
    
    /// <summary>
    /// safely sets the background color to a color
    /// </summary>
    public NKStyle SafeSetBColor(NKColor color) {
        if (color.IsRgb) {
            SetBColor(color.AsRgb);
        }
        else if (color.IsPalette) {
            SetBColor(color.AsPalette);
        }
        else if (color.IsInherit) {
            // nothing
        }
        else {
            SetBColor();
        }

        return this;
    }

    /// <summary>
    /// returns the text color of the specified field
    /// </summary>
    public NKColor GetFColor() {
        if (IsFColorCustom)
            return new NKColor((int)(_raw >> (64 - FORG_COL_OFFSET - FORG_COL_SIZE) & BMP_24));
        if (IsFColorDefault)
            return NKColor.Default;
        if (IsFColorInherit)
            return NKColor.Inherit;

        return new NKColor((NKConsoleColor)(_raw >> (64 - FORG_COL_OFFSET - FORG_COL_SIZE) & BMP_08));
    }
    
    /// <summary>
    /// returns the text color of the specified field
    /// </summary>
    public NKColor GetBColor() {
        if (IsBColorCustom)
            return new NKColor((int)(_raw >> (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE) & BMP_24));
        if (IsBColorDefault)
            return NKColor.Default;
        if (IsBColorInherit)
            return NKColor.Inherit;

        return new NKColor((NKConsoleColor)(_raw >> (64 - BCKG_COL_OFFSET - BCKG_COL_SIZE) & BMP_08));
    }

    /// <summary>
    /// sets the text styles
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetStyles(TextStyles styles) =>
        _raw = _raw & ~(BMP_08 << (64 - STYLES_OFFSET - STYLES_SIZE)) |
               ((ulong)styles  << (64 - STYLES_OFFSET - STYLES_SIZE));

    /// <summary>
    /// returns the text styles
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextStyles GetStyles() => 
        (TextStyles)(_raw >> (64 - STYLES_OFFSET - STYLES_SIZE) & BMP_08);
    
    /// <summary>
    /// safely overwrites the contents of this instance with the contents of the other instance
    /// </summary>
    public NKStyle Override(NKStyle other) {
        if (!other.IsFColorInherit) SetFColor(other.GetFColor());
        if (!other.IsBColorInherit) SetBColor(other.GetBColor());
        SetStyles(other.GetStyles());

        return this;
    }
    
    internal NKStyle(ulong raw) => _raw = raw;

    public NKStyle(NKColor f, NKColor b, TextStyles s) {
        _raw = 0;
        SetFColor(f);
        SetBColor(b);
        SetStyles(s);
    }
    
    public NKStyle(NKColor f, NKColor b) {
        _raw = 0;
        SetFColor(f);
        SetBColor(b);
        SetStyles(TextStyles.NONE);
    }

    public NKStyle(NKColor f, TextStyles s) {
        _raw = 0;
        SetFColor(f);
        SetBColor(NKColor.Default);
        SetStyles(s);
    }

    public NKStyle(NKColor f) {
        _raw = 0;
        SetFColor(f);
        SetBColor(NKColor.Default);
        SetStyles(TextStyles.NONE);
    }

    public NKStyle(TextStyles s) {
        _raw = 0;
        SetFColor(NKColor.Default);
        SetBColor(NKColor.Default);
        SetStyles(s);
    }

    public NKStyle() {
        _raw = 0;
    }
    
    public object Clone() => MemberwiseClone();

    public override string ToString() => $"FColor: {FColor:p}, BColor: {BColor:p}{StylesToString()}";
    
    public string ToString(string format) => ToString(format, CultureInfo.InvariantCulture);
    
    public string ToString(string? format, IFormatProvider? formatProvider) {
        if (string.IsNullOrEmpty(format)) format = "s";
        return format switch {
            "p" or "P" => ToString(),
            _ => ToAnsi()
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToAnsi() => "".AddStyle(this);

    private string StylesToString() {
        var output = new List<string>();
        var styles = GetStyles();
        
        if (styles.IsBold)          output.Add("Bold");
        if (styles.IsItalic)        output.Add("Italic");
        if (styles.IsUnderline)     output.Add("Underline");
        if (styles.IsStrikethrough) output.Add("Strikethrough");
        if (styles.IsFaint)         output.Add("Faint");
        if (styles.IsNegative)      output.Add("Negative");

        return output.Count != 0 ? $", {string.Join(", ", output.ToArray())}" : "";
    }

    public bool Equals(NKStyle other) {
        return _raw == other._raw;
    }

    public override bool Equals(object? obj) {
        return obj is NKStyle other && Equals(other);
    }

    public override int GetHashCode() {
        return _raw.GetHashCode();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(NKStyle left, NKStyle right) => left.Equals(right);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(NKStyle left, NKStyle right) => !left.Equals(right);

    /// <summary>
    /// Overrides the properties of the first NKStyle instance with those of the second NKStyle instance and returns the updated instance.
    /// </summary>
    /// <param name="overriden">The NKStyle instance to be overridden.</param>
    /// <param name="overrider">The NKStyle instance providing the overriding properties.</param>
    /// <returns>The resulting NKStyle instance after applying the overrides.</returns>
    public static NKStyle operator <<(NKStyle overriden, NKStyle overrider) {
        overriden.Override(overrider);
        return overriden;
    }
    
    public static NKStyle Default => new(NKColor.Default, NKColor.Default, TextStyles.NONE);
}