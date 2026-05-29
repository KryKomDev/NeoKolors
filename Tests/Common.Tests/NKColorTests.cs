//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common.Exceptions;

namespace NeoKolors.Common.Tests;

public class NKColorTests {

    [Fact]
    public void Constructor_ShouldInitializeWithCustomColor() {
        var color = new NKColor(0x123456);
        Assert.Equal(0x123456u, color.Value.AsT0);
        Assert.True(color.IsRgb);
        Assert.False(color.IsDefault || color.IsInherit || color.IsPalette);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithConsoleColor() {
        var color = new NKColor(NKConsoleColor.RED);
        Assert.Equal(NKConsoleColor.RED, color.Value.AsT1);
        Assert.True(color.IsPalette);
        Assert.False(color.IsDefault || color.IsInherit || color.IsRgb);
    }
    
    [Fact]
    public void Inherit_ShouldInitializeCorrectly() {
        var color = NKColor.Inherit;
        Assert.True(color.IsInherit);
        Assert.False(color.IsDefault || color.IsRgb || color.IsPalette);
    }
    
    [Fact]
    public void Default_ShouldInitializeCorrectly() {
        var color = NKColor.Default;
        Assert.True(color.IsDefault);
        Assert.False(color.IsRgb || color.IsInherit || color.IsPalette);
    }

    [Fact]
    public void ImplicitConversion_Int() {
        NKColor color = 0x654321;
        Assert.Equal(0x654321u, (uint)color);
        Assert.True(color.IsRgb);
        Assert.False(color.IsDefault || color.IsInherit || color.IsPalette);
    }

    [Fact]
    public void ImplicitConversion_ConsoleColor() {
        NKColor color = NKConsoleColor.BLUE;
        Assert.Equal(NKConsoleColor.BLUE, (NKConsoleColor)color);
    }

    [Fact]
    public void ImplicitConversion_ToConsoleColor_ShouldThrowException() {
        NKColor color = 0x654321;
        Assert.Throws<InvalidColorCastException>(() => {
            NKConsoleColor unused = color;
        });
    }

    [Fact]
    public void ImplicitConversion_DefaultToConsoleColor_ShouldThrowException() {
        NKColor color = NKColor.Default;
        Assert.Throws<InvalidColorCastException>(() => {
            NKConsoleColor unused = color;
        });
    }

    [Fact]
    public void ImplicitConversion_InheritToConsoleColor_ShouldThrowException() {
        NKColor color = NKColor.Inherit;
        Assert.Throws<InvalidColorCastException>(() => {
            NKConsoleColor unused = color;
        });
    }

    [Fact]
    public void ImplicitConversion_ToInt_ShouldThrowException() {
        NKColor color = NKConsoleColor.GREEN;
        Assert.Throws<InvalidColorCastException>(() => {
            uint unused = color;
        });
    }

    [Fact]
    public void FromRgb_ShouldCreateCorrectColor() {
        var c1 = NKColor.FromRgb(255, 128, 0);
        Assert.True(c1.IsRgb);
        Assert.Equal(0xff8000u, c1.AsRgb);

        var c2 = NKColor.FromRgb(0xabcdefu);
        Assert.Equal(0xabcdefu, c2.AsRgb);
    }

    [Fact]
    public void Match_ShouldExecuteCorrectFunc() {
        var rgb = new NKColor(0x112233);
        var res = rgb.Match(
            _ => "default",
            i => $"rgb {i:x6}",
            _ => "palette",
            _ => "inherit"
        );
        Assert.Equal("rgb 112233", res);

        var pal = new NKColor(NKConsoleColor.BLUE);
        res = pal.Match(
            _ => "default",
            _ => "rgb",
            c => $"palette {c}",
            _ => "inherit"
        );
        Assert.Equal("palette BLUE", res);
    }

    [Fact]
    public void GetInverse_ShouldReturnInvertedColor() {
        var c = NKColor.FromRgb(255, 0, 100);
        var inv = c.GetInverse();
        Assert.Equal(NKColor.FromRgb(0, 255, 155), inv);

        var pal = new NKColor(NKConsoleColor.RED); // 9
        var invPal = pal.GetInverse(); // (9+8)%16 = 1
        Assert.Equal(NKConsoleColor.DARK_RED, invPal.AsPalette);
    }

    [Fact]
    public void ToString_WithFormats_ShouldReturnExpectedStrings() {
        var c = NKColor.FromRgb(0x123456);
        Assert.Equal("123456", c.ToString("P"));
        Assert.Equal("\e[38;2;18;52;86m", c.ToString("T"));
        Assert.Equal("\e[48;2;18;52;86m", c.ToString("B"));

        var pal = new NKColor(NKConsoleColor.RED);
        Assert.Equal("RED", pal.ToString("P"));
        Assert.Equal("\e[38;5;9m", pal.ToString("T"));
    }

    [Fact]
    public void Parse_ShouldParseValidStrings() {
        Assert.Equal(NKColor.FromRgb(0x123456), NKColor.Parse("#123456"));
        Assert.Equal(NKColor.Default, NKColor.Parse("Default"));
        Assert.Equal(NKColor.Inherit, NKColor.Parse("Inherit"));
        Assert.Equal(new NKColor(NKConsoleColor.BLUE), NKColor.Parse("BLUE"));
    }

    [Fact]
    public void TryParse_ShouldHandleInvalidStrings() {
        Assert.True(NKColor.TryParse("#123456", null, out var c1));
        Assert.Equal(NKColor.FromRgb(0x123456), c1);

        Assert.False(NKColor.TryParse("invalid", null, out _));
    }

    [Fact]
    public void IParsableValue_Interface_ShouldWork() {
        IParsableValue<NKColor> parsable = new NKColor();
        Assert.True(parsable.TryParse("#112233", null, out var result));
        Assert.Equal(0x112233u, result.AsRgb);

        IParsableValue nonGeneric = parsable;
        Assert.True(nonGeneric.TryParse("#445566", null, out var objResult));
        Assert.Equal(0x445566u, ((NKColor)objResult!).AsRgb);
    }
    }