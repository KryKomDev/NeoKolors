// //
// NeoKolors.Test
// Copyright (c) 2026 KryKom
// //

using NeoKolors.Tui.Fonts.Serialization.Xml.V3;

namespace NeoKolors.Tui.Tests.Fonts;

public class XmlGlyphMaskTests {
    
    [Fact]
    public void TryParse_SimpleValues_ShouldWork() {
        Assert.True(XmlGlyphMask.TryParse("forg", out var mask));
        Assert.NotNull(mask);
        Assert.Equal(XmlSpaceMask.FOREGROUND, mask.Value.SpaceConf);
        Assert.Empty(mask.Value.MapForg);
        Assert.Empty(mask.Value.MapBckg);

        Assert.True(XmlGlyphMask.TryParse("bckg", out mask));
        Assert.NotNull(mask);
        Assert.Equal(XmlSpaceMask.BACKGROUND, mask.Value.SpaceConf);
    }

    [Fact]
    public void TryParse_CustomValues_ShouldWork() {
        Assert.True(XmlGlyphMask.TryParse("custom-forg: 'abc'", out var mask));
        Assert.NotNull(mask);
        Assert.Equal(['a', 'b', 'c'], mask.Value.MapForg);

        Assert.True(XmlGlyphMask.TryParse("custom-bckg: 'xyz'", out mask));
        Assert.NotNull(mask);
        Assert.Equal(['x', 'y', 'z'], mask.Value.MapBckg);
    }

    [Fact]
    public void TryParse_CombinedValues_ShouldWork() {
        Assert.True(XmlGlyphMask.TryParse("custom-forg: 'xyz', custom-bckg: 'abc', bckg", out var mask));
        Assert.NotNull(mask);
        Assert.Equal(XmlSpaceMask.BACKGROUND, mask.Value.SpaceConf);
        Assert.Equal(['x', 'y', 'z'], mask.Value.MapForg);
        Assert.Equal(['a', 'b', 'c'], mask.Value.MapBckg);
    }

    [Fact]
    public void TryParse_EscapedValues_ShouldWork() {
        Assert.True(XmlGlyphMask.TryParse(@"custom-forg: '\n\t\''", out var mask));
        Assert.NotNull(mask);
        Assert.Equal(['\n', '\t', '\''], mask.Value.MapForg);

        Assert.True(XmlGlyphMask.TryParse(@"custom-bckg: '\x41\u0042'", out mask));
        Assert.NotNull(mask);
        Assert.Equal(['A', 'B'], mask.Value.MapBckg);
    }

    [Fact]
    public void TryParse_InvalidValues_ShouldFail() {
        Assert.False(XmlGlyphMask.TryParse("forg, bckg", out _));
        Assert.False(XmlGlyphMask.TryParse("unknown", out _));
        Assert.False(XmlGlyphMask.TryParse("custom-forg: abc", out _)); // missing quotes
    }

    [Fact]
    public void TryParse_CommaInQuotes_ShouldWork() {
        Assert.True(XmlGlyphMask.TryParse("custom-forg: 'a,b', bckg", out var mask));
        Assert.NotNull(mask);
        Assert.Equal(XmlSpaceMask.BACKGROUND, mask.Value.SpaceConf);
        Assert.Equal(['a', ',', 'b'], mask.Value.MapForg);
    }
}
