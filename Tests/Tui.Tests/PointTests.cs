//
// NeoKolors.Test
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Tests;

public class PointTests {
    
    [Fact]
    public void Constructor_ShouldInitializeCorrectly() {
        var p = new Point(5, 10);
        Assert.Equal(5, p.X);
        Assert.Equal(10, p.Y);
    }

    [Fact]
    public void Zero_ShouldReturnZeroPoint() {
        var p = Point.Zero;
        Assert.Equal(0, p.X);
        Assert.Equal(0, p.Y);
    }

    [Fact]
    public void AdditionOperator_ShouldAddCoordinates() {
        var p1 = new Point(2, 3);
        var p2 = new Point(4, 5);
        var result = p1 + p2;
        Assert.Equal(6, result.X);
        Assert.Equal(8, result.Y);
    }

    [Fact]
    public void SubtractionOperator_ShouldSubtractCoordinates() {
        var p1 = new Point(5, 7);
        var p2 = new Point(2, 3);
        var result = p1 - p2;
        Assert.Equal(3, result.X);
        Assert.Equal(4, result.Y);
    }

    [Fact]
    public void ImplicitConversion_FromTuple_ShouldWork() {
        Point p = (1, 2);
        Assert.Equal(1, p.X);
        Assert.Equal(2, p.Y);
    }

    [Fact]
    public void ImplicitConversion_ToTuple_ShouldWork() {
        var p = new Point(3, 4);
        (int x, int y) = p;
        Assert.Equal(3, x);
        Assert.Equal(4, y);
    }

    [Fact]
    public void Min_ShouldReturnMinimumCoordinates() {
        var p1 = new Point(1, 10);
        var p2 = new Point(5, 2);
        var result = Point.Min(p1, p2);
        Assert.Equal(1, result.X);
        Assert.Equal(2, result.Y);
    }

    [Fact]
    public void Max_ShouldReturnMaximumCoordinates() {
        var p1 = new Point(1, 10);
        var p2 = new Point(5, 2);
        var result = Point.Max(p1, p2);
        Assert.Equal(5, result.X);
        Assert.Equal(10, result.Y);
    }

    [Fact]
    public void Rect_ShouldReturnCorrectSize() {
        var p1 = new Point(2, 2);
        var p2 = new Point(5, 6);
        // Size is inclusive calculation: p2 - p1 + 1
        // Width: 5 - 2 + 1 = 4
        // Height: 6 - 2 + 1 = 5
        var size = Point.Rect(p1, p2);
        Assert.Equal(4, size.Width);
        Assert.Equal(5, size.Height);
    }
}
