// //
// NeoKolors.Test
// Copyright (c) 2026 KryKom
// //

using NeoKolors.Tui.Fonts.Serialization.Xml.V3;

namespace NeoKolors.Tui.Tests.Fonts;

public class XmlGlyphAlignPointReplaceTests {

    [Fact]
    public void TryParse_SimpleValues_ShouldWork() {
        Assert.True(XmlGlyphAlignPointReplace.TryParse("forg", out var replace));
        Assert.NotNull(replace);
        Assert.Equal(XmlGlyphAlignPointReplaceType.FORG, replace.Value.Default);

        Assert.True(XmlGlyphAlignPointReplace.TryParse("bckg", out replace));
        Assert.NotNull(replace);
        Assert.Equal(XmlGlyphAlignPointReplaceType.BCKG, replace.Value.Default);

        Assert.True(XmlGlyphAlignPointReplace.TryParse("none", out replace));
        Assert.NotNull(replace);
        Assert.Equal(XmlGlyphAlignPointReplaceType.NONE, replace.Value.Default);
    }

    [Fact]
    public void TryParse_CustomValues_ShouldWork() {
        Assert.True(XmlGlyphAlignPointReplace.TryParse("forg: 'abc'", out var replace));
        Assert.NotNull(replace);
        Assert.Equal(['a', 'b', 'c'], replace.Value.CustomForg);

        Assert.True(XmlGlyphAlignPointReplace.TryParse("bckg: 'xyz'", out replace));
        Assert.NotNull(replace);
        Assert.Equal(['x', 'y', 'z'], replace.Value.CustomBckg);

        Assert.True(XmlGlyphAlignPointReplace.TryParse("none: '123'", out replace));
        Assert.NotNull(replace);
        Assert.Equal(['1', '2', '3'], replace.Value.CustomNone);
    }

    [Fact]
    public void TryParse_SingleCharacter_ShouldWork() {
        Assert.True(XmlGlyphAlignPointReplace.TryParse("X", out var replace));
        Assert.NotNull(replace);
        Assert.Equal('X', replace.Value.SingleReplacement);

        Assert.True(XmlGlyphAlignPointReplace.TryParse(@"\n", out replace));
        Assert.NotNull(replace);
        Assert.Equal('\n', replace.Value.SingleReplacement);
    }

    [Fact]
    public void TryParse_CustomPairs_ShouldWork() {
        Assert.True(XmlGlyphAlignPointReplace.TryParse("custom: 'a' 'b'", out var replace));
        Assert.NotNull(replace);
        Assert.Single(replace.Value.CustomPairs);
        Assert.True(replace.Value.CustomPairs.ContainsKey('a'));
        Assert.Equal('b', replace.Value.CustomPairs['a']);

        Assert.True(XmlGlyphAlignPointReplace.TryParse("custom: 'ac' 'bd'", out replace));
        Assert.NotNull(replace);
        Assert.Equal(2, replace.Value.CustomPairs.Count);
        Assert.True(replace.Value.CustomPairs.ContainsKey('a'));
        Assert.Equal('b', replace.Value.CustomPairs['a']);
        Assert.True(replace.Value.CustomPairs.ContainsKey('c'));
        Assert.Equal('d', replace.Value.CustomPairs['c']);
    }

    [Fact]
    public void TryParse_CombinedValues_ShouldWork() {
        Assert.True(XmlGlyphAlignPointReplace.TryParse("forg: 'a', bckg: 'b', none", out var replace));
        Assert.NotNull(replace);
        Assert.Equal(XmlGlyphAlignPointReplaceType.NONE, replace.Value.Default);
        Assert.Equal(['a'], replace.Value.CustomForg);
        Assert.Equal(['b'], replace.Value.CustomBckg);
    }

    [Fact]
    public void TryParse_InvalidValues_ShouldFail() {
        Assert.False(XmlGlyphAlignPointReplace.TryParse("forg, bckg", out _));
        Assert.False(XmlGlyphAlignPointReplace.TryParse("custom: 'a'", out _)); // missing second quote
        Assert.False(XmlGlyphAlignPointReplace.TryParse("custom: 'a' 'bc'", out _)); // length mismatch
    }

    [Fact]
    public void ToString_ShouldBeReversible() {
        string input = "forg: 'a', bckg: 'b', none";
        Assert.True(XmlGlyphAlignPointReplace.TryParse(input, out var replace));
        Assert.NotNull(replace);
        string output = replace.Value.ToString();
        Assert.True(XmlGlyphAlignPointReplace.TryParse(output, out var replace2));
        Assert.NotNull(replace2);
        
        Assert.Equal(replace.Value.Default, replace2.Value.Default);
        Assert.Equal(replace.Value.CustomForg, replace2.Value.CustomForg);
        Assert.Equal(replace.Value.CustomBckg, replace2.Value.CustomBckg);
    }
}
