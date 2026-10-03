// 
// NeoKolors//

using static NeoKolors.Common.NKConsoleColor;
using static NeoKolors.Common.NKTextStyles;

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
        const NKTextStyles styles = BOLD | ITALIC;

        var style = new NKStyle(f, b, styles);

        Assert.Equal(f,      style.FColor);
        Assert.Equal(b,      style.BColor);
        Assert.Equal(styles, style.Styles);
    }

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
    public void Override_ShouldApplyNonInheritValues() {
        var baseStyle = new NKStyle(RED,  BLACK,           BOLD);
        var overrider = new NKStyle(BLUE, NKColor.Inherit, ITALIC);

        var result = baseStyle.With(overrider);

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
    public void TestParse_ValidInputs(
        string       input,
        object?      expectedF,
        object?      expectedB,
        NKTextStyles expectedStyles
    ) {
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

    [Fact]
    public void RawZero_ReturnsDefaultColors() {
        var style = new NKStyle(0ul, 0ul);
        Assert.Equal(NKColor.Default, style.GetFColor());
        Assert.Equal(NKColor.Default, style.GetBColor());
        Assert.Equal(NKColor.Default, style.FColor);
        Assert.Equal(NKColor.Default, style.BColor);
        Assert.True(style.IsFColorDefault);
        Assert.True(style.IsBColorDefault);
    }

    [Fact]
    public void DefaultStruct_ReturnsDefaultColors() {
        NKStyle style = default;
        Assert.Equal(NKColor.Default, style.GetFColor());
        Assert.Equal(NKColor.Default, style.GetBColor());
        Assert.Equal(NKColor.Default, style.FColor);
        Assert.Equal(NKColor.Default, style.BColor);
    }

    [Fact]
    public void PropertyInit_SupportsAllColorTypes() {
        var defaultColor = NKColor.Default;
        var consoleColor = new NKColor(DARK_MAGENTA);
        var rgbColor     = NKColor.FromRgb(10, 20, 30);
        var inheritColor = NKColor.Inherit;

        var s1 = new NKStyle { FColor = defaultColor, BColor = consoleColor };
        Assert.Equal(defaultColor, s1.GetFColor());
        Assert.Equal(consoleColor, s1.GetBColor());
        Assert.True(s1.IsFColorDefault);
        Assert.True(s1.IsBColorConsole);

        var s2 = s1 with { FColor = rgbColor, BColor = inheritColor };
        Assert.Equal(rgbColor, s2.GetFColor());
        Assert.Equal(inheritColor, s2.GetBColor());
        Assert.True(s2.IsFColorCustom);
        Assert.True(s2.IsBColorInherit);

        var s3 = new NKStyle { FColor = inheritColor, BColor = rgbColor };
        Assert.Equal(inheritColor, s3.FColor);
        Assert.Equal(rgbColor, s3.BColor);
    }

    [Fact]
    public void StyleMode_IsIndependentOfColors() {
        var style = new NKStyle {
            FColor = NKColor.FromRgb(255, 0, 128),
            BColor = new NKColor(CYAN),
            Styles = BOLD,
            InheritedStyles = ITALIC
        };

        Assert.Equal(NKColor.FromRgb(255, 0, 128), style.FColor);
        Assert.Equal((NKColor)CYAN, style.BColor);
        Assert.Equal(BOLD, style.Styles);
        Assert.Equal(ITALIC, style.InheritedStyles);

        var updated = style with { FColor = NKColor.Default };
        Assert.Equal(NKColor.Default, updated.FColor);
        Assert.Equal((NKColor)CYAN, updated.BColor);
        Assert.Equal(BOLD, updated.Styles);
        Assert.Equal(ITALIC, updated.InheritedStyles);
    }

    [Fact]
    public void WithFColor_And_WithBColor_WorkCorrectly() {
        var style = new NKStyle(RED, BLUE, BOLD);

        var s1 = style.WithFColor(GREEN);
        Assert.Equal((NKColor)GREEN, s1.FColor);
        Assert.Equal((NKColor)BLUE,  s1.BColor);

        // When passing NKColor.Inherit, it should remain unchanged
        var s2 = s1.WithFColor(NKColor.Inherit);
        Assert.Equal((NKColor)GREEN, s2.FColor);

        var s3 = style.WithBColor(YELLOW);
        Assert.Equal((NKColor)RED,    s3.FColor);
        Assert.Equal((NKColor)YELLOW, s3.BColor);

        var s4 = s3.WithBColor(NKColor.Inherit);
        Assert.Equal((NKColor)YELLOW, s4.BColor);
    }

    [Fact]
    public void WithStyles_AppliesAndInheritsCorrectly() {
        var style = new NKStyle(RED, BLUE, BOLD);

        var withItalic = style.WithStyles(ITALIC);
        Assert.Equal(ITALIC, withItalic.Styles);

        // Inheriting BOLD from previous style
        var inherited = style.WithStyles(ITALIC, inheritedStyles: BOLD);
        Assert.Equal(BOLD | ITALIC, inherited.Styles);
    }

    [Fact]
    public void Underline_And_UColor_Properties_WorkCorrectly() {
        var underline = new NKUnderlineStyle(new NKColor(RED), NKUnderlineType.CURLY);
        var style = new NKStyle { Underline = underline };

        Assert.Equal(NKUnderlineType.CURLY, style.Underline.Type);
        Assert.Equal(new NKColor(RED), style.Underline.Color);
        Assert.Equal(new NKColor(RED), style.UColor);

        var updatedUColor = style with { UColor = new NKColor(GREEN) };
        Assert.Equal(new NKColor(GREEN), updatedUColor.UColor);
        Assert.Equal(new NKColor(GREEN), updatedUColor.Underline.Color);

        var withUnderline = style.WithUnderline(new NKUnderlineStyle(new NKColor(BLUE), NKUnderlineType.DOTTED));
        Assert.Equal(NKUnderlineType.DOTTED, withUnderline.Underline.Type);
        Assert.Equal(new NKColor(BLUE), withUnderline.Underline.Color);
    }

    [Fact]
    public void OperatorShiftLeft_BehavesAsWith() {
        var baseStyle  = new NKStyle(RED,  BLACK,           BOLD);
        var overrider  = new NKStyle(BLUE, NKColor.Inherit, ITALIC);

        var result = baseStyle << overrider;

        Assert.Equal((NKColor)BLUE,  result.FColor);
        Assert.Equal((NKColor)BLACK, result.BColor);
        Assert.Equal(ITALIC,         result.Styles);
    }

    [Fact]
    public void GetEscSeq_SingleStyle_GeneratesCorrectAnsi() {
        var style = new NKStyle {
            FColor = new NKColor(RED),
            BColor = new NKColor(BLUE),
            Styles = BOLD
        };

        var esc = NKStyle.GetEscSeq(style);
        Assert.StartsWith("\e[", esc);
        Assert.EndsWith("m", esc);
        Assert.Contains("1", esc);      // BOLD
        Assert.Contains("38;5;9", esc); // RED
        Assert.Contains("48;5;12", esc); // BLUE
    }

    [Fact]
    public void GetEscSeq_WithUnderlineTransition_EmitsUnderline() {
        var prev = new NKStyle(RED, BLACK);
        var next = prev with {
            Underline = new NKUnderlineStyle(new NKColor(GREEN), NKUnderlineType.CURLY)
        };

        var esc = NKStyle.GetEscSeq(prev, next);
        Assert.Contains("4:3", esc);     // CURLY underline
        Assert.Contains("58;5;10", esc); // GREEN underline color
    }

    [Fact]
    public void GetEscSeq_BoldAndFaintTransitions_EmitsCorrectSequences() {
        var bold = new NKStyle(styles: BOLD);
        var faint = new NKStyle(styles: FAINT);
        var boldFaint = new NKStyle(styles: BOLD | FAINT);
        var def = NKStyle.Default;

        // BOLD -> Default (turns off bold, must not turn on faint)
        Assert.Equal("\e[22m", NKStyle.GetEscSeq(bold, def));

        // FAINT -> Default (turns off faint, must not turn on bold)
        Assert.Equal("\e[22m", NKStyle.GetEscSeq(faint, def));

        // BOLD -> FAINT (resets intensity and sets faint)
        Assert.Equal("\e[22;2m", NKStyle.GetEscSeq(bold, faint));

        // FAINT -> BOLD (resets intensity and sets bold)
        Assert.Equal("\e[22;1m", NKStyle.GetEscSeq(faint, bold));

        // Default -> BOLD
        Assert.Equal("\e[1m", NKStyle.GetEscSeq(def, bold));

        // Default -> FAINT
        Assert.Equal("\e[2m", NKStyle.GetEscSeq(def, faint));

        // BOLD|FAINT -> BOLD (turns off faint, keeps bold)
        Assert.Equal("\e[22;1m", NKStyle.GetEscSeq(boldFaint, bold));

        // BOLD|FAINT -> FAINT (turns off bold, keeps faint)
        Assert.Equal("\e[22;2m", NKStyle.GetEscSeq(boldFaint, faint));

        // BOLD|FAINT -> Default (turns off both)
        Assert.Equal("\e[22m", NKStyle.GetEscSeq(boldFaint, def));
    }

    [Fact]
    public void Protobuf_Serialization_Roundtrip() {
        var original = new NKStyle {
            FColor          = NKColor.FromRgb(255, 128, 64),
            BColor          = new NKColor(NKConsoleColor.DARK_BLUE),
            Styles          = BOLD | ITALIC,
            InheritedStyles = FAINT,
            Underline       = new NKUnderlineStyle(NKColor.FromRgb(0, 200, 100), NKUnderlineType.CURLY)
        };

        using var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize(ms, original);
        ms.Position = 0;
        var deserialized = ProtoBuf.Serializer.Deserialize<NKStyle>(ms);

        Assert.Equal(original, deserialized);
        Assert.Equal(original.Raw0, deserialized.Raw0);
        Assert.Equal(original.Raw1, deserialized.Raw1);
        Assert.Equal(original.FColor, deserialized.FColor);
        Assert.Equal(original.BColor, deserialized.BColor);
        Assert.Equal(original.Styles, deserialized.Styles);
        Assert.Equal(original.InheritedStyles, deserialized.InheritedStyles);
        Assert.Equal(original.Underline, deserialized.Underline);
    }
}