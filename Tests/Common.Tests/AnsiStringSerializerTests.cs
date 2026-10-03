// NeoKolors
// Copyright (c) krystof 2026

using static NeoKolors.Common.NKConsoleColor;
using static NeoKolors.Common.NKTextStyles;

namespace NeoKolors.Common.Tests;

public class AnsiStringSerializerTests {

    [Fact]
    public void Roundtrip_EmptyAnsiString_Succeeds() {
        var original = new AnsiString();
        var bytes = AnsiStringSerializer.Serialize(original);
        var deserialized = AnsiStringSerializer.Deserialize(bytes);

        Assert.NotNull(deserialized);
        Assert.Equal(0, deserialized.Length);
        Assert.Equal(string.Empty, deserialized.Plain);
    }

    [Fact]
    public void Roundtrip_NullAnsiString_Succeeds() {
        AnsiString? original = null;
        var bytes = AnsiStringSerializer.Serialize(original);
        var deserialized = AnsiStringSerializer.Deserialize(bytes);

        Assert.Null(deserialized);
    }

    [Fact]
    public void Roundtrip_PlainAnsiString_Succeeds() {
        var original = new AnsiString("Hello, World!");
        var bytes = AnsiStringSerializer.Serialize(original);
        var deserialized = AnsiStringSerializer.Deserialize(bytes);

        Assert.NotNull(deserialized);
        Assert.Equal("Hello, World!", deserialized.Plain);
        Assert.Empty(deserialized.Styles);
    }

    [Fact]
    public void Roundtrip_StyledAnsiString_PreservesAllStyleProperties() {
        var style1 = new NKStyle {
            FColor = NKColor.FromRgb(255, 100, 50),
            BColor = new NKColor(DARK_BLUE),
            Styles = BOLD | ITALIC,
            InheritedStyles = FAINT,
            Underline = new NKUnderlineStyle(new NKColor(RED), NKUnderlineType.CURLY)
        };

        var style2 = new NKStyle {
            FColor = new NKColor(GREEN),
            BColor = NKColor.Default,
            Styles = STRIKETHROUGH,
            Underline = new NKUnderlineStyle(NKColor.FromRgb(0, 255, 128), NKUnderlineType.DOTTED)
        };

        var ansi = new AnsiString("Hello World", style1)
            .OverrideStyle(style2, 6..);

        var bytes = AnsiStringSerializer.Serialize(ansi);
        var deserialized = AnsiStringSerializer.Deserialize(bytes);

        Assert.NotNull(deserialized);
        Assert.Equal("Hello World", deserialized.Plain);
        Assert.Equal(ansi.Length, deserialized.Length);

        // Verify style at index 0 matches style1
        var roundtripStyle0 = deserialized.GetStyleAt(0);
        Assert.Equal(style1.FColor, roundtripStyle0.FColor);
        Assert.Equal(style1.BColor, roundtripStyle0.BColor);
        Assert.Equal(style1.Styles, roundtripStyle0.Styles);
        Assert.Equal(style1.InheritedStyles, roundtripStyle0.InheritedStyles);
        Assert.Equal(style1.Underline.Type, roundtripStyle0.Underline.Type);
        Assert.Equal(style1.Underline.Color, roundtripStyle0.Underline.Color);

        // Verify style at index 6 matches style2
        var roundtripStyle6 = deserialized.GetStyleAt(6);
        Assert.Equal(style2.FColor, roundtripStyle6.FColor);
        Assert.Equal(style2.BColor, roundtripStyle6.BColor);
        Assert.Equal(style2.Styles, roundtripStyle6.Styles);
        Assert.Equal(style2.Underline.Type, roundtripStyle6.Underline.Type);
        Assert.Equal(style2.Underline.Color, roundtripStyle6.Underline.Color);
    }
}
