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
    public void ApplyStyles_ReplacesTagsWithAnsi() {
        var input = "<b>Bold</b><i>Italic</i><u>Underline</u>";
        var result = input.ApplyStyles();

        Assert.Contains(EscapeCodes.BOLD_START, result);
        Assert.Contains(EscapeCodes.BOLD_END, result);
        Assert.Contains(EscapeCodes.ITALIC_START, result);
        Assert.Contains(EscapeCodes.ITALIC_END, result);
        Assert.Contains(EscapeCodes.UNDERLINE_START, result);
        Assert.Contains(EscapeCodes.UNDERLINE_END, result);
        Assert.Contains("Bold", result);
        Assert.Contains("Italic", result);
        Assert.Contains("Underline", result);
    }

    [Fact]
    public void ApplyColors_ReplacesHexTagsWithAnsi() {
        var input = "<f#ff0000>Red</f#><b#00ff00>Green Bg</b#>";
        var result = input.ApplyColors();

        Assert.Contains("\e[38;2;255;0;0m", result);
        Assert.Contains("\e[48;2;0;255;0m", result);
        Assert.Contains("Red", result);
        Assert.Contains("Green Bg", result);
        Assert.Contains(EscapeCodes.TEXT_COLOR_RESET, result);
        Assert.Contains(EscapeCodes.BCKG_COLOR_RESET, result);
    }

    [Fact]
    public void ApplyColors_ReplacesPaletteTagsWithAnsi() {
        var input = "<f-red>Red</f-color><b-blue>Blue Bg</b-color>";
        var result = input.ApplyColors();

        Assert.Contains(EscapeCodes.PALETTE_COLOR_RED, result);
        Assert.Contains(EscapeCodes.PALETTE_BCKG_COLOR_BLUE, result);
        Assert.Contains("Red", result);
        Assert.Contains("Blue Bg", result);
    }

    [Fact]
    public void ApplyEffects_AppliesBothStylesAndColors() {
        var input = "<b><f#ff0000>Bold Red</f#></b>";
        var result = input.ApplyEffects();

        Assert.Contains(EscapeCodes.BOLD_START, result);
        Assert.Contains("\e[38;2;255;0;0m", result);
        Assert.Contains("Bold Red", result);
    }

    [Fact]
    public void AddStyle_AppliesSingleStyle() {
        var input = "Hello";
        var result = input.AddStyle(TextStyles.BOLD);

        Assert.Equal($"{EscapeCodes.BOLD_START}Hello{EscapeCodes.BOLD_END}", result);
    }

    [Fact]
    public void AddCStyle_AppliesStyleWithoutNegativeReset() {
        var style = new NKStyle(NKConsoleColor.RED, NKConsoleColor.BLACK, TextStyles.BOLD);
        var input = "Hello";
        var result = input.AddCStyle(style);

        // AddCStyle uses AddCStyle(TextStyles), AddCColorF, AddCColorB
        // It should NOT contain negative resets (like [22m for bold) but just the start sequences.
        Assert.Contains(EscapeCodes.BOLD_START, result);
        Assert.Contains(EscapeCodes.PALETTE_COLOR_RED, result);
        Assert.Contains(EscapeCodes.PALETTE_BCKG_COLOR_BLACK, result);
        Assert.DoesNotContain(EscapeCodes.BOLD_END, result);
    }
}