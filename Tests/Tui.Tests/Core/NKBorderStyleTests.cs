// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Tests.Core;

public class NKBorderStyleTests {

    [Fact]
    public void Constructor_8AnsiChars_AssignsClockwisePerimeter() {
        var tl = new AnsiChar('1', new NKStyle(ConsoleColor.Red));
        var t  = new AnsiChar('2', new NKStyle(ConsoleColor.Green));
        var tr = new AnsiChar('3', new NKStyle(ConsoleColor.Blue));
        var r  = new AnsiChar('4', new NKStyle(ConsoleColor.Yellow));
        var br = new AnsiChar('5', new NKStyle(ConsoleColor.Cyan));
        var b  = new AnsiChar('6', new NKStyle(ConsoleColor.Magenta));
        var bl = new AnsiChar('7', new NKStyle(ConsoleColor.White));
        var l  = new AnsiChar('8', new NKStyle(ConsoleColor.DarkGray));

        var border = new NKBorderStyle(tl, t, tr, r, br, b, bl, l);

        Assert.Equal(tl, border.TopLeft);
        Assert.Equal(t,  border.Top);
        Assert.Equal(tr, border.TopRight);
        Assert.Equal(r,  border.Right);
        Assert.Equal(br, border.BottomRight);
        Assert.Equal(b,  border.Bottom);
        Assert.Equal(bl, border.BottomLeft);
        Assert.Equal(l,  border.Left);
        Assert.False(border.IsBorderless);
    }

    [Fact]
    public void Constructor_8CharsAndStyles_AssignsClockwisePerimeter() {
        var s1 = new NKStyle(ConsoleColor.Red);
        var s2 = new NKStyle(ConsoleColor.Green);
        var s3 = new NKStyle(ConsoleColor.Blue);
        var s4 = new NKStyle(ConsoleColor.Yellow);
        var s5 = new NKStyle(ConsoleColor.Cyan);
        var s6 = new NKStyle(ConsoleColor.Magenta);
        var s7 = new NKStyle(ConsoleColor.White);
        var s8 = new NKStyle(ConsoleColor.DarkGray);

        var border = new NKBorderStyle('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
            s1, s2, s3, s4, s5, s6, s7, s8);

        Assert.Equal('A', border.TopLeft.Char);
        Assert.Equal(s1,  border.TopLeft.Style);

        Assert.Equal('B', border.Top.Char);
        Assert.Equal(s2,  border.Top.Style);

        Assert.Equal('C', border.TopRight.Char);
        Assert.Equal(s3,  border.TopRight.Style);

        Assert.Equal('D', border.Right.Char);
        Assert.Equal(s4,  border.Right.Style);

        Assert.Equal('E', border.BottomRight.Char);
        Assert.Equal(s5,  border.BottomRight.Style);

        Assert.Equal('F', border.Bottom.Char);
        Assert.Equal(s6,  border.Bottom.Style);

        Assert.Equal('G', border.BottomLeft.Char);
        Assert.Equal(s7,  border.BottomLeft.Style);

        Assert.Equal('H', border.Left.Char);
        Assert.Equal(s8,  border.Left.Style);
    }

    [Fact]
    public void Constructor_8CharsSingleStyle_AssignsAllElements() {
        var style = new NKStyle(ConsoleColor.Magenta);
        var border = new NKBorderStyle('1', '2', '3', '4', '5', '6', '7', '8', style);

        Assert.Equal('1', border.TopLeft.Char);
        Assert.Equal('2', border.Top.Char);
        Assert.Equal('3', border.TopRight.Char);
        Assert.Equal('4', border.Right.Char);
        Assert.Equal('5', border.BottomRight.Char);
        Assert.Equal('6', border.Bottom.Char);
        Assert.Equal('7', border.BottomLeft.Char);
        Assert.Equal('8', border.Left.Char);

        Assert.Equal(style, border.TopLeft.Style);
        Assert.Equal(style, border.Top.Style);
        Assert.Equal(style, border.TopRight.Style);
        Assert.Equal(style, border.Right.Style);
        Assert.Equal(style, border.BottomRight.Style);
        Assert.Equal(style, border.Bottom.Style);
        Assert.Equal(style, border.BottomLeft.Style);
        Assert.Equal(style, border.Left.Style);
    }

    [Fact]
    public void Constructor_6CharsAndStyles_AssignsEdgesAndCorners() {
        var vs  = new NKStyle(ConsoleColor.Red);
        var hs  = new NKStyle(ConsoleColor.Green);
        var tls = new NKStyle(ConsoleColor.Blue);
        var trs = new NKStyle(ConsoleColor.Yellow);
        var bls = new NKStyle(ConsoleColor.Cyan);
        var brs = new NKStyle(ConsoleColor.Magenta);

        var border = new NKBorderStyle('|', '-', '1', '2', '3', '4', vs, hs, tls, trs, bls, brs);

        Assert.Equal('1', border.TopLeft.Char);
        Assert.Equal(tls, border.TopLeft.Style);

        Assert.Equal('-', border.Top.Char);
        Assert.Equal(hs,  border.Top.Style);

        Assert.Equal('2', border.TopRight.Char);
        Assert.Equal(trs, border.TopRight.Style);

        Assert.Equal('|', border.Right.Char);
        Assert.Equal(vs,  border.Right.Style);

        Assert.Equal('4', border.BottomRight.Char);
        Assert.Equal(brs, border.BottomRight.Style);

        Assert.Equal('-', border.Bottom.Char);
        Assert.Equal(hs,  border.Bottom.Style);

        Assert.Equal('3', border.BottomLeft.Char);
        Assert.Equal(bls, border.BottomLeft.Style);

        Assert.Equal('|', border.Left.Char);
        Assert.Equal(vs,  border.Left.Style);
    }

    [Fact]
    public void GetNormal_ConstructsExpectedBorder() {
        var border = NKBorderStyle.GetNormal();

        Assert.Equal('│', border.Left.Char);
        Assert.Equal('│', border.Right.Char);
        Assert.Equal('─', border.Top.Char);
        Assert.Equal('─', border.Bottom.Char);
        Assert.Equal('┌', border.TopLeft.Char);
        Assert.Equal('┐', border.TopRight.Char);
        Assert.Equal('└', border.BottomLeft.Char);
        Assert.Equal('┘', border.BottomRight.Char);
    }

    [Fact]
    public void GetInset_StyleOverload_AssignsCorrectShading() {
        var shadow    = new NKStyle(ConsoleColor.DarkGray);
        var highlight = new NKStyle(ConsoleColor.White);

        var border = NKBorderStyle.GetInset(shadow, highlight);

        Assert.Equal('┌', border.TopLeft.Char);
        Assert.Equal(shadow, border.TopLeft.Style);

        Assert.Equal('─', border.Top.Char);
        Assert.Equal(shadow, border.Top.Style);

        Assert.Equal('┐', border.TopRight.Char);
        Assert.Equal(highlight, border.TopRight.Style);

        Assert.Equal('│', border.Right.Char);
        Assert.Equal(highlight, border.Right.Style);

        Assert.Equal('┘', border.BottomRight.Char);
        Assert.Equal(highlight, border.BottomRight.Style);

        Assert.Equal('─', border.Bottom.Char);
        Assert.Equal(highlight, border.Bottom.Style);

        Assert.Equal('└', border.BottomLeft.Char);
        Assert.Equal(shadow, border.BottomLeft.Style);

        Assert.Equal('│', border.Left.Char);
        Assert.Equal(shadow, border.Left.Style);
    }

    [Fact]
    public void GetInset_ColorOverload_AssignsCorrectShading() {
        var shadow    = NKColor.FromRgb(30, 30, 30);
        var highlight = NKColor.FromRgb(200, 200, 200);
        var bckg      = NKColor.FromRgb(10, 10, 10);

        var border = NKBorderStyle.GetInset(shadow, highlight, bckg);

        var expectedShadow    = new NKStyle(shadow, bckg);
        var expectedHighlight = new NKStyle(highlight, bckg);

        Assert.Equal(expectedShadow, border.TopLeft.Style);
        Assert.Equal(expectedShadow, border.Top.Style);
        Assert.Equal(expectedHighlight, border.TopRight.Style);
        Assert.Equal(expectedHighlight, border.Right.Style);
        Assert.Equal(expectedHighlight, border.BottomRight.Style);
        Assert.Equal(expectedHighlight, border.Bottom.Style);
        Assert.Equal(expectedShadow, border.BottomLeft.Style);
        Assert.Equal(expectedShadow, border.Left.Style);
    }

    [Fact]
    public void GetOutset_StyleOverload_AssignsCorrectShading() {
        var shadow    = new NKStyle(ConsoleColor.DarkGray);
        var highlight = new NKStyle(ConsoleColor.White);

        var border = NKBorderStyle.GetOutset(shadow, highlight);

        Assert.Equal('┌', border.TopLeft.Char);
        Assert.Equal(highlight, border.TopLeft.Style);

        Assert.Equal('─', border.Top.Char);
        Assert.Equal(highlight, border.Top.Style);

        Assert.Equal('┐', border.TopRight.Char);
        Assert.Equal(shadow, border.TopRight.Style);

        Assert.Equal('│', border.Right.Char);
        Assert.Equal(shadow, border.Right.Style);

        Assert.Equal('┘', border.BottomRight.Char);
        Assert.Equal(shadow, border.BottomRight.Style);

        Assert.Equal('─', border.Bottom.Char);
        Assert.Equal(shadow, border.Bottom.Style);

        Assert.Equal('└', border.BottomLeft.Char);
        Assert.Equal(highlight, border.BottomLeft.Style);

        Assert.Equal('│', border.Left.Char);
        Assert.Equal(highlight, border.Left.Style);
    }

    [Fact]
    public void GetInsetThick_AssignsThickCharsAndCorrectShading() {
        var shadow    = new NKStyle(ConsoleColor.DarkGray);
        var highlight = new NKStyle(ConsoleColor.White);

        var border = NKBorderStyle.GetInsetThick(shadow, highlight);

        Assert.Equal('┏', border.TopLeft.Char);
        Assert.Equal('━', border.Top.Char);
        Assert.Equal('┓', border.TopRight.Char);
        Assert.Equal('┃', border.Right.Char);
        Assert.Equal('┛', border.BottomRight.Char);
        Assert.Equal('━', border.Bottom.Char);
        Assert.Equal('┗', border.BottomLeft.Char);
        Assert.Equal('┃', border.Left.Char);

        Assert.Equal(shadow,    border.TopLeft.Style);
        Assert.Equal(shadow,    border.Top.Style);
        Assert.Equal(highlight, border.TopRight.Style);
        Assert.Equal(highlight, border.Right.Style);
        Assert.Equal(highlight, border.BottomRight.Style);
        Assert.Equal(highlight, border.Bottom.Style);
        Assert.Equal(shadow,    border.BottomLeft.Style);
        Assert.Equal(shadow,    border.Left.Style);
    }

    [Fact]
    public void GetOutsetThick_AssignsThickCharsAndCorrectShading() {
        var shadow    = new NKStyle(ConsoleColor.DarkGray);
        var highlight = new NKStyle(ConsoleColor.White);

        var border = NKBorderStyle.GetOutsetThick(shadow, highlight);

        Assert.Equal('┏', border.TopLeft.Char);
        Assert.Equal('━', border.Top.Char);
        Assert.Equal('┓', border.TopRight.Char);
        Assert.Equal('┃', border.Right.Char);
        Assert.Equal('┛', border.BottomRight.Char);
        Assert.Equal('━', border.Bottom.Char);
        Assert.Equal('┗', border.BottomLeft.Char);
        Assert.Equal('┃', border.Left.Char);

        Assert.Equal(highlight, border.TopLeft.Style);
        Assert.Equal(highlight, border.Top.Style);
        Assert.Equal(shadow,    border.TopRight.Style);
        Assert.Equal(shadow,    border.Right.Style);
        Assert.Equal(shadow,    border.BottomRight.Style);
        Assert.Equal(shadow,    border.Bottom.Style);
        Assert.Equal(highlight, border.BottomLeft.Style);
        Assert.Equal(highlight, border.Left.Style);
    }

    [Fact]
    public void Draw_WithRect2D_RendersPerimeterAccurately() {
        var border = NKBorderStyle.GetAscii();
        using var canvas = new NKCharCanvas(10, 10);

        // Draw a 5x4 box at (2, 3): columns [2..6], rows [3..6]
        border.Draw(new Rect2D(2, 3, 7, 7), canvas, 0);

        // Corners
        Assert.Equal('+', canvas.Data[2, 3].Char); // TopLeft
        Assert.Equal('+', canvas.Data[6, 3].Char); // TopRight
        Assert.Equal('+', canvas.Data[2, 6].Char); // BottomLeft
        Assert.Equal('+', canvas.Data[6, 6].Char); // BottomRight

        // Top edge
        for (int x = 3; x <= 5; x++) {
            Assert.Equal('-', canvas.Data[x, 3].Char);
        }

        // Bottom edge
        for (int x = 3; x <= 5; x++) {
            Assert.Equal('-', canvas.Data[x, 6].Char);
        }

        // Left edge
        for (int y = 4; y <= 5; y++) {
            Assert.Equal('|', canvas.Data[2, y].Char);
        }

        // Right edge
        for (int y = 4; y <= 5; y++) {
            Assert.Equal('|', canvas.Data[6, y].Char);
        }

        // Inside should remain empty
        for (int y = 4; y <= 5; y++) {
            for (int x = 3; x <= 5; x++) {
                Assert.Equal(' ', canvas.Data[x, y].Char);
            }
        }
    }

    [Fact]
    public void Draw_2x2_RendersFourCornersOnly() {
        var border = NKBorderStyle.GetAscii();
        using var canvas = new NKCharCanvas(5, 5);

        // 2x2 box at (1, 1): columns [1, 2], rows [1, 2]
        border.Draw(new Rect2D(1, 1, 3, 3), canvas, 0);

        Assert.Equal('+', canvas.Data[1, 1].Char);
        Assert.Equal('+', canvas.Data[2, 1].Char);
        Assert.Equal('+', canvas.Data[1, 2].Char);
        Assert.Equal('+', canvas.Data[2, 2].Char);
    }

    [Fact]
    public void Draw_1x1_RendersTopLeftOnly() {
        var border = NKBorderStyle.GetAscii();
        using var canvas = new NKCharCanvas(5, 5);

        border.Draw(new Rect2D(1, 1, 2, 2), canvas, 0);

        Assert.Equal('+', canvas.Data[1, 1].Char);
        Assert.Equal(' ', canvas.Data[2, 1].Char);
        Assert.Equal(' ', canvas.Data[1, 2].Char);
    }

    [Fact]
    public void Draw_DegenerateOrBorderless_DoesNotDraw() {
        var border = NKBorderStyle.GetAscii();
        using var canvas = new NKCharCanvas(5, 5);

        // Zero size
        border.Draw(new Rect2D(1, 1, 1, 1), canvas, 0);
        Assert.Equal(' ', canvas.Data[1, 1].Char);

        // Borderless
        var borderless = NKBorderStyle.GetBorderless();
        borderless.Draw(new Rect2D(0, 0, 5, 5), canvas, 0);
        Assert.Equal(' ', canvas.Data[0, 0].Char);
    }
}

