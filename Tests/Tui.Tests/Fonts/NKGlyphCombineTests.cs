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

    [Fact]
    public void Combine_Bottom_BaselineShouldBeMainBaseline() {
        var g1 = CreateGlyph('A'); // size 1x1, baseline 0
        var g2 = CreateGlyph('B'); // size 1x1, baseline 0

        var combined = NKGlyph.Combine(g1, g2, Drct.BOTTOM);

        // A should be at (0,0), B at (0,1)
        // The main glyph is A, which remains at y=0. Its bottom is 1 unit above the bottom of the combined glyph,
        // so the combined glyph's bottom sits 1 unit below the baseline, making BaselineOffset = -1.
        Assert.Equal(-1, combined.BaselineOffset);
    }

    [Fact]
    public void Combine_CharAlign_BaselineShouldBeMainBaseline() {
        var g1Glyph = new GlyphCell[1, 2];
        g1Glyph[0, 0] = GlyphCell.Char('A');
        g1Glyph[0, 1] = GlyphCell.Char('B');
        var g1 = new NKGlyph(g1Glyph, 1, new[] { new AlignPoint('x', new Metriks.Point2D(0, 0)) });

        var g2Glyph = new GlyphCell[1, 1];
        g2Glyph[0, 0] = GlyphCell.Char('C');
        var g2 = new NKGlyph(g2Glyph, 0, new[] { new AlignPoint('x', new Metriks.Point2D(0, 0)) });

        var combined = NKGlyph.Combine(g1, g2, 'x');

        Assert.NotNull(combined);
        // The main glyph 'g1' is at y=0, so baseline should be main's baseline (1).
        Assert.Equal(1, combined.BaselineOffset);
    }
}