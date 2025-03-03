using NeoKolors.Common;

namespace NeoKolors.Test.Common;

public class StringEffectsTests {

    [Fact]
    public void AddColor_Int_ShouldReturnCorrectValue() {
        int color = System.Drawing.Color.FromArgb(100, 200, 255).ColorToInt();
        Assert.Equal("\e[38;2;100;200;255mHello\e[38;1;m", "Hello".AddColor(color));
    }

    [Fact]
    public void AddColor_SystemDrawingColor_ShouldReturnCorrectValue() {
        var color = System.Drawing.Color.FromArgb(100, 200, 255);
        Assert.Equal("\e[38;2;100;200;255mHello\e[38;1;m", "Hello".AddColor(color));
    }
    
    [Fact]
    public void AddColor_Color_Custom_ShouldReturnCorrectValue() {
        var color = new Color(System.Drawing.Color.FromArgb(100, 200, 255).ColorToInt());
        Assert.Equal("\e[38;2;100;200;255mHello\e[38;1;m", "Hello".AddColor(color));
    }
    
    [Fact]
    public void AddColor_ConsoleColor_ShouldReturnCorrectValue() {
        var color = ConsoleColor.Cyan;
        Assert.Equal("\e[38;5;14mHello\e[38;5;7m", "Hello".AddColor(color));
    }
    
    [Fact]
    public void AddColor_Color_Palette_ShouldReturnCorrectValue() {
        var color = new Color(ConsoleColor.Cyan);
        Assert.Equal("\e[38;5;14mHello\e[38;5;7m", "Hello".AddColor(color));
    }
    
    [Fact]
    public void AddColorB_Int_ShouldReturnCorrectValue() {
        int color = System.Drawing.Color.FromArgb(100, 200, 255).ColorToInt();
        Assert.Equal("\e[48;2;100;200;255mHello\e[48;1;m", "Hello".AddColorB(color));
    }

    [Fact]
    public void AddColorB_SystemDrawingColor_ShouldReturnCorrectValue() {
        var color = System.Drawing.Color.FromArgb(100, 200, 255);
        Assert.Equal("\e[48;2;100;200;255mHello\e[48;1;m", "Hello".AddColorB(color));
    }
    
    [Fact]
    public void AddColorB_Color_Custom_ShouldReturnCorrectValue() {
        var color = new Color(System.Drawing.Color.FromArgb(100, 200, 255).ColorToInt());
        Assert.Equal("\e[48;2;100;200;255mHello\e[48;1;m", "Hello".AddColorB(color));
    }
    
    [Fact]
    public void AddColorB_ConsoleColor_ShouldReturnCorrectValue() {
        var color = ConsoleColor.Cyan;
        Assert.Equal("\e[48;5;14mHello\e[48;5;7m", "Hello".AddColorB(color));
    }
    
    [Fact]
    public void AddColorB_Color_Palette_ShouldReturnCorrectValue() {
        var color = new Color(ConsoleColor.Cyan);
        Assert.Equal("\e[48;5;14mHello\e[48;5;7m", "Hello".AddColorB(color));
    }

    [Fact]
    public void VisibleLength_ShouldReturnCorrectValue() {
        var s = "\e[38;2;1;1;1mHello\e[38;1;m World\b";
        Assert.Equal(12, s.VisibleLength());
    }
}