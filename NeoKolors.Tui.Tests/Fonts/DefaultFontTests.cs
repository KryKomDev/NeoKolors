//
// NeoKolors.Test
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Fonts;

namespace NeoKolors.Tui.Tests;

public class DefaultFontTests {

    [Fact]
    public void PlaceString_ShouldPlaceText() {
        var font = new DefaultFont();
        var canvas = new NKCharCanvas(10, 5);
        
        font.PlaceString("Hello", canvas);
        
        Assert.Equal('H', canvas[0,0].Char);
        Assert.Equal('e', canvas[1,0].Char);
        Assert.Equal('o', canvas[4,0].Char);
    }

    [Fact]
    public void PlaceString_WithNewlines_ShouldHandleVerticalPlacement() {
        var font = new DefaultFont();
        var canvas = new NKCharCanvas(10, 5);
        
        font.PlaceString("A\nB", canvas);
        
        Assert.Equal('A', canvas[0,0].Char);
        Assert.Equal('B', canvas[0,1].Char);
    }
    
    [Fact]
    public void GetSize_ShouldReturnCorrectSize() {
        var font = new DefaultFont();
        var size = font.GetSize("ABC");
        Assert.Equal(3, size.Width);
        Assert.Equal(1, size.Height);
        
        var minSize = font.GetMinSize("A\nBC");
        Assert.Equal(2, minSize.Width); // max line length is 2 ('BC')
        Assert.Equal(2, minSize.Height); // 2 lines
    }
}
