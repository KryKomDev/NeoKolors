//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Common.Tests;

public class StringEffectsTests {

    [Fact]
    public void AddColor_Int_ShouldReturnCorrectValue() {
        int color = System.Drawing.Color.FromArgb(100, 200, 255).ColorToInt();
        Assert.Equal("\e[38;2;100;200;255mHello\e[39m", "Hello".AddColor(color));
    }

    [Fact]
    public void AddColor_SystemDrawingColor_ShouldReturnCorrectValue() {
        var color = System.Drawing.Color.FromArgb(100, 200, 255);
        Assert.Equal("\e[38;2;100;200;255mHello\e[39m", "Hello".AddColor(color));
    }
    
    [Fact]
    public void AddColor_Color_Custom_ShouldReturnCorrectValue() {
        var color = new NKColor(System.Drawing.Color.FromArgb(100, 200, 255).ColorToInt());
        Assert.Equal("\e[38;2;100;200;255mHello\e[39m", "Hello".AddColor(color));
    }
    
    [Fact]
    public void AddColor_ConsoleColor_ShouldReturnCorrectValue() {
        var color = NKConsoleColor.CYAN;
        Assert.Equal("\e[38;5;14mHello\e[39m", "Hello".AddColor(color));
    }
    
    [Fact]
    public void AddColor_Color_Palette_ShouldReturnCorrectValue() {
        var color = new NKColor(NKConsoleColor.CYAN);
        Assert.Equal("\e[38;5;14mHello\e[39m", "Hello".AddColor(color));
    }
    
    [Fact]
    public void AddColorB_Int_ShouldReturnCorrectValue() {
        int color = System.Drawing.Color.FromArgb(100, 200, 255).ColorToInt();
        Assert.Equal("\e[48;2;100;200;255mHello\e[49m", "Hello".AddColorB(color));
    }

    [Fact]
    public void AddColorB_SystemDrawingColor_ShouldReturnCorrectValue() {
        var color = System.Drawing.Color.FromArgb(100, 200, 255);
        Assert.Equal("\e[48;2;100;200;255mHello\e[49m", "Hello".AddColorB(color));
    }
    
    [Fact]
    public void AddColorB_Color_Custom_ShouldReturnCorrectValue() {
        var color = new NKColor(System.Drawing.Color.FromArgb(100, 200, 255).ColorToInt());
        Assert.Equal("\e[48;2;100;200;255mHello\e[49m", "Hello".AddColorB(color));
    }
    
    [Fact]
    public void AddColorB_ConsoleColor_ShouldReturnCorrectValue() {
        var color = NKConsoleColor.CYAN;
        Assert.Equal("\e[48;5;14mHello\e[49m", "Hello".AddColorB(color));
    }
    
    [Fact]
    public void AddColorB_Color_Palette_ShouldReturnCorrectValue() {
        var color = new NKColor(NKConsoleColor.CYAN);
        Assert.Equal("\e[48;5;14mHello\e[49m", "Hello".AddColorB(color));
    }

    [Fact]
    public void VisibleLength_ShouldReturnCorrectValue() {
        var s = "\e[38;2;1;1;1mHello\e[39m World\b";
        Assert.Equal(12, s.VisibleLength());
    }

    [Fact]
    public void Capitalize_ShouldReturnCorrectValue() {
        var s = "hello, world!";
        Assert.Equal("Hello, World!", s.Capitalize());
        s = "";
        Assert.Equal("", s.Capitalize());
        s = null;
        Assert.Throws<ArgumentNullException>(() => s!.Capitalize());
    }

    [Fact]
    public void CapitalizeFirst_ShouldReturnCorrectValue() {
        var s = "hello, world!";
        Assert.Equal("Hello, world!", s.CapitalizeFirst());
        s = "";
        Assert.Equal("", s.CapitalizeFirst());
        s = null;
        Assert.Throws<ArgumentNullException>(() => s!.CapitalizeFirst());
    }
}