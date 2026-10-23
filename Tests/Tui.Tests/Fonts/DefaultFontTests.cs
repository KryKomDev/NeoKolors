//
// NeoKolors.Test
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Fonts;

namespace NeoKolors.Tui.Tests.Fonts;

public class DefaultFontTests {

    [Fact]
    public void DrawString_ShouldPlaceText_OnCharCanvas() {
        var font = new DefaultFont();
        using var canvas = new NKCharCanvas(10, 5);
        
        font.DrawString("Hello", canvas);
        
        Assert.Equal('H', canvas.Data[0, 0].Char);
        Assert.Equal('e', canvas.Data[1, 0].Char);
        Assert.Equal('o', canvas.Data[4, 0].Char);
    }

    [Fact]
    public void DrawString_WithNewlines_ShouldHandleVerticalPlacement() {
        var font = new DefaultFont();
        using var canvas = new NKCharCanvas(10, 5);
        
        font.DrawString("A\nB", canvas);
        
        Assert.Equal('A', canvas.Data[0, 0].Char);
        Assert.Equal('B', canvas.Data[0, 1].Char);
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

    [Fact]
    public void DrawString_WithTextRenderingOptions_String_ShouldRenderCorrectly() {
        var font = new DefaultFont();
        using var canvas = new NKCharCanvas(10, 5);
        var options = new TextRenderingOptions(
            horizontalAlign: HorizontalAlign.CENTER,
            verticalAlign: VerticalAlign.CENTER
        );

        font.DrawString("Hi", canvas, new Rect2D(0, 0, 10, 5), options);

        // 10 width, 2 chars -> offset (10 - 2) / 2 = 4
        // 5 height, 1 line -> offset (5 - 1 + 1) / 2 = 2
        Assert.Equal('H', canvas.Data[4, 2].Char);
        Assert.Equal('i', canvas.Data[5, 2].Char);
    }

    [Fact]
    public void DrawString_WithTextRenderingOptions_AnsiString_ShouldRenderAndApplyColoring() {
        var font = new DefaultFont();
        using var canvas = new NKCharCanvas(10, 5);
        var solidColor = NKColor.FromRgb(255, 0, 0);
        var options = new TextRenderingOptions(
            horizontalAlign: HorizontalAlign.LEFT,
            verticalAlign: VerticalAlign.TOP,
            coloring: ColoringStrategies.Solid(solidColor)
        );

        var ansi = new AnsiString("OK");
        font.DrawString(ansi, canvas, new Rect2D(0, 0, 10, 5), options);

        Assert.Equal('O', canvas.Data[0, 0].Char);
        Assert.Equal('K', canvas.Data[1, 0].Char);
        Assert.Equal(solidColor, canvas.Data[0, 0].Style.FColor);
        Assert.Equal(solidColor, canvas.Data[1, 0].Style.FColor);
    }

    [Fact]
    public void DrawString_WithTextRenderingOptions_WithoutBounds_ShouldUseDrawerSize() {
        var font = new DefaultFont();
        using var canvas = new NKCharCanvas(10, 5);
        var options = new TextRenderingOptions(
            horizontalAlign: HorizontalAlign.RIGHT,
            verticalAlign: VerticalAlign.TOP
        );

        font.DrawString("AB", canvas, options);

        // Right aligned on width 10: 10 - 2 = 8
        Assert.Equal('A', canvas.Data[8, 0].Char);
        Assert.Equal('B', canvas.Data[9, 0].Char);
    }

    [Fact]
    public void DrawString_WithWordWrapHard_ShouldWrapCharacters() {
        var font = new DefaultFont();
        using var canvas = new NKCharCanvas(4, 5);
        var options = new TextRenderingOptions(
            wordWrap: WordWrapType.HARD
        );

        font.DrawString("123456", canvas, new Rect2D(0, 0, 4, 5), options);

        // Line 0: "1234", Line 1: "56"
        Assert.Equal('1', canvas.Data[0, 0].Char);
        Assert.Equal('4', canvas.Data[3, 0].Char);
        Assert.Equal('5', canvas.Data[0, 1].Char);
        Assert.Equal('6', canvas.Data[1, 1].Char);
    }

    [Fact]
    public void DrawString_WithLeading_ShouldApplyVerticalLineSpacing() {
        var font = new DefaultFont();
        using var canvas = new NKCharCanvas(10, 10);
        var options = new TextRenderingOptions(
            spacing: new FontSpacingInfo(leading: 3)
        );

        font.DrawString("A\nB", canvas, new Rect2D(0, 0, 10, 10), options);

        Assert.Equal('A', canvas.Data[0, 0].Char);
        Assert.Equal('B', canvas.Data[0, 3].Char);
        Assert.Equal(' ', canvas.Data[0, 1].Char);
        Assert.Equal(' ', canvas.Data[0, 2].Char);
    }

    [Fact]
    public void DrawString_WithLetterSpacing_ShouldAddGapsBetweenChars() {
        var font = new DefaultFont();
        using var canvas = new NKCharCanvas(10, 5);
        var options = new TextRenderingOptions(
            spacing: new FontSpacingInfo(letterSpacing: 2)
        );

        font.DrawString("AB", canvas, new Rect2D(0, 0, 10, 5), options);

        // 'A' at 0, gap of 2, 'B' at 3
        Assert.Equal('A', canvas.Data[0, 0].Char);
        Assert.Equal(' ', canvas.Data[1, 0].Char);
        Assert.Equal(' ', canvas.Data[2, 0].Char);
        Assert.Equal('B', canvas.Data[3, 0].Char);
    }

    [Fact]
    public void DrawString_WithWordSpacing_ShouldExpandSpaces() {
        var font = new DefaultFont();
        using var canvas = new NKCharCanvas(10, 5);
        var options = new TextRenderingOptions(
            spacing: new FontSpacingInfo(wordSpacing: 2)
        );

        font.DrawString("A B", canvas, new Rect2D(0, 0, 10, 5), options);

        // 'A' at 0, space advance = 1 + 2 = 3 columns, 'B' at 4
        Assert.Equal('A', canvas.Data[0, 0].Char);
        Assert.Equal(' ', canvas.Data[1, 0].Char);
        Assert.Equal(' ', canvas.Data[2, 0].Char);
        Assert.Equal(' ', canvas.Data[3, 0].Char);
        Assert.Equal('B', canvas.Data[4, 0].Char);
    }

    [Fact]
    public void DrawString_WithJustifyText_SingleLine_ShouldDistributeSpaces() {
        var font = new DefaultFont();
        using var canvas = new NKCharCanvas(9, 5);
        var options = new TextRenderingOptions(
            justifyText: true
        );

        font.DrawString("A B C", canvas, new Rect2D(0, 0, 9, 5), options);

        // Target width: 9. Chars: 'A', 'B', 'C' (len 3). 6 spaces over 2 gaps = 3 spaces per gap
        // "A   B   C": indices 0, 4, 8
        Assert.Equal('A', canvas.Data[0, 0].Char);
        Assert.Equal('B', canvas.Data[4, 0].Char);
        Assert.Equal('C', canvas.Data[8, 0].Char);
    }

    [Fact]
    public void GetSize_WithTextRenderingOptions_ShouldReflectSpacingAndWrap() {
        var font = new DefaultFont();
        var options = new TextRenderingOptions(
            wordWrap: WordWrapType.HARD,
            spacing: new FontSpacingInfo(leading: 2, letterSpacing: 1)
        );

        // "ABCD" -> with letterSpacing 1: "A B C D" -> length 7
        // Wrapped at maxWidth 4:
        // Line 0: "AB" -> 'A', 'B' -> length 1 + 1 + 1 = 3 <= 4
        // Line 1: "CD" -> 'C', 'D' -> length 3 <= 4
        // Height: 2 lines with leading 2 = (2 - 1) * 2 + 1 = 3
        var size = font.GetSize(new AnsiString("ABCD"), options, maxWidth: 4);

        Assert.Equal(3, size.X);
        Assert.Equal(3, size.Y);
    }

    [Fact]
    public void GetSize_WithNewlines_ShouldComputeCorrectWidthAndHeight() {
        var font = new DefaultFont();

        // 1. Multi-line with varying line lengths
        var size1 = font.GetSize("Line 1\nLine 222\nL3");
        Assert.Equal(8, size1.X); // "Line 222" is length 8
        Assert.Equal(3, size1.Y); // 3 lines

        // 2. CRLF newlines
        var size2 = font.GetSize("Hello\r\nWorld!");
        Assert.Equal(6, size2.X); // "World!" is length 6
        Assert.Equal(2, size2.Y); // 2 lines

        // 3. Trailing newline
        var size3 = font.GetSize("Hello\n");
        Assert.Equal(5, size3.X);
        Assert.Equal(2, size3.Y);

        // 4. Consecutive newlines (empty lines)
        var size4 = font.GetSize("A\n\nB");
        Assert.Equal(1, size4.X);
        Assert.Equal(3, size4.Y);

        // 5. Empty / null string
        Assert.Equal(Size2D.Zero, font.GetSize(""));
        Assert.Equal(Size2D.Zero, font.GetSize((string)null!));

        // 6. AnsiString overload
        var sizeAnsi = font.GetSize(new AnsiString("Alpha\nBeta\nGamma"));
        Assert.Equal(5, sizeAnsi.X);
        Assert.Equal(3, sizeAnsi.Y);
    }

    [Fact]
    public void GetSize_WithMaxWidthAndNewlines_ShouldWrapAndAccountForNewlines() {
        var font = new DefaultFont();

        // "12345 67890\nABC" with maxWidth 5:
        // Line 1: "12345" (5)
        // Line 2: "67890" (5)
        // Line 3: "ABC" (3)
        var size = font.GetSize("12345 67890\nABC", maxWidth: 5);
        Assert.Equal(5, size.X);
        Assert.Equal(3, size.Y);

        // Empty string with maxWidth should return Zero
        Assert.Equal(Size2D.Zero, font.GetSize("", maxWidth: 10));
    }

    [Fact]
    public void DrawString_AnsiString_WithNewlinesAndMaxWidth_ShouldRenderCorrectlyWithoutDesync() {
        var font = new DefaultFont();
        using var canvas = new NKCharCanvas(10, 5);

        var ansi = new AnsiString("AB\nCD");
        font.DrawString(ansi, canvas, maxWidth: 10);

        Assert.Equal('A', canvas.Data[0, 0].Char);
        Assert.Equal('B', canvas.Data[1, 0].Char);
        Assert.Equal('C', canvas.Data[0, 1].Char);
        Assert.Equal('D', canvas.Data[1, 1].Char);
    }

    [Fact]
    public void DrawString_WithBounds_BottomAlignment_ShouldRenderAtBottom() {
        var font = new DefaultFont();
        using var canvas = new NKCharCanvas(10, 5);

        // Single line in 5-high bounds, bottom aligned: row 4
        font.DrawString(
            new AnsiString("Bottom"),
            canvas,
            new Rect2D(0, 0, 10, 5),
            horizontalAlign: HorizontalAlign.LEFT,
            verticalAlign: VerticalAlign.BOTTOM
        );

        Assert.Equal('B', canvas.Data[0, 4].Char);
        Assert.Equal('o', canvas.Data[1, 4].Char);
        Assert.Equal('m', canvas.Data[5, 4].Char);
    }

    [Fact]
    public void DrawString_WithBounds_MultiLine_ShouldRenderAllLines() {
        var font = new DefaultFont();
        using var canvas = new NKCharCanvas(10, 5);

        font.DrawString(
            new AnsiString("Row1\nRow2\nRow3"),
            canvas,
            new Rect2D(0, 0, 10, 5),
            horizontalAlign: HorizontalAlign.LEFT,
            verticalAlign: VerticalAlign.TOP
        );

        Assert.Equal('R', canvas.Data[0, 0].Char);
        Assert.Equal('1', canvas.Data[3, 0].Char);
        Assert.Equal('R', canvas.Data[0, 1].Char);
        Assert.Equal('2', canvas.Data[3, 1].Char);
        Assert.Equal('R', canvas.Data[0, 2].Char);
        Assert.Equal('3', canvas.Data[3, 2].Char);
    }
}

