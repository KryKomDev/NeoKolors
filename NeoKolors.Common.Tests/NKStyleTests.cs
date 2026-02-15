// 
// NeoKolors//

namespace NeoKolors.Common.Tests;

public class NKStyleTests {
    
    [Fact]
    public void TestDefaultConstructor() {
        var style = new NKStyle();
        Assert.True(style.IsFColorDefault);
        Assert.True(style.IsBColorDefault);
        Assert.Equal(0, (byte)style.Styles);
    }

    [Fact]
    public void TestSetFColorCustom() {
        var style = new NKStyle();
        var color = new NKColor(0x123456);
        style.SetFColor(color);
        Assert.True(style.IsFColorCustom);
        Assert.Equal(color, style.FColor);
    }

    [Fact]
    public void TestSetBColorCustom() {
        var style = new NKStyle();
        var color = new NKColor(0x654321);
        style.SetBColor(color);
        Assert.True(style.IsBColorCustom);
        Assert.Equal(color, style.BColor);
    }
    
    [Fact]
    public void TestSetFColorConsole() {
        var style = new NKStyle();
        var color = new NKColor(NKConsoleColor.RED);
        style.SetFColor(color);
        Assert.Equal(color, style.FColor);
        Assert.False(style.IsFColorCustom);
    }

    [Fact]
    public void TestSetBColorConsole() {
        var style = new NKStyle();
        var color = new NKColor(NKConsoleColor.RED);
        style.SetBColor(color);
        Assert.False(style.IsBColorCustom);
        Assert.Equal(color, style.BColor);
    }
    
    [Fact]
    public void TestSetFColorDefault() {
        var style = new NKStyle();
        var color = new NKColor();
        style.SetFColor(color);
        Assert.Equal(color, style.FColor);
        Assert.False(style.IsFColorCustom);
        Assert.True(style.IsFColorDefault);
    }

    [Fact]
    public void TestSetBColorDefault() {
        var style = new NKStyle();
        var color = new NKColor();
        style.SetBColor(color);
        Assert.Equal(color, style.BColor);
        Assert.False(style.IsBColorCustom);
        Assert.True(style.IsBColorDefault);
    }

    [Fact]
    public void TestSetStyles() {
        var style = new NKStyle();
        style.SetStyles(TextStyles.BOLD | TextStyles.ITALIC);
        Assert.True(style.Styles.HasFlag(TextStyles.BOLD));
        Assert.True(style.Styles.HasFlag(TextStyles.ITALIC));
    }

    [Fact]
    public void TestEquals() {
        var style1 = new NKStyle(new NKColor(0x123456), new NKColor(0x654321), TextStyles.BOLD);
        var style2 = new NKStyle(new NKColor(0x123456), new NKColor(0x654321), TextStyles.BOLD);
        Assert.True(style1.Equals(style2));
        Assert.True(style1 == style2);
    }

    [Fact]
    public void TestNotEquals() {
        var style1 = new NKStyle(new NKColor(0x123456), new NKColor(0x654321), TextStyles.BOLD);
        var style2 = new NKStyle(new NKColor(0x654321), new NKColor(0x123456), TextStyles.ITALIC);
        Assert.False(style1.Equals(style2));
        Assert.False(style1 == style2);
    }
    
    [Theory]
    [InlineData(0xff0000, 0x00ff00)]
    [InlineData(NKConsoleColor.RED, NKConsoleColor.BLUE)]
    public void Constructor_SetsCorrectValues(NKColor f, NKColor b) {
        var styles = TextStyles.BOLD | TextStyles.ITALIC;

        var style = new NKStyle(f, b, styles);

        Assert.Equal(f, style.FColor);
        Assert.Equal(b, style.BColor);
        Assert.Equal(styles, style.Styles);
    }

    // [Fact]
    // public void Constructor_RunUntilFail_Rgb_Rgb() {
    //     var r = new Random();
    //     var f = new NKColor(r.Next(0, 0xffffff));
    //     var b = new NKColor(r.Next(0, 0xffffff));
    //     var s = (TextStyles)r.Next(0, 0xff);
    //     var n = new NKStyle(f, b, s);
    //     Assert.Equal(f, n.FColor);
    //     Assert.Equal(b, n.BColor);
    //     Assert.Equal(s, n.Styles);
    // }
    //
    // [Fact]
    // public void Constructor_RunUntilFail_Rgb_Con() {
    //     var r = new Random();
    //     var f = new NKColor(r.Next(0, 0xffffff));
    //     var b = new NKColor((NKConsoleColor)r.Next(0, 0xff));
    //     var s = (TextStyles)r.Next(0, 0xff);
    //     var n = new NKStyle(f, b, s);
    //     Assert.Equal(f, n.FColor);
    //     Assert.Equal(b, n.BColor);
    //     Assert.Equal(s, n.Styles);
    // }
    //
    // [Fact]
    // public void Constructor_RunUntilFail_Con_Rgb() {
    //     var r = new Random();
    //     var f = new NKColor((NKConsoleColor)r.Next(0, 0xff));
    //     var b = new NKColor(r.Next(0, 0xffffff));
    //     var s = (TextStyles)r.Next(0, 0xff);
    //     var n = new NKStyle(f, b, s);
    //     Assert.Equal(f, n.FColor);
    //     Assert.Equal(b, n.BColor);
    //     Assert.Equal(s, n.Styles);
    // }
    
    [Fact]
    public void Constructor_ShouldInitCorrectly_Default_Inherit() {
        var f = NKColor.Default;
        var b = NKColor.Inherit;
        var s = TextStyles.NONE;
        var n = new NKStyle(f, b, s);
        Assert.Equal(f, n.FColor);
        Assert.Equal(b, n.BColor);
        Assert.Equal(s, n.Styles);
    }
    
    
    [Fact]
    public void SetStyles_ShouldPreserveColors() {
        var f = new NKColor(0x112233);
        var b = new NKColor(0x445566);
        var s = new NKStyle(f, b);
        
        Assert.Equal(f, s.FColor);
        Assert.Equal(b, s.BColor);
        Assert.Equal(TextStyles.NONE, s.Styles);
        
        s.SetStyles(TextStyles.BOLD | TextStyles.UNDERLINE);
        
        Assert.Equal(f, s.FColor);
        Assert.Equal(b, s.BColor);
        Assert.Equal(TextStyles.BOLD | TextStyles.UNDERLINE, s.Styles);
        
        s.SetStyles(TextStyles.NONE);
        Assert.Equal(TextStyles.NONE, s.Styles);
        Assert.Equal(f, s.FColor);
        Assert.Equal(b, s.BColor);
    }

    [Fact]
    public void SetFColor_ShouldPreserveStyles() {
        var s = new NKStyle(s: TextStyles.ITALIC);
        Assert.Equal(TextStyles.ITALIC, s.Styles);
        
        s.SetFColor(new NKColor(0xAABBCC));
        Assert.Equal(TextStyles.ITALIC, s.Styles);
    }

    [Fact]
    public void SetBColor_ShouldPreserveStyles() {
        var s = new NKStyle(s: TextStyles.STRIKETHROUGH);
        Assert.Equal(TextStyles.STRIKETHROUGH, s.Styles);
        
        s.SetBColor(new NKColor(0xDDEEFF));
        Assert.Equal(TextStyles.STRIKETHROUGH, s.Styles);
    }

    [Fact]
    public void SetStyles_Overwrite_ShouldWork() {
        var s = new NKStyle(s: TextStyles.BOLD);
        s.SetStyles(TextStyles.FAINT); // Overwrite, not merge
        Assert.Equal(TextStyles.FAINT, s.Styles);
        Assert.False(s.Styles.HasFlag(TextStyles.BOLD));
    }
}