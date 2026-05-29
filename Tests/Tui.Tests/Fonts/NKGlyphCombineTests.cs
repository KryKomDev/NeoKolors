// NeoKolors
// Copyright (c) krystof 2026

using NeoKolors.Tui.Fonts;
using Drct = NeoKolors.Tui.Fonts.Serialization.Xml.V3.XmlGlyphAlignDirection;

namespace NeoKolors.Tui.Tests.Fonts;

public class NKGlyphCombineTests {
    private NKGlyph CreateGlyph(char c) {
        var glyph = new GlyphCell[1, 1];
        glyph[0, 0] = GlyphCell.Char(c);
        return new NKGlyph(glyph, 0);
    }

    [Fact]
    public void Combine_Right_ShouldNotOverlap() {
        var g1 = CreateGlyph('A');
        var g2 = CreateGlyph('B');

        var combined = NKGlyph.Combine(g1, g2, Drct.RIGHT);

        Assert.Equal(2, combined.Width);
        Assert.Equal(1, combined.Height);
        Assert.Equal('A', combined.Glyph[0, 0].Character);
        Assert.Equal('B', combined.Glyph[1, 0].Character);
    }

    [Fact]
    public void Combine_Top_ShouldNotOverlap() {
        var g1 = CreateGlyph('A');
        var g2 = CreateGlyph('B');

        var combined = NKGlyph.Combine(g1, g2, Drct.TOP);

        Assert.Equal(1, combined.Width);
        Assert.Equal(2, combined.Height);
        Assert.Equal('B', combined.Glyph[0, 0].Character);
        Assert.Equal('A', combined.Glyph[0, 1].Character);
    }

    [Fact]
    public void Combine_BottomRight_ShouldNotOverlap() {
        var g1 = CreateGlyph('A');
        var g2 = CreateGlyph('B');

        var combined = NKGlyph.Combine(g1, g2, Drct.BOTTOM_RIGHT);

        // A at (0,0)
        // B at (1,1)
        // Size 2x2
        Assert.Equal(2, combined.Width);
        Assert.Equal(2, combined.Height);
        Assert.Equal('A', combined.Glyph[0, 0].Character);
        Assert.Equal('B', combined.Glyph[1, 1].Character);
    }
}