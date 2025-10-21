// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Fonts.V2;

public sealed class GlyphInfo : OneOfBase<SimpleGlyphInfo, AutoCompoundGlyphInfo, LigatureGlyphInfo> {
    public GlyphInfo(OneOf<SimpleGlyphInfo, AutoCompoundGlyphInfo, LigatureGlyphInfo> input) 
        : base(input) { }
    
    public static implicit operator GlyphInfo(SimpleGlyphInfo info) => new(info);
    public static implicit operator GlyphInfo(AutoCompoundGlyphInfo info) => new(info);
    public static implicit operator GlyphInfo(LigatureGlyphInfo info) => new(info);
    
    public IGlyph Glyph => 
        Match(
            s => s.Glyph, 
            _ => throw new InvalidOperationException("Cannot get glyph from auto-compound glyph."), 
            l => l.Glyph
        );
}