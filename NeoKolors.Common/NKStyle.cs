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
    
    [FieldOffset(0)] 
    private ulong _raw;

    /// <summary>
    /// The actual compressed style
    /// Bit 0 represents the most significant bit and bit 63 the least significant bit.<br/>
    /// <b>Meaning</b>:
    /// <ul>
    ///     <li>bit 00-23: foreground color</li>
    ///     <li>bit 24-47: background color</li>
    ///     <li>bit 56-61: text style bitmap (strikethrough, negative, faint, underline, italic and bold respectively)</li>
    ///     <li>bit 62: toggles terminal color palette mode for text (uses custom color if 1, palette colors else)  </li>
    ///     <li>bit 63: toggles terminal color palette mode for a background (uses custom color if 1, palette colors else)</li>
    /// </ul> 
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
        get => (_raw & 0b10) == 0b10;
    }

    /// <summary>
    /// returns whether the background color is in palette mode or custom mode
    /// </summary>
    public bool IsBColorCustom {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_raw & 0b01) == 0b01;
    }

    /// <summary>
    /// returns whether the text color is the default color
    /// </summary>
    public bool IsFColorDefault {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsFColorCustom && (_raw & 0x00_01_00_00_00_00_00_00ul) == 0x00_01_00_00_00_00_00_00ul;
    }
    
    /// <summary>
    /// returns whether the background color is the default color
    /// </summary>
    public bool IsBColorDefault {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsBColorCustom && (_raw & 0x00_00_00_00_01_00_00_00ul) == 0x00_00_00_00_01_00_00_00ul;
    }
    
    /// <summary>
    /// returns whether the text color should be inherited
    /// </summary>
    public bool IsFColorInherit {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsFColorCustom && (_raw & 0x00_02_00_00_00_00_00_00ul) == 0x00_02_00_00_00_00_00_00ul;
    }
    
    /// <summary>
    /// returns whether the background color should be inherited
    /// </summary>
    public bool IsBColorInherit {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsBColorCustom && (_raw & 0x00_00_00_00_02_00_00_00ul) == 0x00_00_00_00_02_00_00_00ul;
    }
    
    
    /// <summary>
    /// sets the color of the text to a custom color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFColor(UInt24 color) => 
        _raw = _raw & 0x00_00_00_ff_ff_ff_ff_fdul | ((ulong)color << 40) | 0b10;

    /// <summary>
    /// sets the color of the text to a console color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFColor(NKConsoleColor color) => 
        _raw = _raw & 0x00_00_00_ff_ff_ff_ff_fdul | ((ulong)color << 40);

    /// <summary>
    /// sets the color to be the default color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFColor() => 
        _raw = _raw & 0x00_00_00_ff_ff_ff_ff_fdul | 0x00_01_00_00_00_00_00_00ul;
    
    /// <summary>
    /// sets the color to be the default color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFColor(DefaultColor _) => 
        _raw = _raw & 0x00_00_00_ff_ff_ff_ff_fdul | 0x00_01_00_00_00_00_00_00ul;
    
    /// <summary>
    /// sets the color to inherit mode
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFColor(InheritColor _) => 
        _raw = _raw & 0x00_00_00_ff_ff_ff_ff_fdul | 0x00_02_00_00_00_00_00_00ul;

    /// <summary>
    /// sets the color of the text to a color
    /// </summary>
    public void SetFColor(NKColor color) {
        if (color.Color.IsT0) 
            SetFColor((UInt24)color.Color.AsT0);
        else if (color.Color.IsT1) 
            SetFColor(color.Color.AsT1);
        else if (color.Color.IsT3) 
            SetFColor(new InheritColor());
        else 
            SetFColor();
    }
    
    /// <summary>
    /// safely sets the color of the text to a color
    /// </summary>
    public void SafeSetFColor(NKColor color) {
        if (color.Color.IsT0) {
            SetFColor((UInt24)color.Color.AsT0);
        }
        else if (color.Color.IsT1) {
            SetFColor(color.Color.AsT1);
        }
        else if (color.Color.IsT3) {
            // nothing
        }
        else {
            SetFColor();
        }
    }

    /// <summary>
    /// sets the background color to a custom color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBColor(UInt24 color) => 
        _raw = _raw & 0xff_ff_ff_00_00_00_ff_feul | ((ulong)color << 16) | 0b01;

    /// <summary>
    /// sets the background color to a console color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBColor(NKConsoleColor color) => 
        _raw = _raw & 0xff_ff_ff_00_00_00_ff_feul | ((ulong)color << 16);

    /// <summary>
    /// sets the background color to the default color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBColor() => 
        _raw = _raw & 0xff_ff_ff_00_00_00_ff_feul | 0x00_00_00_00_01_00_00_00ul;

    /// <summary>
    /// sets the background color to the default color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBColor(DefaultColor _) => 
        _raw = _raw & 0xff_ff_ff_00_00_00_ff_feul | 0x00_00_00_00_01_00_00_00ul;
    
    /// <summary>
    /// sets the background color to inherit mode
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBColor(InheritColor _) => 
        _raw = _raw & 0xff_ff_ff_00_00_00_ff_feul | 0x00_00_00_00_02_00_00_00ul;
    
    /// <summary>
    /// sets the background color to a color
    /// </summary>
    public void SetBColor(NKColor color) {
        if (color.Color.IsT0)
            SetBColor((UInt24)color.Color.AsT0);
        else if (color.Color.IsT1)
            SetBColor(color.Color.AsT1);
        else if (color.Color.IsT3)
            SetBColor(new InheritColor());
        else
            SetBColor();
    }
    
    /// <summary>
    /// safely sets the background color to a color
    /// </summary>
    public void SafeSetBColor(NKColor color) {
        if (color.Color.IsT0) {
            SetBColor((UInt24)color.Color.AsT0);
        }
        else if (color.Color.IsT1) {
            SetBColor(color.Color.AsT1);
        }
        else if (color.Color.IsT3) {
            // nothing
        }
        else {
            SetBColor();
        }
    }

    /// <summary>
    /// returns the text color of the specified field
    /// </summary>
    public NKColor GetFColor() {
        if (IsFColorCustom)
            return new NKColor((int)(_raw >> 40));
        if (IsFColorDefault)
            return NKColor.Default;
        if (IsFColorInherit)
            return NKColor.Inherit;

        return new NKColor((NKConsoleColor)((_raw & 0x00_00_ff_00_00_00_00_00ul) >> 40));
    }
    
    /// <summary>
    /// returns the text color of the specified field
    /// </summary>
    public NKColor GetBColor() {
        if (IsBColorCustom)
            return new NKColor((int)(_raw >> 16));
        if (IsBColorDefault)
            return NKColor.Default;
        if (IsBColorInherit)
            return NKColor.Inherit;

        return new NKColor((NKConsoleColor)((_raw & 0x00_00_00_00_00_ff_00_00ul) >> 16));
    }

    /// <summary>
    /// sets the text styles
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetStyles(TextStyles styles) => _raw |= 0x00_00_00_00_00_00_00_fcul & (ulong)((int)styles << 2);

    /// <summary>
    /// returns the text styles
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextStyles GetStyles() => (TextStyles)((_raw & 0x00_00_00_00_00_00_00fcul) >> 2);
    
    /// <summary>
    /// safely overwrites the contents of this instance with the contents of the other instance
    /// </summary>
    public void Override(NKStyle other) {
        if (!other.IsFColorInherit) SetFColor(other.GetFColor());
        if (!other.IsBColorInherit) SetBColor(other.GetBColor());
        SetStyles(other.GetStyles());
    }
    
    public NKStyle(ulong raw) => _raw = raw;

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
        SetFColor(NKColor.Default);
        SetBColor(NKColor.Default);
        SetStyles(TextStyles.NONE);
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
        
        if (styles.HasFlag(TextStyles.BOLD)) output.Add("Bold");
        if (styles.HasFlag(TextStyles.ITALIC)) output.Add("Italic");
        if (styles.HasFlag(TextStyles.UNDERLINE)) output.Add("Underline");
        if (styles.HasFlag(TextStyles.STRIKETHROUGH)) output.Add("Strikethrough");
        if (styles.HasFlag(TextStyles.FAINT)) output.Add("Faint");
        if (styles.HasFlag(TextStyles.NEGATIVE)) output.Add("Negative");

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
    
    public static NKStyle Default => new(NKColor.Default, NKColor.Inherit, TextStyles.NONE);
}