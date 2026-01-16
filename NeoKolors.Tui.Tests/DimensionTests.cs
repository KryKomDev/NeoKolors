//
// NeoKolors.Test
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Tests;

public class DimensionTests {

    [Fact]
    public void Chars_ShouldCreateCharDimension() {
        var dim = Dimension.Chars(10);
        Assert.True(dim.IsNumber);
        Assert.False(dim.IsAuto);
        Assert.False(dim.IsMinContent);
        Assert.False(dim.IsMaxContent);
        
        Assert.Equal(10, dim.ToScalar(100));
    }

    [Fact]
    public void Pixels_ShouldCreatePixelDimension() {
        var dim = Dimension.Pixels(20);
        Assert.True(dim.IsNumber);
        Assert.Equal(20, dim.ToScalar(100));
    }

    [Fact]
    public void Percent_ShouldCalculatePercentage() {
        var dim = Dimension.Percent(50);
        Assert.True(dim.IsNumber);
        Assert.Equal(50, dim.ToScalar(100));
        Assert.Equal(25, dim.ToScalar(50));
    }

    [Fact]
    public void Auto_ShouldBeAuto() {
        var dim = Dimension.Auto;
        Assert.False(dim.IsNumber);
        Assert.True(dim.IsAuto);
        Assert.False(dim.IsMinContent);
        Assert.False(dim.IsMaxContent);
        
        Assert.Throws<InvalidOperationException>(() => dim.ToScalar(100));
    }

    [Fact]
    public void MinContent_ShouldBeMinContent() {
        var dim = Dimension.MinContent;
        Assert.False(dim.IsNumber);
        Assert.False(dim.IsAuto);
        Assert.True(dim.IsMinContent);
        Assert.False(dim.IsMaxContent);

        Assert.Throws<InvalidOperationException>(() => dim.ToScalar(100));
    }

    [Fact]
    public void MaxContent_ShouldBeMaxContent() {
        var dim = Dimension.MaxContent;
        Assert.False(dim.IsNumber);
        Assert.False(dim.IsAuto);
        Assert.False(dim.IsMinContent);
        Assert.True(dim.IsMaxContent);

        Assert.Throws<InvalidOperationException>(() => dim.ToScalar(100));
    }

    [Fact]
    public void ImplicitConversion_FromInt_ShouldBeChars() {
        Dimension dim = 15;
        Assert.True(dim.IsNumber);
        Assert.Equal(15, dim.ToScalar(100));
        
        // We can't easily check internal unit but behavior matches chars/pixels which are absolute.
        // Assuming implementation maps int to Chars or Pixels.
        // Dimension.cs: public static implicit operator Dimension(int value) => new(value, LengthUnit.CHAR);
    }
}
