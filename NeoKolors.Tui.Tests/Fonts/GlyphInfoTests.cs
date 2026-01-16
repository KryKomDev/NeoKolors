//
// NeoKolors.Test
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Fonts;

namespace NeoKolors.Tui.Tests;

public class GlyphInfoTests {
    
    // Mock implementation of IGlyph for testing
    private class MockGlyph : IGlyph {
        public int Width { get; set; }
        public int Height { get; set; }
        public char?[,] Glyph { get; set; } = new char?[0,0];
        public int BaselineOffset { get; set; }
        public GlyphAlignmentPointCollection AlignPoints { get; set; } = new();
        
        public bool Equals(IGlyph? other) {
            return other is MockGlyph mg && Width == mg.Width && Height == mg.Height;
        }
    }

    [Fact]
    public void SimpleGlyphInfo_Equality_ShouldWork() {
        var glyph = new MockGlyph { Width = 10, Height = 10 };
        var info1 = new SimpleGlyphInfo(glyph, 'A');
        var info2 = new SimpleGlyphInfo(glyph, 'A');
        var info3 = new SimpleGlyphInfo(glyph, 'B');
        
        Assert.Equal(info1, info2);
        Assert.NotEqual(info1, info3);
        Assert.True(info1 == info2);
        Assert.True(info1 != info3);
    }
    
    [Fact]
    public void LigatureGlyphInfo_Equality_ShouldWork() {
        var glyph = new MockGlyph { Width = 10, Height = 10 };
        var info1 = new LigatureGlyphInfo(glyph, "==");
        var info2 = new LigatureGlyphInfo(glyph, "==");
        var info3 = new LigatureGlyphInfo(glyph, "!=");
        
        Assert.Equal(info1, info2);
        Assert.NotEqual(info1, info3);
    }

    [Fact]
    public void AutoCompoundGlyphInfo_Equality_ShouldWork() {
        var glyph = new MockGlyph { Width = 10, Height = 10 };
        var applicable = new ApplicableChars([]);
        var info1 = new AutoCompoundGlyphInfo(glyph, 'A', CompoundGlyphAlignment.Center(), applicable);
        var info2 = new AutoCompoundGlyphInfo(glyph, 'A', CompoundGlyphAlignment.Center(), applicable);
        var info3 = new AutoCompoundGlyphInfo(glyph, 'B', CompoundGlyphAlignment.Center(), applicable);
        
        Assert.Equal(info1, info2);
        Assert.NotEqual(info1, info3);
    }

    [Fact]
    public void ApplicableChars_ShouldIdentifyApplicableCharacters() {
        var appChars = new ApplicableChars(['x', 'y']);
        
        Assert.True(appChars.IsApplicable('x')); // Explicit char
        Assert.False(appChars.IsApplicable('a')); // Lower letter (not in explicit list and group is NONE)
        Assert.False(appChars.IsApplicable('A')); 
        Assert.False(appChars.IsApplicable('1')); 
    }
    
    [Fact]
    public void GlyphInfo_ImplicitConversions_ShouldWork() {
        var glyph = new MockGlyph();
        var simple = new SimpleGlyphInfo(glyph, 'A');
        GlyphInfo info = simple;
        
        Assert.Equal(simple.Glyph, info.Glyph);
        
        var ligature = new LigatureGlyphInfo(glyph, "==");
        info = ligature;
        Assert.Equal(ligature.Glyph, info.Glyph);
    }
}