//
// NeoKolors.Test
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Tests;

public class SizeTests {
    
    [Fact]
    public void Constructor_ShouldClampNegativeValuesToZero() {
        var size = new Size(-5, 10);
        Assert.Equal(0, size.Width);
        Assert.Equal(10, size.Height);

        var size2 = new Size(10, -5);
        Assert.Equal(10, size2.Width);
        Assert.Equal(0, size2.Height);
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly() {
        var size = new Size(10, 20);
        Assert.Equal(10, size.Width);
        Assert.Equal(20, size.Height);
    }

    [Fact]
    public void Equality_ShouldWork() {
        var s1 = new Size(10, 20);
        var s2 = new Size(10, 20);
        var s3 = new Size(1, 1);
        
        Assert.Equal(s1, s2);
        Assert.NotEqual(s1, s3);
        Assert.True(s1 == s2);
        Assert.True(s1 != s3);
    }

    [Fact]
    public void Addition_ShouldAddDimensions() {
        var s1 = new Size(10, 20);
        var s2 = new Size(5, 5);
        var result = s1 + s2;
        
        Assert.Equal(15, result.Width);
        Assert.Equal(25, result.Height);
    }
    
    [Fact]
    public void Addition_WithPoint_ShouldReturnRectangle() {
        var size = new Size(10, 10);
        var point = new Point(5, 5);
        
        // Size + Point => Rectangle
        Rectangle rect = size + point;
        
        Assert.Equal(5, rect.LowerX);
        Assert.Equal(5, rect.LowerY);
        // Width 10 means from 5 to 5+10-1 = 14
        Assert.Equal(14, rect.HigherX);
        Assert.Equal(14, rect.HigherY);
        Assert.Equal(10, rect.Width);
        Assert.Equal(10, rect.Height);
    }

    [Fact]
    public void Clamp_ShouldClampValues() {
        var size = new Size(50, 50);
        var min = new Size(10, 10);
        var max = new Size(40, 40);
        
        var clamped = size.Clamp(min, max);
        
        Assert.Equal(40, clamped.Width);
        Assert.Equal(40, clamped.Height);
        
        var size2 = new Size(5, 5);
        var clamped2 = size2.Clamp(min, max);
        Assert.Equal(10, clamped2.Width);
        Assert.Equal(10, clamped2.Height);
    }
    
    [Fact]
    public void Max_ShouldReturnMaxDimensions() {
        var s1 = new Size(10, 50);
        var s2 = new Size(50, 10);
        
        var max = Size.Max(s1, s2);
        Assert.Equal(50, max.Width);
        Assert.Equal(50, max.Height);
    }
    
    [Fact]
    public void Min_ShouldReturnMinDimensions() {
        var s1 = new Size(10, 50);
        var s2 = new Size(50, 10);
        
        var min = Size.Min(s1, s2);
        Assert.Equal(10, min.Width);
        Assert.Equal(10, min.Height);
    }
}
