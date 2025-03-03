using NeoKolors.Tui;

namespace NeoKolors.Test.Tui;

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
    public void Width_ShouldReturnCorrectValue() {
        var rect = new Rectangle(2, 2, 5, 5);
        Assert.Equal(3, rect.Width);
    }

    [Fact]
    public void Height_ShouldReturnCorrectValue() {
        var rect = new Rectangle(2, 2, 5, 5);
        Assert.Equal(3, rect.Height);
    }

    [Fact]
    public void Contains_ShouldReturnTrue_WhenPointIsInside() {
        var rect = new Rectangle(2, 2, 5, 5);
        Assert.True(rect.Contains(3, 3));
    }

    [Fact]
    public void Contains_ShouldReturnFalse_WhenPointIsOutside() {
        var rect = new Rectangle(2, 2, 5, 5);
        Assert.False(rect.Contains(6, 6));
    }

    [Fact]
    public void Overlaps_ShouldReturnTrue_WhenRectanglesOverlap() {
        var rect1 = new Rectangle(2, 2, 5, 5);
        var rect2 = new Rectangle(4, 4, 6, 6);
        Assert.True(rect1.Overlaps(rect2));
    }

    [Fact]
    public void Overlaps_ShouldReturnFalse_WhenRectanglesDoNotOverlap() {
        var rect1 = new Rectangle(2, 2, 5, 5);
        var rect2 = new Rectangle(6, 6, 8, 8);
        Assert.False(rect1.Overlaps(rect2));
    }

    [Fact]
    public void LowerX_ShouldSwapValues_WhenSetHigherThanHigherX() {
        var rect = new Rectangle(2, 2, 5, 5);
        rect.LowerX = 6;
        Assert.Equal(5, rect.LowerX);
        Assert.Equal(6, rect.HigherX);
    }

    [Fact]
    public void LowerY_ShouldSwapValues_WhenSetHigherThanHigherY() {
        var rect = new Rectangle(2, 2, 5, 5);
        rect.LowerY = 6;
        Assert.Equal(5, rect.LowerY);
        Assert.Equal(6, rect.HigherY);
    }

    [Fact]
    public void HigherX_ShouldSwapValues_WhenSetLowerThanLowerX() {
        var rect = new Rectangle(2, 2, 5, 5);
        rect.HigherX = 1;
        Assert.Equal(1, rect.LowerX);
        Assert.Equal(2, rect.HigherX);
    }

    [Fact]
    public void HigherY_ShouldSwapValues_WhenSetLowerThanLowerY() {
        var rect = new Rectangle(2, 2, 5, 5);
        rect.HigherY = 1;
        Assert.Equal(1, rect.LowerY);
        Assert.Equal(2, rect.HigherY);
    }
}