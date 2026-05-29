// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Tui.Fonts;

public record FontSpacingInfo {
    
    public bool Ligatures     { get; init; }
    public int  Leading       { get; init; }
    public int  LetterSpacing { get; init; }
    public int  WordSpacing   { get; init; }

    public FontSpacingInfo(
        bool           ligatures     = false, 
        int            leading       = 0,
        int            letterSpacing = 0,
        int            wordSpacing   = 0) 
    {
        Ligatures     = ligatures;
        Leading       = leading;
        LetterSpacing = letterSpacing;
        WordSpacing   = wordSpacing;
    }
}