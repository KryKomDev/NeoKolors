//
// NeoKolors.Test
// Copyright (c) 2026 KryKom
//

namespace NeoKolors.Tui.Tests;

public class RectangleTests {
    
    [Fact]
    public void Constructor_ShouldInitializeCorrectly() {
        var rect = new Rectangle(2, 2, 5, 5);
        Assert.Equal(2, rect.LowerX);
        Assert.Equal(2, rect.LowerY);
        Assert.Equal(5, rect.HigherX);
        Assert.Equal(5, rect.HigherY);
    }

    [Fact]
    public void Constructor_WithPointAndSize_ShouldInitializeCorrectly() {
        var rect = new Rectangle(new Point(10, 10), new Size(5, 5));
        Assert.Equal(10, rect.LowerX);
        Assert.Equal(10, rect.LowerY);
        Assert.Equal(14, rect.HigherX);
        Assert.Equal(14, rect.HigherY);
        Assert.Equal(5, rect.Width);
        Assert.Equal(5, rect.Height);
    }

    [Fact]
    public void WidthHeight_ShouldBeCorrect() {
        var rect = new Rectangle(2, 2, 5, 5);
        Assert.Equal(4, rect.Width);
        Assert.Equal(4, rect.Height);
    }

    [Fact]
    public void Contains_ShouldWork() {
        var rect = new Rectangle(2, 2, 5, 5);
        Assert.True(rect.Contains(2, 2));
        Assert.True(rect.Contains(5, 5));
        Assert.True(rect.Contains(3, 3));
        Assert.False(rect.Contains(1, 1));
        Assert.False(rect.Contains(6, 6));
    }

    [Fact]
    public void Overlaps_ShouldWork() {
        var rect1 = new Rectangle(0, 0, 10, 10);
        var rect2 = new Rectangle(5, 5, 15, 15);
        var rect3 = new Rectangle(11, 11, 20, 20);
        
        Assert.True(rect1.Overlaps(rect2));
        Assert.True(rect2.Overlaps(rect1));
        Assert.False(rect1.Overlaps(rect3));
    }

    [Fact]
    public void LowerX_Setter_ShouldSwapIfGreater() {
        var rect = new Rectangle(10, 10, 20, 20);
        rect.LowerX = 25;
        Assert.Equal(20, rect.LowerX);
        Assert.Equal(25, rect.HigherX);
    }

    [Fact]
    public void HigherX_Setter_ShouldSwapIfLower() {
        var rect = new Rectangle(10, 10, 20, 20);
        rect.HigherX = 5;
        Assert.Equal(5, rect.LowerX);
        Assert.Equal(10, rect.HigherX);
    }

    [Fact]
    public void Equality_ShouldWork() {
        var r1 = new Rectangle(0, 0, 10, 10);
        var r2 = new Rectangle(0, 0, 10, 10);
        var r3 = new Rectangle(1, 1, 11, 11);
        
        Assert.Equal(r1, r2);
        Assert.NotEqual(r1, r3);
        Assert.True(r1 == r2);
        Assert.True(r1 != r3);
    }

    [Fact]
    public void Operators_Addition_ShouldOffset() {
        var rect = new Rectangle(0, 0, 10, 10);
        var point = new Point(5, 5);
        var result = rect + point;
        
        Assert.Equal(5, result.LowerX);
        Assert.Equal(5, result.LowerY);
        Assert.Equal(15, result.HigherX);
        Assert.Equal(15, result.HigherY);
    }

    [Fact]
    public void Decompose_ShouldReturnLocationAndSize() {
        var rect = new Rectangle(10, 10, 19, 19);
        var (loc, size) = rect.Decompose();
        
        Assert.Equal(10, loc.X);
        Assert.Equal(10, loc.Y);
        Assert.Equal(10, size.Width);
        Assert.Equal(10, size.Height);
    }

    [Fact]
    public void ToString_ShouldReturnExpectedFormat() {
        var rect = new Rectangle(0, 0, 9, 9);
        var str = rect.ToString();
        Assert.Equal("[0, 0]:[9, 9] (10, 10)", str);
    }
}
