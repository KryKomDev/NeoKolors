//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Runtime.CompilerServices;

namespace NeoKolors.Common;

/// <summary>
/// contains information about console styles (bg / fg color, bold, italic etc.)
/// </summary>
public struct NKStyle : ICloneable, IEquatable<NKStyle> {
    
    /// <summary>
    /// The actual compressed style
    /// Bit 0 represents the most significant bit and bit 63 the least significant bit.<br/>
    /// <b>meaning</b>:
    /// <ul>
    ///     <li>bit 00 - 23: foreground color</li>
    ///     <li>bit 24 - 47: background color</li>
    ///     <li>bit 56 - 61: text style bitmap (strikethrough, negative, faint, underline, italic and bold respectively)</li>
    ///     <li>bit 62: toggles terminal color palette mode for text (uses custom color if 1, palette colors else)  </li>
    ///     <li>bit 63: toggles terminal color palette mode for background (uses custom color if 1, palette colors else)</li>
    /// </ul> 
    /// </summary>
    public ulong Raw { get; private set; }

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
        get => (Raw & 0b10) == 0b10;
    }

    /// <summary>
    /// returns whether the background color is in palette mode or custom mode
    /// </summary>
    public bool IsBColorCustom {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (Raw & 0b01) == 0b01;
    }

    /// <summary>
    /// returns whether the text color is the default color
    /// </summary>
    public bool IsFColorDefault {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsFColorCustom && (Raw & 0x00_01_00_00_00_00_00_00ul) == 0x00_01_00_00_00_00_00_00ul;
    }
    
    /// <summary>
    /// returns whether the background color is the default color
    /// </summary>
    public bool IsBColorDefault {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsBColorCustom && (Raw & 0x00_00_00_00_01_00_00_00ul) == 0x00_00_00_00_01_00_00_00ul;
    }
    
    
    /// <summary>
    /// sets the color of text to a custom color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFColor(UInt24 color) => 
        Raw = Raw & 0x00_00_00_ff_ff_ff_ff_fdul | ((ulong)color << 40) | 0b10;

    /// <summary>
    /// sets the color of text to a console color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFColor(NKConsoleColor color) => 
        Raw = Raw & 0x00_00_00_ff_ff_ff_ff_fdul | ((ulong)color << 40);

    /// <summary>
    /// sets the color to be the default color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFColor() => 
        Raw = Raw & 0x00_00_00_ff_ff_ff_ff_fdul | 0x00_01_00_00_00_00_00_00ul;

    /// <summary>
    /// sets the color of text to a color
    /// </summary>
    public void SetFColor(NKColor color) {
        if (color.Color.IsT0)
            SetFColor((UInt24)color.Color.AsT0);
        else if (color.Color.IsT1)
            SetFColor(color.Color.AsT1);
        else
            SetFColor();
    }

    /// <summary>
    /// sets the background color to a custom color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBColor(UInt24 color) => 
        Raw = Raw & 0xff_ff_ff_00_00_00_ff_feul | ((ulong)color << 16) | 0b01;

    /// <summary>
    /// sets the background color to a console color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBColor(NKConsoleColor color) => 
        Raw = Raw & 0xff_ff_ff_00_00_00_ff_feul | ((ulong)color << 16);

    /// <summary>
    /// sets the background color to the default color
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBColor() => 
        Raw = Raw & 0xff_ff_ff_00_00_00_ff_feul | 0x00_00_00_00_01_00_00_00ul;

    /// <summary>
    /// sets the background color to a color
    /// </summary>
    public void SetBColor(NKColor color) {
        if (color.Color.IsT0)
            SetBColor((UInt24)color.Color.AsT0);
        else if (color.Color.IsT1)
            SetBColor(color.Color.AsT1);
        else
            SetBColor();
    }

    /// <summary>
    /// returns the text color of the specified field
    /// </summary>
    public NKColor GetFColor() {
        if (IsFColorCustom)
            return new NKColor((int)(Raw >> 40));
        if (IsFColorDefault)
            return new NKColor();

        return new NKColor((NKConsoleColor)((Raw & 0x00_00_ff_00_00_00_00_00ul) >> 40));
    }
    
    /// <summary>
    /// returns the text color of the specified field
    /// </summary>
    public NKColor GetBColor() {
        if (IsBColorCustom)
            return new NKColor((int)(Raw >> 16));
        if (IsBColorDefault)
            return new NKColor();

        return new NKColor((NKConsoleColor)((Raw & 0x00_00_00_00_00_ff_00_00ul) >> 16));
    }

    /// <summary>
    /// sets the text styles
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetStyles(TextStyles styles) => Raw |= 0x00_00_00_00_00_00_00_fcul & (ulong)((int)styles << 2);

    /// <summary>
    /// returns the text styles
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextStyles GetStyles() => (TextStyles)((Raw & 0x00_00_00_00_00_00_00fcul) >> 2);

    public NKStyle(ulong raw) => Raw = raw;

    public NKStyle(NKColor f, NKColor b, TextStyles s) {
        Raw = 0;
        SetFColor(f);
        SetBColor(b);
        SetStyles(s);
    }

    public NKStyle() {
        FColor = new NKColor();
        BColor = new NKColor();
        Styles = 0;
    }
    
    public object Clone() => MemberwiseClone();

    public override string ToString() => $"FColor: {FColor}, BColor: {BColor}{StylesToString()}";

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
        return Raw == other.Raw;
    }

    public override bool Equals(object? obj) {
        return obj is NKStyle other && Equals(other);
    }

    public override int GetHashCode() {
        return Raw.GetHashCode();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(NKStyle left, NKStyle right) => Equals(left, right);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(NKStyle left, NKStyle right) => !(left == right);
}