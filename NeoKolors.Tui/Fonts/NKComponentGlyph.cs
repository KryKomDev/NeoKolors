// NeoKolors
// Copyright (c) 2025 KryKom

using Metriks;

namespace NeoKolors.Tui.Fonts;

public class NKComponentGlyph : IGlyph {
    
    public char?[,] Glyph { get; }
    public int Width => Glyph.Len0;
    public int Height => Glyph.Len1;
    public int BaselineOffset { get; }
    public GlyphAlignmentPointCollection AlignPoints { get; }

    public NKComponentGlyph(
        char?[,] glyph,
        int baselineOffset = 0,
        GlyphAlignmentPointCollection? glyphAlignmentPointCollection = null) 
    {
        Glyph = glyph;
        BaselineOffset = baselineOffset;
        AlignPoints = glyphAlignmentPointCollection ?? new GlyphAlignmentPointCollection();
    }
}