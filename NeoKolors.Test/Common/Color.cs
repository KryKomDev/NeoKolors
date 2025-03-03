//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using NeoKolors.Common.Exceptions;

namespace NeoKolors.Test.Common;

public class ColorTests {

    [Fact]
    public void Constructor_ShouldInitializeWithCustomColor() {
        var color = new Color(0x123456);
        Assert.False(color.IsPaletteSafe);
        Assert.Equal(0x123456, color.CustomColor);
        Assert.Null(color.ConsoleColor);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithConsoleColor() {
        var color = new Color(ConsoleColor.Red);
        Assert.True(color.IsPaletteSafe);
        Assert.Equal(ConsoleColor.Red, color.ConsoleColor);
        Assert.Null(color.CustomColor);
    }

    [Fact]
    public void ImplicitConversion_FromConsoleColor() {
        Color color = ConsoleColor.Blue;
        Assert.True(color.IsPaletteSafe);
        Assert.Equal(ConsoleColor.Blue, color.ConsoleColor);
    }

    [Fact]
    public void ImplicitConversion_FromInt() {
        Color color = 0x654321;
        Assert.False(color.IsPaletteSafe);
        Assert.Equal(0x654321, color.CustomColor);
    }

    [Fact]
    public void ImplicitConversion_ToConsoleColor_ShouldThrowException() {
        Color color = 0x654321;
        Assert.Throws<InvalidColorCastException>(() => {
            ConsoleColor consoleColor = color;
        });
    }

    [Fact]
    public void ImplicitConversion_ToInt_ShouldThrowException() {
        Color color = ConsoleColor.Green;
        Assert.Throws<InvalidColorCastException>(() => {
            int customColor = color;
        });
    }

    [Fact]
    public void Clone_ShouldReturnCopy() {
        var color = new Color(ConsoleColor.Yellow);
        var clone = (Color)color.Clone();
        Assert.Equal(color.ConsoleColor, clone.ConsoleColor);
        Assert.Equal(color.IsPaletteSafe, clone.IsPaletteSafe);
    }
}