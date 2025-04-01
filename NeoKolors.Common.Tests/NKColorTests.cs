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
        Assert.Equal(0x123456, color.Color.AsT0);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithConsoleColor() {
        var color = new NKColor(NKConsoleColor.RED);
        Assert.Equal(NKConsoleColor.RED, color.Color.AsT1);
    }

    [Fact]
    public void ImplicitConversion_Int() {
        NKColor color = 0x654321;
        Assert.Equal(0x654321, (int)color);
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
    public void ImplicitConversion_ToInt_ShouldThrowException() {
        NKColor color = NKConsoleColor.GREEN;
        Assert.Throws<InvalidColorCastException>(() => {
            int unused = color;
        });
    }

    [Fact]
    public void Clone_ShouldReturnCopy() {
        var color = new NKColor(NKConsoleColor.YELLOW);
        var clone = (NKColor)color.Clone();
        Assert.Equal(color.Color, clone.Color);
    }
}