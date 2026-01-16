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
        
        // TopLeft alignment: align point (0,0).
        // g1 at (0,0), g2 at (0,0).
        // Since g2 is drawn over g1 (implied by "copy second glyph" after "copy first glyph"),
        // result should have 'B' at (0,0) if they overlap?
        // Or size matches?
        // g1 width 1, g2 width 1.
        // align (0,0).
        // size: max((1,1), (1,1)) = (1,1). min(0,0).
        // Rect: 1-0+1 = 2.
        // Array size: 2-1 = 1x1.
        // Result: 1x1.
        // content: 'B' overwrites 'A'?
        
        Assert.Equal(1, compound.Width);
        Assert.Equal(1, compound.Height);
        Assert.Equal('B', compound.Glyph[0,0]);
    }

    [Fact]
    public void Constructor_ShouldCombineGlyphs_TopRight() {
        var g1 = CreateGlyph('A');
        var g2 = CreateGlyph('B');
        
        var compound = new NKCompoundGlyph(g1, g2, CompoundGlyphAlignment.TopRight());
        
        // TopRight alignment: align point (first.Width - second.Width, 0)
        // (1 - 1, 0) = (0, 0).
        // Same as TopLeft for equal sized glyphs.
        
        Assert.Equal(1, compound.Width);
        Assert.Equal(1, compound.Height);
        Assert.Equal('B', compound.Glyph[0,0]);
    }
    
    [Fact]
    public void Constructor_ShouldExtendCanvas_WhenGlyphIsShifted() {
        // Create 2x2 glyph 'A'
        var g1 = new NKComponentGlyph(new char?[,] { { 'A', 'A' }, { 'A', 'A' } }); // 2x2
        var g2 = CreateGlyph('B'); // 1x1
        
        // Center: (first.Width/2 - second.Width/2, ...)
        // (2/2 - 1/2, ...) = (1 - 0, ...) = (1, ...)
        // So B starts at x=1.
        
        var compound = new NKCompoundGlyph(g1, g2, CompoundGlyphAlignment.Center());
        
        // Size should be 2x2 (B fits inside).
        Assert.Equal(2, compound.Width);
        Assert.Equal(2, compound.Height);
        
        // A at (0,0), (0,1), (1,0), (1,1).
        // B at (1, 1). (Center of 2x2 is (1,1)? 
        // y align: 2/2 - 1/2 = 1.
        // So B at (1,1).
        
        Assert.Equal('A', compound.Glyph[0,0]);
        Assert.Equal('A', compound.Glyph[0,1]);
        Assert.Equal('A', compound.Glyph[1,0]);
        Assert.Equal('B', compound.Glyph[1,1]); // B overwrites A at 1,1
    }
}
