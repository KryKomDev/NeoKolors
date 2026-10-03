// NeoKolors
// Copyright (c) 2026 KryKom

using System.Text;
using static NeoKolors.Common.NKConsoleColor;

namespace NeoKolors.Common.Tests;

public class NKUnderlineTests {

    #region NKUnderlineType Tests

    [Theory]
    [InlineData(NKUnderlineType.NORMAL, "\e[4m")]
    [InlineData(NKUnderlineType.THICK,  "\e[4:2m")]
    [InlineData(NKUnderlineType.CURLY,  "\e[4:3m")]
    [InlineData(NKUnderlineType.DOTTED, "\e[4:4m")]
    [InlineData(NKUnderlineType.DASHED, "\e[4:5m")]
    public void UnderlineType_GetEscSeq_WithEscape_ReturnsExpected(NKUnderlineType type, string expected) {
        Assert.Equal(expected, NKUnderlineType.GetEscSeq(type));
        Assert.Equal(expected, NKUnderlineType.GetEscSeq(type, true));
    }

    [Theory]
    [InlineData(NKUnderlineType.NORMAL, "4;")]
    [InlineData(NKUnderlineType.THICK,  "4:2;")]
    [InlineData(NKUnderlineType.CURLY,  "4:3;")]
    [InlineData(NKUnderlineType.DOTTED, "4:4;")]
    [InlineData(NKUnderlineType.DASHED, "4:5;")]
    public void UnderlineType_GetEscSeq_WithoutEscape_ReturnsInner(NKUnderlineType type, string expected) {
        Assert.Equal(expected, NKUnderlineType.GetEscSeq(type, false));
    }

    [Fact]
    public void UnderlineType_Transition_SameType_ReturnsEmpty() {
        Assert.Equal(string.Empty, NKUnderlineType.GetEscSeq(NKUnderlineType.CURLY, NKUnderlineType.CURLY));

        var sb = new StringBuilder();
        NKUnderlineType.AppendEscSeq(sb, NKUnderlineType.CURLY, NKUnderlineType.CURLY);
        Assert.Equal(0, sb.Length);
    }

    [Fact]
    public void UnderlineType_Transition_DifferentType_ReturnsNext() {
        var seq = NKUnderlineType.GetEscSeq(NKUnderlineType.NORMAL, NKUnderlineType.CURLY);
        Assert.Equal(EscapeCodes.GetUnderline(NKUnderlineType.CURLY), seq);

        var sb = new StringBuilder();
        NKUnderlineType.AppendEscSeq(sb, NKUnderlineType.NORMAL, NKUnderlineType.DOTTED);
        Assert.Equal(EscapeCodes.GetUnderline(NKUnderlineType.DOTTED), sb.ToString());
    }

    [Fact]
    public void UnderlineType_AppendEscSeq_AppendsCorrectly() {
        var sb = new StringBuilder();
        NKUnderlineType.AppendEscSeq(sb, NKUnderlineType.THICK, false);
        Assert.Equal("4:2;", sb.ToString());
    }

    #endregion

    #region NKUnderlineStyle Tests

    [Fact]
    public void UnderlineStyle_Default_InitializesCorrectly() {
        var style = default(NKUnderlineStyle);
        Assert.Equal(NKColor.Default, style.Color);
        Assert.Equal(NKUnderlineType.NORMAL, style.Type);
        Assert.False(style.InheritType);
    }

    [Fact]
    public void UnderlineStyle_Constructor_SetsColorAndType() {
        var color = NKColor.FromRgb(255, 0, 128);
        var style = new NKUnderlineStyle(color, NKUnderlineType.CURLY);

        Assert.Equal(color, style.Color);
        Assert.Equal(NKUnderlineType.CURLY, style.Type);
        Assert.False(style.InheritType);
    }

    [Fact]
    public void UnderlineStyle_PropertyInit_WorksCorrectly() {
        var style = new NKUnderlineStyle {
            Color = new NKColor(RED),
            Type = NKUnderlineType.DASHED,
            InheritType = true
        };

        Assert.Equal(new NKColor(RED), style.Color);
        Assert.Equal(NKUnderlineType.DASHED, style.Type);
        Assert.True(style.InheritType);
    }

    [Fact]
    public void UnderlineStyle_GetEscSeq_WithColor_EmitsUnderlineAndColor() {
        var color = NKColor.FromRgb(200, 100, 50);
        var style = new NKUnderlineStyle(color, NKUnderlineType.CURLY);

        var seq = style.GetEscSeq();

        Assert.Contains("\e[4:3m", seq);
        Assert.Contains(color.Underline, seq);
    }

    [Fact]
    public void UnderlineStyle_GetEscSeq_WithInheritColor_EmitsOnlyUnderline() {
        var style = new NKUnderlineStyle(NKColor.Inherit, NKUnderlineType.DOTTED);

        var seq = style.GetEscSeq();

        Assert.Equal("\e[4:4m", seq);
    }

    [Fact]
    public void UnderlineStyle_With_CombinesStylesCorrectly() {
        // Base style has RED color, CURLY type
        var baseStyle = new NKUnderlineStyle(new NKColor(RED), NKUnderlineType.CURLY);

        // Other has Inherit color and DASHED type
        var other = new NKUnderlineStyle(NKColor.Inherit, NKUnderlineType.DASHED);
        var merged1 = baseStyle.With(other);

        // Color preserved from baseStyle because other is Inherit
        Assert.Equal(new NKColor(RED), merged1.Color);
        // Type overridden by other because other.InheritType is false
        Assert.Equal(NKUnderlineType.DASHED, merged1.Type);

        // When other has InheritType = true, it inherits type from baseStyle
        var otherInheritingType = new NKUnderlineStyle {
            Color = new NKColor(GREEN),
            Type = NKUnderlineType.THICK,
            InheritType = true
        };
        var merged2 = baseStyle.With(otherInheritingType);

        Assert.Equal(new NKColor(GREEN), merged2.Color);
        Assert.Equal(NKUnderlineType.CURLY, merged2.Type);
    }

    [Fact]
    public void UnderlineStyle_ToString_ContainsExpectedText() {
        var style = new NKUnderlineStyle(new NKColor(BLUE), NKUnderlineType.CURLY);
        var str = style.ToString();

        Assert.Contains("NKUnderlineStyle", str);
        Assert.Contains("Color", str);
        Assert.Contains("Type", str);
        Assert.Contains("CURLY", str);
    }

    #endregion
}
