// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Fonts;

public interface ICompoundGlyph : IGlyph {
    
    public IGlyph MainGlyph { get; }
    public IGlyph SecondaryGlyph { get; }
    
    public CompoundGlyphAlignment Alignment { get; }
}