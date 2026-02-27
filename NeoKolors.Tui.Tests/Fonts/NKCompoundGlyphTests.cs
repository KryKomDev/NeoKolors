//
// NeoKolors.Test
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Fonts;

namespace NeoKolors.Tui.Tests;

public class NKCompoundGlyphTests {

    private NKComponentGlyph CreateGlyph(char c) {
        return new NKComponentGlyph(new char?[,] { { c } });
    }

    [Fact]
    public void Constructor_ShouldCombineGlyphs_TopLeft() {
        var g1 = CreateGlyph('A');
        var g2 = CreateGlyph('B');
        
        var compound = new NKCompoundGlyph(g1, g2, CompoundGlyphAlignment.TopLeft());
        
        // TopLeft alignment: stacks g2 ABOVE g1.
        // g1 width 1, g2 width 1.
        // align (0,-1).
        // size: width 1, height 2.
        
        Assert.Equal(1, compound.Width);
        Assert.Equal(2, compound.Height);
        Assert.Equal('B', compound.Glyph[0,0]);
        Assert.Equal('A', compound.Glyph[0,1]);
    }

    [Fact]
    public void Constructor_ShouldCombineGlyphs_TopRight() {
        var g1 = CreateGlyph('A');
        var g2 = CreateGlyph('B');
        
        var compound = new NKCompoundGlyph(g1, g2, CompoundGlyphAlignment.TopRight());
        
        // TopRight alignment: stacks g2 ABOVE g1.
        
        Assert.Equal(1, compound.Width);
        Assert.Equal(2, compound.Height);
        Assert.Equal('B', compound.Glyph[0,0]);
        Assert.Equal('A', compound.Glyph[0,1]);
    }
    
    [Fact]
    public void Constructor_ShouldExtendCanvas_WhenGlyphIsShifted() {
        // Create 2x2 glyph 'A'
        var g1 = new NKComponentGlyph(new char?[,] { { 'A', 'A' }, { 'A', 'A' } }); // 2x2
        var g2 = CreateGlyph('B'); // 1x1
        
        // CENTER alignment: (first.Width/2 - second.Width/2, first.Height/2 - second.Height/2)
        // (2/2 - 1/2, 2/2 - 1/2) = (1, 1)
        // So B starts at x=1, y=1.
        
        var compound = new NKCompoundGlyph(g1, g2, CompoundGlyphAlignment.Center());
        
        // Size should be 2x2 (B fits inside).
        Assert.Equal(2, compound.Width);
        Assert.Equal(2, compound.Height);
        
        // A at (0,0), (0,1), (1,0), (1,1).
        // B at (1, 1).
        
        Assert.Equal('A', compound.Glyph[0,0]);
        Assert.Equal('A', compound.Glyph[0,1]);
        Assert.Equal('A', compound.Glyph[1,0]);
        Assert.Equal('B', compound.Glyph[1,1]); // B overwrites A at 1,1
    }
}
