//
// NeoKolors.Test
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Tests;

public class DefaultFontTests {

    [Fact]
    public void PlaceString_ShouldPlaceText() {
        var font = new DefaultFont();
        var canvas = new NKCharCanvas(10, 5);
        
        font.PlaceString("Hello", canvas);
        
        Assert.Equal('H', canvas[0, 0].Char!.Value);
        Assert.Equal('e', canvas[1, 0].Char!.Value);
        Assert.Equal('o', canvas[4, 0].Char!.Value);
    }

    [Fact]
    public void PlaceString_WithNewlines_ShouldHandleVerticalPlacement() {
        var font = new DefaultFont();
        var canvas = new NKCharCanvas(10, 5);
        
        font.PlaceString("A\nB", canvas);
        
        Assert.Equal('A', canvas[0,0].Char!.Value);
        Assert.Equal('B', canvas[0,1].Char!.Value);
    }
    
    [Fact]
    public void GetSize_ShouldReturnCorrectSize() {
        var font = new DefaultFont();
        var size = font.GetSize("ABC");
        Assert.Equal(3, size.X);
        Assert.Equal(1, size.Y);
        
        var minSize = font.GetMinSize("A\nBC");
        Assert.Equal(2, minSize.X); // max line length is 2 ('BC')
        Assert.Equal(2, minSize.Y); // 2 lines
    }
}
