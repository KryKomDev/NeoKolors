//
// NeoKolors
// Copyright (c) 2025 KryKom
//

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
    public void Match_ActionOverload_ExecutesCorrectBranch() {
        int branch = 0;

        NKColor.Default.Match(
            _ => branch = 1,
            _ => branch = 2,
            _ => branch = 3,
            _ => branch = 4
        );
        Assert.Equal(1, branch);

        NKColor.FromRgb(10, 20, 30).Match(
            _ => branch = 1,
            _ => branch = 2,
            _ => branch = 3,
            _ => branch = 4
        );
        Assert.Equal(2, branch);

        new NKColor(NKConsoleColor.CYAN).Match(
            _ => branch = 1,
            _ => branch = 2,
            _ => branch = 3,
            _ => branch = 4
        );
        Assert.Equal(3, branch);

        NKColor.Inherit.Match(
            _ => branch = 1,
            _ => branch = 2,
            _ => branch = 3,
            _ => branch = 4
        );
        Assert.Equal(4, branch);
    }

    [Fact]
    public void Lerp_ValidRgbColors_InterpolatesCorrectly() {
        var start = NKColor.FromRgb(0, 0, 0);
        var end   = NKColor.FromRgb(100, 200, 50);

        var mid = NKColor.Lerp(start, end, 0.5f);
        Assert.Equal(NKColor.FromRgb(50, 100, 25), mid);

        var zero = NKColor.Lerp(start, end, 0f);
        Assert.Equal(start, zero);

        var one = NKColor.Lerp(start, end, 1f);
        Assert.Equal(end, one);
    }

    [Fact]
    public void Lerp_NonRgbColor_ThrowsInvalidOperationException() {
        var rgb = NKColor.FromRgb(10, 20, 30);
        var pal = new NKColor(NKConsoleColor.RED);

        Assert.Throws<InvalidOperationException>(() => NKColor.Lerp(pal, pal, 0.5f));
    }

    [Fact]
    public void GetMultiStopColor_InterpolatesAcrossSegments() {
        var colors = new[] {
            NKColor.FromRgb(0, 0, 0),
            NKColor.FromRgb(100, 100, 100),
            NKColor.FromRgb(200, 200, 200)
        };

        Assert.Equal(colors[0], NKColor.GetMultiStopColor(colors, 0f));
        Assert.Equal(colors[1], NKColor.GetMultiStopColor(colors, 0.5f));
        Assert.Equal(colors[2], NKColor.GetMultiStopColor(colors, 1f));

        // Edge cases: empty array and single element
        Assert.Equal(NKColor.Default, NKColor.GetMultiStopColor([], 0.5f));
        Assert.Equal(colors[0], NKColor.GetMultiStopColor([colors[0]], 0.5f));
    }

    [Fact]
    public void PredefinedColors_HaveCorrectPaletteValues() {
        Assert.Equal(NKConsoleColor.WHITE,        NKColor.White.AsPalette);
        Assert.Equal(NKConsoleColor.BLACK,        NKColor.Black.AsPalette);
        Assert.Equal(NKConsoleColor.RED,          NKColor.Red.AsPalette);
        Assert.Equal(NKConsoleColor.GREEN,        NKColor.Green.AsPalette);
        Assert.Equal(NKConsoleColor.BLUE,         NKColor.Blue.AsPalette);
        Assert.Equal(NKConsoleColor.YELLOW,       NKColor.Yellow.AsPalette);
        Assert.Equal(NKConsoleColor.CYAN,         NKColor.Cyan.AsPalette);
        Assert.Equal(NKConsoleColor.MAGENTA,      NKColor.Magenta.AsPalette);
        Assert.Equal(NKConsoleColor.DARK_RED,     NKColor.DarkRed.AsPalette);
        Assert.Equal(NKConsoleColor.DARK_GREEN,   NKColor.DarkGreen.AsPalette);
        Assert.Equal(NKConsoleColor.DARK_YELLOW,  NKColor.DarkYellow.AsPalette);
        Assert.Equal(NKConsoleColor.DARK_BLUE,    NKColor.DarkBlue.AsPalette);
        Assert.Equal(NKConsoleColor.DARK_MAGENTA, NKColor.DarkMagenta.AsPalette);
        Assert.Equal(NKConsoleColor.DARK_CYAN,    NKColor.DarkCyan.AsPalette);
        Assert.Equal(NKConsoleColor.DARK_GRAY,    NKColor.DarkGray.AsPalette);
        Assert.Equal(NKConsoleColor.GRAY,         NKColor.Gray.AsPalette);
    }

    [Fact]
    public void Underline_Property_ReturnsExpectedAnsi() {
        var rgb = NKColor.FromRgb(10, 20, 30);
        Assert.Equal("\e[58;2;10;20;30m", rgb.Underline);
        Assert.Equal("\e[58;2;10;20;30m", rgb.ToString("U"));

        var pal = new NKColor(NKConsoleColor.RED);
        Assert.Equal("\e[58;5;9m", pal.Underline);
        Assert.Equal("\e[58;5;9m", pal.ToString("U"));

        Assert.Equal(EscapeCodes.UNDERLINE_COLOR_RESET, NKColor.Default.Underline);
        Assert.Equal("Inherit", NKColor.Inherit.Underline);
    }

    [Fact]
    public void NKColorExtensions_ConvertsValuesCorrectly() {
        int intVal = 0x123456;
        NKColor c1 = intVal.Rgb;
        Assert.True(c1.IsRgb);
        Assert.Equal(0x123456u, c1.AsRgb);

        uint uintVal = 0x654321u;
        NKColor c2 = uintVal.Rgb;
        Assert.True(c2.IsRgb);
        Assert.Equal(0x654321u, c2.AsRgb);

        NKConsoleColor palVal = NKConsoleColor.CYAN;
        NKColor c3 = palVal.NK;
        Assert.True(c3.IsPalette);
        Assert.Equal(NKConsoleColor.CYAN, c3.AsPalette);
    }

    [Theory]
    [InlineData(0x123456)]
    [InlineData(0)]
    [InlineData(0xffffff)]
    public void Protobuf_Serialization_Rgb_Roundtrip(int rgb) {
        var original = new NKColor(rgb);
        using var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize(ms, original);
        ms.Position = 0;
        var deserialized = ProtoBuf.Serializer.Deserialize<NKColor>(ms);

        Assert.Equal(original, deserialized);
        Assert.Equal(original.Type, deserialized.Type);
        Assert.Equal(original.AsRgb, deserialized.AsRgb);
    }

    [Fact]
    public void Protobuf_Serialization_SpecialColors_Roundtrip() {
        foreach (var original in new[] { NKColor.Default, NKColor.Inherit, new NKColor(NKConsoleColor.MAGENTA) }) {
            using var ms = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, original);
            ms.Position = 0;
            var deserialized = ProtoBuf.Serializer.Deserialize<NKColor>(ms);

            Assert.Equal(original, deserialized);
            Assert.Equal(original.Type, deserialized.Type);
        }
    }
}