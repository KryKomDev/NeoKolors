// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Fonts.V2;

public readonly struct NKFontInfo {
    
    /// <summary>
    /// Gets the name of the font associated with this NKFontInfo instance.
    /// This property provides the identifier or descriptive name that categorizes the font.
    /// It is a read-only property initialized during the construction of an NKFontInfo object.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the detailed information associated with the monospace characteristics of a font.
    /// This property indicates whether the font is monospace and, if so, provides the fixed
    /// character width and height for the font. If the font is not monospace, these values
    /// are unavailable, and an attempt to access them will result in an exception.
    /// </summary>
    public MonospaceInfo MonospaceInfo { get; }

    /// <summary>
    /// Indicates whether the font associated with this NKFontInfo instance supports ligatures.
    /// Ligatures are special character combinations that improve visual aesthetics
    /// by replacing specific sequences of characters with a single glyph.
    /// This property is read-only and is determined during the initialization of the NKFontInfo instance.
    /// </summary>
    public bool Ligatures { get; }

    /// <summary>
    /// Indicates whether kerning is enabled for the font represented by this NKFontInfo instance.
    /// Kerning adjusts the spacing between specific pairs of characters to improve visual text appearance
    /// and create a more visually pleasing result in typeset text.
    /// This is a read-only property initialized when the NKFontInfo instance is constructed.
    /// </summary>
    public bool Kerning { get; }

    /// <summary>
    /// Gets the spacing value applied between characters for the font associated with this NKFontInfo instance.
    /// This property determines the amount of space inserted between adjacent characters in the font.
    /// It is a read-only property initialized during the construction of an NKFontInfo object.
    /// </summary>
    public int CharSpacing { get; }

    /// <summary>
    /// Gets the line spacing value associated with this NKFontInfo instance.
    /// This property specifies the vertical space between lines of text, measured in pixels.
    /// It is a read-only property initialized during the construction of an NKFontInfo object.
    /// </summary>
    public int LineSpacing { get; }

    /// <summary>
    /// Gets the amount of spacing inserted between words in the font associated with this NKFontInfo instance.
    /// This property represents the horizontal space applied between consecutive words when rendering text,
    /// providing a measure of layout flexibility for text styling.
    /// It is a read-only property initialized during the construction of an NKFontInfo object.
    /// </summary>
    public int WordSpacing { get; }

    public NKFontInfo(
        string name,
        MonospaceInfo monospaceInfo,
        bool ligatures,
        bool kerning,
        int charSpacing,
        int lineSpacing,
        int wordSpacing) 
    {
        Name          = name;
        MonospaceInfo = monospaceInfo;
        Ligatures     = ligatures;
        Kerning       = kerning;
        CharSpacing   = charSpacing;
        LineSpacing   = lineSpacing;
        WordSpacing   = wordSpacing;
    }
}