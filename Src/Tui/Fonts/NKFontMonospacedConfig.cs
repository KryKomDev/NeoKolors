// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Tui.Fonts;

public readonly record struct NKFontMonospacedConfig {
    public int  GlyphWidth  { get; init; }
    public int  GlyphHeight { get; init; }
    public bool AlignToGrid { get; init; }

    public NKFontMonospacedConfig(int glyphWidth, int glyphHeight, bool alignToGrid) {
        GlyphWidth  = glyphWidth;
        GlyphHeight = glyphHeight;
        AlignToGrid = alignToGrid;
    }
}