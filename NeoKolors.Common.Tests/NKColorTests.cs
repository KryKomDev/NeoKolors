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
    public void Equality_ShouldWorkCorrectly() {
        var c1 = new NKColor(0x123456);
        var c2 = new NKColor(0x123456);
        var c3 = new NKColor(0x654321);
        var c4 = NKColor.Default;
        var c5 = NKColor.Default;
        var c6 = NKColor.Inherit;

        Assert.True(c1.Equals(c2));
        Assert.True(c1 == c2);
        Assert.False(c1 != c2);
        
        Assert.False(c1.Equals(c3));
        Assert.False(c1 == c3);
        
        Assert.True(c4.Equals(c5));
        Assert.False(c4.Equals(c6));
        
        Assert.Equal(c1.GetHashCode(), c2.GetHashCode());
        Assert.NotEqual(c1.GetHashCode(), c3.GetHashCode());
        Assert.Equal(c4.GetHashCode(), c5.GetHashCode());
    }
}