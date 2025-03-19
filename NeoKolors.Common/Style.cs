//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Common;

/// <summary>
/// contains information about console styles (bg / fg color, bold, italic etc.)
/// </summary>
public struct Style : ICloneable {
    
    /// <summary>
    /// The actual compressed style
    /// Bit 0 represents the most significant bit and bit 63 the least significant bit.<br/>
    /// <b>meaning</b>:
    /// <ul>
    ///     <li>bit 00 - 23: custom foreground color in RGB where R is byte 0, </li>
    ///     <li>bit 14 - 47: custom background color in RGB where R is byte 3, </li>
    ///     <li>bit 48 - 51: palette foreground color, </li>
    ///     <li>bit 52 - 55: palette background color, </li>
    ///     <li>bit 56 - 61: text style bitmap (strikethrough, negative, faint, underline, italic and bold respectively)</li>
    ///     <li>bit 62: toggles terminal color palette mode for text (uses custom color if 1, palette colors else)  </li>
    ///     <li>bit 63: toggles terminal color palette mode for background (uses custom color if 1, palette colors else)</li>
    /// </ul> 
    /// </summary>
    public ulong Raw { get; private set; }

    /// <summary>
    /// represents the color of the text
    /// </summary>
    public Color FColor {
        get => GetFColor();
        set => SetFColor(value);
    }

    /// <summary>
    /// represents the background color
    /// </summary>
    public Color BColor {
        get => GetBColor();
        set => SetBColor(value);
    }

    /// <summary>
    /// bitmap of the individual text styles (see <see cref="TextStyles"/>)
    /// </summary>
    /// <seealso cref="TextStyles"/>
    public int Styles {
        get => GetStyles();
        set => SetStyles(value);
    }
    
    /// <summary>
    /// returns whether the text color is in palette mode or custom mode
    /// </summary>
    public bool IsFColorCustom => (Raw & 0b10) == 0b10;

    /// <summary>
    /// returns whether the background color is in palette mode or custom mode
    /// </summary>
    public bool IsBColorCustom => (Raw & 0b01) == 0b01;
    
    /// <summary>
    /// sets the color of text to a custom color
    /// </summary>
    public void SetFColor(UInt24 color) {
        Raw = Raw & 0x00_00_00_ff_ff_ff_ff_fdul | ((ulong)color << 40) | 0b10;
    }

    /// <summary>
    /// sets the color of text to a console color
    /// </summary>
    public void SetFColor(ConsoleColor color) {
        Raw = Raw & 0xff_ff_ff_ff_ff_ff_f0_fdul | ((ulong)color << 12) | 0b00;
    }

    /// <summary>
    /// sets the color of text to a color
    /// </summary>
    public void SetFColor(Color color) {
        if (color.IsPaletteSafe)
            SetFColor((ConsoleColor)color.ConsoleColor!);
        else
            SetFColor((UInt24)color.CustomColor!);
    }

    /// <summary>
    /// sets the background color to a custom color
    /// </summary>
    public void SetBColor(UInt24 color) {
        Raw = Raw & 0xff_ff_ff_00_00_00_ff_feul | ((ulong)color << 16) | 0b01;
    }

    /// <summary>
    /// sets the background color to a console color
    /// </summary>
    /// <param name="color"></param>
    public void SetBColor(ConsoleColor color) {
        Raw = Raw & 0xff_ff_ff_ff_ff_ff_f0_feul | ((ulong)color << 8) & 0b00;
    }

    /// <summary>
    /// sets the background color to a color
    /// </summary>
    public void SetBColor(Color color) {
        if (color.IsPaletteSafe)
            SetBColor((ConsoleColor)color.ConsoleColor!);
        else
            SetBColor((UInt24)color.CustomColor!);
    }

    /// <summary>
    /// returns the text color of the specified field
    /// </summary>
    public Color GetFColor() {
        if (IsFColorCustom) {
            UInt24 color = (int)(Raw >> 40);
            return new Color(color);
        }
        else {
            int color = (int)((Raw & 0x00_00_00_00_00_00_f0_00ul) >> 12);
            return new Color((ConsoleColor)color);
        }
    }
    
    /// <summary>
    /// returns the text color of the specified field
    /// </summary>
    public Color GetBColor() {
        if (IsBColorCustom) {
            UInt24 color = (int)(Raw >> 16);
            return new Color(color);
        }
        else {
            int color = (int)((Raw & 0x00_00_00_00_00_00_0f_00ul) >> 8);
            return new Color((ConsoleColor)color);
        }
    }

    /// <summary>
    /// sets the text styles
    /// </summary>
    public void SetStyles(int styles) => Raw |= 0x00_00_00_00_00_00_00_fcul & (ulong)(styles << 2);

    /// <summary>
    /// returns the text styles
    /// </summary>
    public int GetStyles() => (int)((Raw & 0x00_00_00_00_00_00_00fcul) >> 2);

    public Style(ulong raw) => Raw = raw;

    public Style(Color f, Color b, int s) {
        Raw = 0;
        SetFColor(f);
        SetBColor(b);
        SetStyles(s);
    }
    
    public object Clone() => MemberwiseClone();
}