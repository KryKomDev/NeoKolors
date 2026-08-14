// 
// NeoKolors//

using static NeoKolors.Common.NKConsoleColor;
using static NeoKolors.Common.TextStyles;

namespace NeoKolors.Common.Tests;

public class NKStyleTests {

    [Fact]
    public void TestDefaultConstructor() {
        var style = new NKStyle();
        Assert.True(style.IsFColorDefault);
        Assert.True(style.IsBColorDefault);
        Assert.Equal(0, (byte)style.Styles);
    }

    [Fact]
    public void TestSetFColorCustom() {
        var style = new NKStyle();
        var color = new NKColor(0x123456);
        style.SetFColor(color);
        Assert.True(style.IsFColorCustom);
        Assert.Equal(color, style.FColor);
    }

    [Fact]
    public void TestSetBColorCustom() {
        var style = new NKStyle();
        var color = new NKColor(0x654321);
        style.SetBColor(color);
        Assert.True(style.IsBColorCustom);
        Assert.Equal(color, style.BColor);
    }

    [Fact]
    public void TestSetFColorConsole() {
        var style = new NKStyle();
        var color = new NKColor(RED);
        style.SetFColor(color);
        Assert.Equal(color, style.FColor);
        Assert.False(style.IsFColorCustom);
    }

    [Fact]
    public void TestSetBColorConsole() {
        var style = new NKStyle();
        var color = new NKColor(RED);
        style.SetBColor(color);
        Assert.False(style.IsBColorCustom);
        Assert.Equal(color, style.BColor);
    }

    [Fact]
    public void TestSetFColorDefault() {
        var style = new NKStyle();
        var color = new NKColor();
        style.SetFColor(color);
        Assert.Equal(color, style.FColor);
        Assert.False(style.IsFColorCustom);
        Assert.True(style.IsFColorDefault);
    }

    [Fact]
    public void TestSetBColorDefault() {
        var style = new NKStyle();
        var color = new NKColor();
        style.SetBColor(color);
        Assert.Equal(color, style.BColor);
        Assert.False(style.IsBColorCustom);
        Assert.True(style.IsBColorDefault);
    }

    [Fact]
    public void TestSetStyles() {
        var style = new NKStyle();
        style.SetStyles(BOLD | ITALIC);
        Assert.True(style.Styles.HasFlag(BOLD));
        Assert.True(style.Styles.HasFlag(ITALIC));
    }

    [Fact]
    public void TestEquals() {
        var style1 = new NKStyle(new NKColor(0x123456), new NKColor(0x654321), BOLD);
        var style2 = new NKStyle(new NKColor(0x123456), new NKColor(0x654321), BOLD);
        Assert.True(style1.Equals(style2));
        Assert.True(style1 == style2);
    }

    [Fact]
    public void TestNotEquals() {
        var style1 = new NKStyle(new NKColor(0x123456), new NKColor(0x654321), BOLD);
        var style2 = new NKStyle(new NKColor(0x654321), new NKColor(0x123456), ITALIC);
        Assert.False(style1.Equals(style2));
        Assert.False(style1 == style2);
    }

    [Theory]
    [InlineData(0xff0000, 0x00ff00)]
    [InlineData(RED,      BLUE)]
    public void Constructor_SetsCorrectValues(NKColor f, NKColor b) {
        var styles = BOLD | ITALIC;

        var style = new NKStyle(f, b, styles);

        Assert.Equal(f,      style.FColor);
        Assert.Equal(b,      style.BColor);
        Assert.Equal(styles, style.Styles);
    }

    // [Fact]
    // public void Constructor_RunUntilFail_Rgb_Rgb() {
    //     var r = new Random();
    //     var f = new NKColor(r.Next(0, 0xffffff));
    //     var b = new NKColor(r.Next(0, 0xffffff));
    //     var s = (TextStyles)r.Next(0, 0xff);
    //     var n = new NKStyle(f, b, s);
    //     Assert.Equal(f, n.FColor);
    //     Assert.Equal(b, n.BColor);
    //     Assert.Equal(s, n.Styles);
    // }
    //
    // [Fact]
    // public void Constructor_RunUntilFail_Rgb_Con() {
    //     var r = new Random();
    //     var f = new NKColor(r.Next(0, 0xffffff));
    //     var b = new NKColor((NKConsoleColor)r.Next(0, 0xff));
    //     var s = (TextStyles)r.Next(0, 0xff);
    //     var n = new NKStyle(f, b, s);
    //     Assert.Equal(f, n.FColor);
    //     Assert.Equal(b, n.BColor);
    //     Assert.Equal(s, n.Styles);
    // }
    //
    // [Fact]
    // public void Constructor_RunUntilFail_Con_Rgb() {
    //     var r = new Random();
    //     var f = new NKColor((NKConsoleColor)r.Next(0, 0xff));
    //     var b = new NKColor(r.Next(0, 0xffffff));
    //     var s = (TextStyles)r.Next(0, 0xff);
    //     var n = new NKStyle(f, b, s);
    //     Assert.Equal(f, n.FColor);
    //     Assert.Equal(b, n.BColor);
    //     Assert.Equal(s, n.Styles);
    // }

    [Fact]
    public void Constructor_ShouldInitCorrectly_Default_Inherit() {
        var f = NKColor.Default;
        var b = NKColor.Inherit;
        var s = NONE;
        var n = new NKStyle(f, b, s);
        Assert.Equal(f, n.FColor);
        Assert.Equal(b, n.BColor);
        Assert.Equal(s, n.Styles);
    }


    [Fact]
    public void SetStyles_ShouldPreserveColors() {
        var f = new NKColor(0x112233);
        var b = new NKColor(0x445566);
        var s = new NKStyle(f, b);

        Assert.Equal(f,    s.FColor);
        Assert.Equal(b,    s.BColor);
        Assert.Equal(NONE, s.Styles);

        s.SetStyles(BOLD | UNDERLINE);

        Assert.Equal(f,                s.FColor);
        Assert.Equal(b,                s.BColor);
        Assert.Equal(BOLD | UNDERLINE, s.Styles);

        s.SetStyles(NONE);
        Assert.Equal(NONE, s.Styles);
        Assert.Equal(f,    s.FColor);
        Assert.Equal(b,    s.BColor);
    }

    [Fact]
    public void SetFColor_ShouldPreserveStyles() {
        var s = new NKStyle(s: ITALIC);
        Assert.Equal(ITALIC, s.Styles);

        s.SetFColor(new NKColor(0xAABBCC));
        Assert.Equal(ITALIC, s.Styles);
    }

    [Fact]
    public void SetBColor_ShouldPreserveStyles() {
        var s = new NKStyle(s: STRIKETHROUGH);
        Assert.Equal(STRIKETHROUGH, s.Styles);

        s.SetBColor(new NKColor(0xDDEEFF));
        Assert.Equal(STRIKETHROUGH, s.Styles);
    }

    [Fact]
    public void TestSetStyles_Overwrite_ShouldWork() {
        var s = new NKStyle(s: BOLD);
        s.SetStyles(FAINT); // Overwrite, not merge
        Assert.Equal(FAINT, s.Styles);
        Assert.False(s.Styles.HasFlag(BOLD));
    }

    [Fact]
    public void Override_ShouldApplyNonInheritValues() {
        var baseStyle = new NKStyle(RED,  BLACK,           BOLD);
        var overrider = new NKStyle(BLUE, NKColor.Inherit, ITALIC);

        var result = baseStyle.OverrideWith(overrider);

        Assert.Equal((NKColor)BLUE,  result.FColor);
        Assert.Equal((NKColor)BLACK, result.BColor);
        Assert.Equal(ITALIC,         result.Styles);
    }

    [Fact]
    public void GetEscSeq_ShouldReturnDiff() {
        var s1 = new NKStyle(RED,  BLACK, BOLD);
        var s2 = new NKStyle(BLUE, BLACK, BOLD | ITALIC);

        var esc = NKStyle.GetEscSeq(s1, s2);

        // s2 has BLUE instead of RED, and adds ITALIC
        Assert.Contains("38;5;12", esc);   // BLUE
        Assert.Contains("3;",      esc);   // ITALIC start
        Assert.DoesNotContain("22;", esc); // BOLD should stay
    }

    [Fact]
    public void ToString_WithFormats_ShouldReturnExpectedStrings() {
        var style = new NKStyle(RED, BLACK, BOLD);

        Assert.Contains("FColor: RED, BColor: BLACK, Bold", style.ToString("P", null));
        Assert.StartsWith("\e[", style.ToAnsi());
    }

    [Theory]
    [InlineData("f#red",                               RED,      null,     NONE)]
    [InlineData("F#DARK-RED",                          DARK_RED, null,     NONE)]
    [InlineData("b#blue; bold",                        null,     BLUE,     BOLD)]
    [InlineData("f#123456,b#654321;italic,bold",       0x123456, 0x654321, ITALIC | BOLD)]
    [InlineData("strikethrough; b#inherit; f#default", null,     null,     STRIKETHROUGH)]
    [InlineData("italic, strikethrough",               null,     null,     ITALIC | STRIKETHROUGH)]
    public void TestParse_ValidInputs(string input, object? expectedF, object? expectedB, TextStyles expectedStyles) {
        var style = NKStyle.Parse(input);
        Assert.Equal(expectedStyles, style.Styles);

        if (expectedF is NKConsoleColor fConsole) {
            Assert.Equal((NKColor)fConsole, style.FColor);
        }
        else if (expectedF is int fInt) {
            Assert.Equal((NKColor)fInt, style.FColor);
        }
        else if (input.Contains("f#default")) {
            Assert.True(style.IsFColorDefault);
        }

        if (expectedB is NKConsoleColor bConsole) {
            Assert.Equal((NKColor)bConsole, style.BColor);
        }
        else if (expectedB is int bInt) {
            Assert.Equal((NKColor)bInt, style.BColor);
        }
        else if (input.Contains("b#inherit")) {
            Assert.True(style.IsBColorInherit);
        }
    }

    [Fact]
    public void TestParse_EmptyString_ReturnsDefault() {
        var style = NKStyle.Parse("");
        Assert.Equal(NKStyle.Default, style);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("f#")]
    [InlineData("f#invalid-color")]
    [InlineData("b#not-a-color")]
    public void TestParse_InvalidInputs_ThrowsFormatException(string input) {
        Assert.Throws<FormatException>(() => NKStyle.Parse(input));
    }

    [Fact]
    public void TestTryParse_Success() {
        var ok = NKStyle.TryParse("f#green; bold", out var style);
        Assert.True(ok);
        Assert.Equal((NKColor)GREEN, style.FColor);
        Assert.True(style.Styles.HasFlag(BOLD));
    }

    [Fact]
    public void TestTryParse_Failure() {
        var ok = NKStyle.TryParse("invalid-part", out var style);
        Assert.False(ok);
        Assert.Equal(default, style);
    }
}