//
// NeoKolors.Test
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Tests;

public class SizeFTests {
    
    [Fact]
    public void Constructor_ShouldClampNegativeValuesToZero() {
        var size = new SizeF(-5.5f, 10.5f);
        Assert.Equal(0, size.Width);
        Assert.Equal(10.5f, size.Height);

        var size2 = new SizeF(10.5f, -5.5f);
        Assert.Equal(10.5f, size2.Width);
        Assert.Equal(0, size2.Height);
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly() {
        var size = new SizeF(10.5f, 20.5f);
        Assert.Equal(10.5f, size.Width);
        Assert.Equal(20.5f, size.Height);
    }

    [Fact]
    public void Equality_ShouldWork() {
        var s1 = new SizeF(10.5f, 20.5f);
        var s2 = new SizeF(10.5f, 20.5f);
        var s3 = new SizeF(1.1f, 1.1f);
        
        Assert.Equal(s1, s2);
        Assert.NotEqual(s1, s3);
        Assert.True(s1 == s2);
        Assert.True(s1 != s3);
    }

    [Fact]
    public void Addition_ShouldAddDimensions() {
        var s1 = new SizeF(10.5f, 20.5f);
        var s2 = new SizeF(5.5f, 5.5f);
        var result = s1 + s2;
        
        Assert.Equal(16.0f, result.Width);
        Assert.Equal(26.0f, result.Height);
    }

    [Fact]
    public void Clamp_ShouldClampValues() {
        var size = new SizeF(50.5f, 50.5f);
        var min = new SizeF(10.0f, 10.0f);
        var max = new SizeF(40.0f, 40.0f);
        
        var clamped = size.Clamp(min, max);
        
        Assert.Equal(40.0f, clamped.Width);
        Assert.Equal(40.0f, clamped.Height);
    }
    
    [Fact]
    public void Max_ShouldReturnMaxDimensions() {
        var s1 = new SizeF(10.5f, 50.5f);
        var s2 = new SizeF(50.5f, 10.5f);
        
        var max = SizeF.Max(s1, s2);
        Assert.Equal(50.5f, max.Width);
        Assert.Equal(50.5f, max.Height);
    }
    
    [Fact]
    public void Min_ShouldReturnMinDimensions() {
        var s1 = new SizeF(10.5f, 50.5f);
        var s2 = new SizeF(50.5f, 10.5f);
        
        var min = SizeF.Min(s1, s2);
        Assert.Equal(10.5f, min.Width);
        Assert.Equal(10.5f, min.Height);
    }
    
    [Fact]
    public void Constants_ShouldBeCorrect() {
        Assert.Equal(0, SizeF.Zero.Width);
        Assert.Equal(0, SizeF.Zero.Height);
        Assert.Equal(1, SizeF.One.Width);
        Assert.Equal(1, SizeF.One.Height);
        Assert.Equal(2, SizeF.Two.Width);
        Assert.Equal(2, SizeF.Two.Height);
    }
}
