// 
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Common.Tests;

public class NKStyleTests {
    
    [Fact]
    public void TestDefaultConstructor() {
        var style = new NKStyle();
        Assert.True(style.IsFColorDefault);
        Assert.True(style.IsBColorDefault);
        Assert.Equal(TextStyles.NONE, style.Styles);
    }

    [Fact]
    public void TestSetFColorCustom() {
        var style = new NKStyle();
        var color = new NKColor(0x123456);
        style.SetFColor(color);
        Assert.Equal(color, style.FColor);
        Assert.True(style.IsFColorCustom);
    }

    [Fact]
    public void TestSetBColorCustom() {
        var style = new NKStyle();
        var color = new NKColor(0x654321);
        style.SetBColor(color);
        Assert.Equal(color, style.BColor);
        Assert.True(style.IsBColorCustom);
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
        Assert.Equal(color, style.BColor);
        Assert.False(style.IsBColorCustom);
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
    public void TestClone() {
        var style = new NKStyle(new NKColor(0x123456), new NKColor(0x654321), TextStyles.BOLD);
        var clone = (NKStyle)style.Clone();
        Assert.Equal(style, clone);
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
}