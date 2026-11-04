// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Core.Tests;

public class DimensionTests {

    [Fact]
    public void Fixed_Max_ToSize_DoesNotOverflow_AndReturnsIntMax() {
        var screen = new Size2D(120, 40);
        var size = Dimension.Fixed.Max.ToSize(100, screen);
        Assert.Equal(int.MaxValue, size);
    }

    [Fact]
    public void GrowTrack_WithDefaultMax_ResolvesToParentWidth() {
        var screen = new Size2D(120, 40);
        var dim = Dimension.GrowTrack();
        var resolved = dim.Resolve(100, screen, contentSize: 15);
        Assert.Equal(100, resolved);
    }

    [Fact]
    public void Auto_ResolvesToParentWidth() {
        var screen = new Size2D(120, 40);
        var dim = Dimension.Auto;
        var resolved = dim.Resolve(100, screen, contentSize: 15);
        Assert.Equal(100, resolved);
    }

    [Fact]
    public void FitContent_WithDefaultMax_ResolvesToContentSize() {
        var screen = new Size2D(120, 40);
        var dim = Dimension.FitContent();
        var resolved = dim.Resolve(100, screen, contentSize: 15);
        Assert.Equal(15, resolved);
    }

    [Fact]
    public void Fixed_ToSize_CalculatesUnitsCorrectly() {
        var screen = new Size2D(200, 100);

        Assert.Equal(42, Dimension.Chars(42).Resolve(100, screen));
        Assert.Equal(50, Dimension.Percent(50).Resolve(100, screen));
        Assert.Equal(100, Dimension.Vw(50).Resolve(100, screen));
        Assert.Equal(50, Dimension.Vh(50).Resolve(100, screen));
    }
}
