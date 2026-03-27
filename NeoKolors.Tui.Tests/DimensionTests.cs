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
    public void Stretch_ShouldBeStretch() {
        var dim = Dimension.Stretch;
        Assert.False(dim.IsNumber);
        Assert.True(dim.IsStretch);

        Assert.Throws<InvalidOperationException>(() => dim.ToScalar(100));
    }

    [Fact]
    public void ImplicitConversion_FromInt_ShouldBeChars() {
        Dimension dim = 15;
        Assert.True(dim.IsNumber);
        Assert.Equal(15, dim.ToScalar(100));
    }

    [Fact]
    public void ToScalarX_Pixel_ShouldApplyPixelWidth() {
        var dim = Dimension.Pixels(10);
        // Default PIXEL_WIDTH is 2 (from UnitDimension.cs)
        Assert.Equal(20, dim.ToScalarX(100));
        Assert.Equal(10, dim.ToScalarY(100));
    }

    [Fact]
    public void Viewport_ShouldReturnBasedOnConsoleSize() {
        try {
            var dimW = Dimension.ViewportWidth(50);
            var dimH = Dimension.ViewportHeight(50);
            
            // We can't know the exact value but it should not throw
            var w = dimW.ToScalar(100);
            var h = dimH.ToScalar(100);
            
            Assert.Equal((int)(System.Console.BufferWidth * 0.5f), w);
            Assert.Equal((int)(System.Console.BufferHeight * 0.5f), h);
        } catch (IOException) {
            // Ignore IOException in headless environments
        }
    }

    [Fact]
    public void Operators_Addition_ShouldCreateExpression() {
        var dim1 = Dimension.Chars(10);
        var dim2 = Dimension.Percent(20);
        
        var expr = dim1 + dim2;
        Assert.Equal(30, expr.ToScalar(100)); // 10 + 20% of 100 = 30
    }

    [Fact]
    public void Operators_Subtraction_ShouldCreateExpression() {
        var dim1 = Dimension.Chars(50);
        var dim2 = Dimension.Chars(10);
        
        var expr = dim1 - dim2;
        Assert.Equal(40, expr.ToScalar(100));
    }

    [Fact]
    public void Parse_BasicUnits_ShouldWork() {
        Assert.True(Dimension.Parse("10px").IsNumber);
        Assert.Equal(10, Dimension.Parse("10px").ToScalar(100));
        
        Assert.Equal(15, Dimension.Parse("15ch").ToScalar(100));
        Assert.Equal(50, Dimension.Parse("50%").ToScalar(100));
        Assert.Equal(Dimension.Auto, Dimension.Parse("auto"));
        Assert.Equal(Dimension.MinContent, Dimension.Parse("min-content"));
        Assert.Equal(Dimension.MaxContent, Dimension.Parse("max-content"));
        Assert.Equal(Dimension.Stretch, Dimension.Parse("stretch"));
    }

    [Fact]
    public void Parse_Expressions_ShouldWork() {
        var dim = Dimension.Parse("10ch + 20%");
        Assert.Equal(30, dim.ToScalar(100));
        
        dim = Dimension.Parse("100% - 10ch");
        Assert.Equal(90, dim.ToScalar(100));
    }

    [Fact]
    public void TryParse_InvalidInput_ShouldReturnFalse() {
        Assert.False(Dimension.TryParse("invalid", null, out _));
        Assert.False(Dimension.TryParse("", null, out _));
        Assert.False(Dimension.TryParse(null, null, out _));
    }

    [Fact]
    public void ToString_ShouldReturnCorrectFormat() {
        Assert.Equal("10 char", Dimension.Chars(10).ToString());
        Assert.Equal("auto", Dimension.Auto.ToString());
        Assert.Equal("min-content", Dimension.MinContent.ToString());
        Assert.Equal("max-content", Dimension.MaxContent.ToString());
        Assert.Equal("stretch", Dimension.Stretch.ToString());
    }
}
