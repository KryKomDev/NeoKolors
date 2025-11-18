// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Fonts;

public readonly struct NKFontInfo : IFontInfo {
    
    /// <summary>
    /// Gets the name of the font associated with this NKFontInfo instance.
    /// This property provides the identifier or descriptive name that categorizes the font.
    /// It is a read-only property initialized during the construction of an NKFontInfo object.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Provides detailed information about the spacing characteristics of the font.
    /// This property encapsulates either fixed dimensions for monospaced fonts
    /// or variable spacing details for proportional fonts by leveraging the
    /// FontSpacingInfo abstraction. It allows distinguishing between monospaced and variable fonts
    /// and accessing their respective spacing configurations.
    /// </summary>
    public FontSpacingInfo SpacingInfo { get; }

    /// <summary>
    /// Indicates whether the font associated with this NKFontInfo instance supports ligatures.
    /// Ligatures are special character combinations that improve visual aesthetics
    /// by replacing specific sequences of characters with a single glyph.
    /// This property is read-only and is determined during the initialization of the NKFontInfo instance.
    /// </summary>
    public bool Ligatures { get; }

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


    public NKFontInfo(
        string name,
        FontSpacingInfo spacing,
        bool ligatures,
        int charSpacing,
        int lineSpacing) 
    {
        Name            = name;
        SpacingInfo     = spacing;
        Ligatures       = ligatures;
        CharSpacing     = charSpacing;
        LineSpacing     = lineSpacing;    
    }
}