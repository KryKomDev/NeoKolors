//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using SkiaSharp;

namespace NeoKolors.Common.Tests;

public class ColorFormatTests {
    
    [Fact]
    public void TestColorToSkiaConversion() {
        var color = System.Drawing.Color.FromArgb(255, 100, 150, 200);
        var skColor = color.ColorToSkia();
        
        Assert.Equal(100, skColor.Red);
        Assert.Equal(150, skColor.Green);
        Assert.Equal(200, skColor.Blue);
    }

    [Fact]
    public void TestSkiaToColorConversion() {
        var skColor = new SKColor(100, 150, 200);
        var color = skColor.SkiaToColor();
        
        Assert.Equal(100, color.R);
        Assert.Equal(150, color.G);
        Assert.Equal(200, color.B);
    }

    [Fact]
    public void TestIntToSkiaConversion() {
        int colorInt = (255 << 24) | (100 << 16) | (200 << 8) | 255; // ARGB
        var skColor = colorInt.IntToSkia(false);
        
        Assert.Equal(255, skColor.Alpha);
        Assert.Equal(100, skColor.Red);
        Assert.Equal(200, skColor.Green);
        Assert.Equal(255, skColor.Blue);
    }

    [Fact]
    public void TestHsvToSkiaConversion() {
        // Red color in HSV
        var skColor = ColorFormat.HsvToSkia(0, 1, 1);
        
        Assert.Equal(255, skColor.Red);
        Assert.Equal(0, skColor.Green);
        Assert.Equal(0, skColor.Blue);
    }

    [Fact]
    public void TestSkiaToHsvConversion() {
        var skColor = new SKColor(255, 0, 0); // Red
        ColorFormat.SkiaToHsv(skColor, out double h, out double s, out double v);
        
        Assert.Equal(0, h);
        Assert.Equal(1, s);
        Assert.Equal(1, v);
    }

    [Fact]
    public void TestHsvToColorConversion() {
        // Blue color in HSV
        var color = ColorFormat.HsvToColor(240, 1, 1);
        
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TestColorToHsvConversion() {
        var color = System.Drawing.Color.FromArgb(0, 255, 0); // Green
        var (h, s, v) = color.ColorToHsv();
        
        Assert.Equal(120, h);
        Assert.Equal(1, s);
        Assert.Equal(1, v);
    }

    [Fact]
    public void TestHsvToIntConversion() {
        int colorInt = ColorFormat.HsvToInt(0, 1, 1); // Red
        Assert.Equal(0xFF0000, colorInt);
    }

    [Fact]
    public void TestIntToColorConversion() {
        int colorInt = (255 << 24) | (100 << 16) | (200 << 8) | 255; // ARGB
        var color = colorInt.IntToColor();
        
        Assert.Equal(255, color.A);
        Assert.Equal(100, color.R);
        Assert.Equal(200, color.G);
        Assert.Equal(255, color.B);
    } 

    [Fact]
    public void TestColorToIntConversion() {
        var color = System.Drawing.Color.FromArgb(255, 100, 200, 255);
        int colorInt = color.ColorToInt();
        
        Assert.Equal(255, colorInt >>> 24);
        Assert.Equal(100, (colorInt >>> 16) & 0xFF);
        Assert.Equal(200, (colorInt >>> 8) & 0xFF);
        Assert.Equal(255, colorInt & 0xFF);
    }
}