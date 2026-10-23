// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Tests.Core;

public class NKTableStyleTests {

    #region Constructor Tests

    [Fact]
    public void Constructor_25AnsiChars_AssignsAllProperties() {
        var c = new AnsiChar[25];
        for (int i = 0; i < 25; i++) {
            c[i] = new AnsiChar((char)('A' + i), new NKStyle(ConsoleColor.Red));
        }

        var table = new NKTableStyle(
            c[0], c[1], c[2], c[3], c[4], c[5], c[6], c[7],
            c[8], c[9], c[10], c[11], c[12], c[13], c[14],
            c[15], c[16], c[17], c[18], c[19], c[20], c[21], c[22], c[23], c[24],
            NKTableStyle.Features.BORDER
        );

        Assert.Equal(c[0], table.TopLeft);
        Assert.Equal(c[1], table.Top);
        Assert.Equal(c[2], table.TopRight);
        Assert.Equal(c[3], table.Right);
        Assert.Equal(c[4], table.BottomRight);
        Assert.Equal(c[5], table.Bottom);
        Assert.Equal(c[6], table.BottomLeft);
        Assert.Equal(c[7], table.Left);
        Assert.Equal(c[8], table.LeftT);
        Assert.Equal(c[9], table.RightT);
        Assert.Equal(c[10], table.BottomT);
        Assert.Equal(c[11], table.TopT);
        Assert.Equal(c[12], table.Cross);
        Assert.Equal(c[13], table.VerticalSeparator);
        Assert.Equal(c[14], table.HorizontalSeparator);

        Assert.Equal(c[15], table.Header_TopLeft);
        Assert.Equal(c[16], table.Header_Top);
        Assert.Equal(c[17], table.Header_TopRight);
        Assert.Equal(c[18], table.Header_Right);
        Assert.Equal(c[19], table.Header_Bottom);
        Assert.Equal(c[20], table.Header_Left);
        Assert.Equal(c[21], table.Header_LeftT);
        Assert.Equal(c[22], table.Header_RightT);
        Assert.Equal(c[23], table.Header_TopT);
        Assert.Equal(c[24], table.Header_Cross);

        Assert.Equal(NKTableStyle.Features.BORDER, table.EnabledFeatures);
    }

    [Fact]
    public void Constructor_15AnsiChars_AssignsBodyAndDefaultsHeader() {
        var tl = new AnsiChar('┌', default);
        var t  = new AnsiChar('─', default);
        var tr = new AnsiChar('┐', default);
        var r  = new AnsiChar('│', default);
        var br = new AnsiChar('┘', default);
        var b  = new AnsiChar('─', default);
        var bl = new AnsiChar('└', default);
        var l  = new AnsiChar('│', default);
        var lt = new AnsiChar('├', default);
        var rt = new AnsiChar('┤', default);
        var bt = new AnsiChar('┴', default);
        var tt = new AnsiChar('┬', default);
        var cr = new AnsiChar('┼', default);
        var vs = new AnsiChar('│', default);
        var hs = new AnsiChar('─', default);

        var table = new NKTableStyle(tl, t, tr, r, br, b, bl, l, lt, rt, bt, tt, cr, vs, hs);

        Assert.Equal(tl, table.TopLeft);
        Assert.Equal(tl, table.Header_TopLeft);
        Assert.Equal(t,  table.Header_Top);
        Assert.Equal(tr, table.Header_TopRight);
        Assert.Equal(r,  table.Header_Right);
        Assert.Equal(hs, table.Header_Bottom);
        Assert.Equal(l,  table.Header_Left);
        Assert.Equal(lt, table.Header_LeftT);
        Assert.Equal(rt, table.Header_RightT);
        Assert.Equal(tt, table.Header_TopT);
        Assert.Equal(cr, table.Header_Cross);
    }

    [Fact]
    public void Constructor_15CharsSingleStyle_AssignsAllElements() {
        var style = new NKStyle(ConsoleColor.Green);
        var table = new NKTableStyle(
            '1', '2', '3', '4', '5', '6', '7', '8',
            '9', 'A', 'B', 'C', 'D', 'E', 'F',
            style
        );

        Assert.Equal('1', table.TopLeft.Char);
        Assert.Equal(style, table.TopLeft.Style);
        Assert.Equal('D', table.Cross.Char);
        Assert.Equal(style, table.Cross.Style);
        Assert.Equal('F', table.Header_Bottom.Char);
    }

    [Fact]
    public void Constructor_11CharsSingleStyle_AssignsExpectedElements() {
        var style = new NKStyle(ConsoleColor.Blue);
        var table = new NKTableStyle(
            '|', '-', '1', '2', '3', '4', 'L', 'R', 'B', 'T', '+',
            style
        );

        Assert.Equal('1', table.TopLeft.Char);
        Assert.Equal('-', table.Top.Char);
        Assert.Equal('2', table.TopRight.Char);
        Assert.Equal('|', table.Right.Char);
        Assert.Equal('4', table.BottomRight.Char);
        Assert.Equal('-', table.Bottom.Char);
        Assert.Equal('3', table.BottomLeft.Char);
        Assert.Equal('|', table.Left.Char);
        Assert.Equal('L', table.LeftT.Char);
        Assert.Equal('R', table.RightT.Char);
        Assert.Equal('B', table.BottomT.Char);
        Assert.Equal('T', table.TopT.Char);
        Assert.Equal('+', table.Cross.Char);
        Assert.Equal('|', table.VerticalSeparator.Char);
        Assert.Equal('-', table.HorizontalSeparator.Char);
    }

    [Fact]
    public void Constructor_16CharsWithHeader_AssignsDedicatedHeader() {
        var bodyStyle = new NKStyle(ConsoleColor.DarkGray);
        var headerStyle = new NKStyle(ConsoleColor.Yellow);

        var table = new NKTableStyle(
            '│', '─', '┌', '┐', '└', '┘', '├', '┤', '┴', '┬', '┼',
            headerHorizontal: '═',
            headerLeftT: '╞',
            headerRightT: '╡',
            headerTopT: '┬',
            headerCross: '╪',
            bodyStyle: bodyStyle,
            headerStyle: headerStyle
        );

        Assert.Equal('─', table.HorizontalSeparator.Char);
        Assert.Equal(bodyStyle, table.HorizontalSeparator.Style);

        Assert.Equal('═', table.Header_Bottom.Char);
        Assert.Equal(headerStyle, table.Header_Bottom.Style);
        Assert.Equal('╞', table.Header_LeftT.Char);
        Assert.Equal(headerStyle, table.Header_LeftT.Style);
        Assert.Equal('╡', table.Header_RightT.Char);
        Assert.Equal('╪', table.Header_Cross.Char);
    }

    [Fact]
    public void Constructor_5CharsCompact_AssignsAllParts() {
        var table = new NKTableStyle('|', '-', 'c', 't', 'x');

        Assert.Equal('c', table.TopLeft.Char);
        Assert.Equal('-', table.Top.Char);
        Assert.Equal('|', table.Left.Char);
        Assert.Equal('t', table.LeftT.Char);
        Assert.Equal('x', table.Cross.Char);
    }

    [Fact]
    public void Constructor_3CharsCompact_AssignsJunctionToAllIntersections() {
        var table = new NKTableStyle('|', '-', '+');

        Assert.Equal('+', table.TopLeft.Char);
        Assert.Equal('+', table.TopRight.Char);
        Assert.Equal('+', table.BottomLeft.Char);
        Assert.Equal('+', table.BottomRight.Char);
        Assert.Equal('+', table.LeftT.Char);
        Assert.Equal('+', table.RightT.Char);
        Assert.Equal('+', table.BottomT.Char);
        Assert.Equal('+', table.TopT.Char);
        Assert.Equal('+', table.Cross.Char);
        Assert.Equal('|', table.VerticalSeparator.Char);
        Assert.Equal('-', table.HorizontalSeparator.Char);
    }

    [Fact]
    public void Constructor_FromBorderStyle_CopiesBorderChars() {
        var border = NKBorderStyle.GetRounded(new NKStyle(ConsoleColor.Cyan));
        var table = new NKTableStyle(border);

        Assert.Equal(border.TopLeft, table.TopLeft);
        Assert.Equal(border.Top, table.Top);
        Assert.Equal(border.TopRight, table.TopRight);
        Assert.Equal(border.BottomRight, table.BottomRight);
        Assert.Equal(border.BottomLeft, table.BottomLeft);
    }

    [Fact]
    public void Constructor_Default_AssignsAscii() {
        var table = new NKTableStyle();

        Assert.Equal('+', table.TopLeft.Char);
        Assert.Equal('-', table.Top.Char);
        Assert.Equal('|', table.Left.Char);
        Assert.Equal('+', table.Cross.Char);
        Assert.Equal(NKTableStyle.Features.DEFAULT, table.EnabledFeatures);
    }

    #endregion

    #region Factory Method Tests

    [Fact]
    public void GetAscii_ConstructsExpectedTableStyle() {
        var table = NKTableStyle.GetAscii();

        Assert.Equal('+', table.TopLeft.Char);
        Assert.Equal('-', table.Top.Char);
        Assert.Equal('|', table.Left.Char);
        Assert.Equal('+', table.Cross.Char);
        Assert.Equal(table, NKTableStyle.Ascii);
    }

    [Fact]
    public void GetNormal_ConstructsExpectedTableStyle() {
        var table = NKTableStyle.GetNormal();

        Assert.Equal('┌', table.TopLeft.Char);
        Assert.Equal('─', table.Top.Char);
        Assert.Equal('┐', table.TopRight.Char);
        Assert.Equal('│', table.Left.Char);
        Assert.Equal('├', table.LeftT.Char);
        Assert.Equal('┤', table.RightT.Char);
        Assert.Equal('┴', table.BottomT.Char);
        Assert.Equal('┬', table.TopT.Char);
        Assert.Equal('┼', table.Cross.Char);
        Assert.Equal('└', table.BottomLeft.Char);
        Assert.Equal('┘', table.BottomRight.Char);
        Assert.Equal(table, NKTableStyle.Normal);
    }

    [Fact]
    public void GetRounded_ConstructsExpectedTableStyle() {
        var table = NKTableStyle.GetRounded();

        Assert.Equal('╭', table.TopLeft.Char);
        Assert.Equal('╮', table.TopRight.Char);
        Assert.Equal('╰', table.BottomLeft.Char);
        Assert.Equal('╯', table.BottomRight.Char);
        Assert.Equal('│', table.Left.Char);
        Assert.Equal('─', table.Top.Char);
        Assert.Equal(table, NKTableStyle.Rounded);
    }

    [Fact]
    public void GetThick_ConstructsExpectedTableStyle() {
        var table = NKTableStyle.GetThick();

        Assert.Equal('┏', table.TopLeft.Char);
        Assert.Equal('━', table.Top.Char);
        Assert.Equal('┓', table.TopRight.Char);
        Assert.Equal('┃', table.Left.Char);
        Assert.Equal('┣', table.LeftT.Char);
        Assert.Equal('┫', table.RightT.Char);
        Assert.Equal('┻', table.BottomT.Char);
        Assert.Equal('┳', table.TopT.Char);
        Assert.Equal('╋', table.Cross.Char);
        Assert.Equal('┗', table.BottomLeft.Char);
        Assert.Equal('┛', table.BottomRight.Char);
        Assert.Equal(table, NKTableStyle.Thick);
    }

    [Fact]
    public void GetDouble_ConstructsExpectedTableStyle() {
        var table = NKTableStyle.GetDouble();

        Assert.Equal('╔', table.TopLeft.Char);
        Assert.Equal('═', table.Top.Char);
        Assert.Equal('╗', table.TopRight.Char);
        Assert.Equal('║', table.Left.Char);
        Assert.Equal('╠', table.LeftT.Char);
        Assert.Equal('╣', table.RightT.Char);
        Assert.Equal('╩', table.BottomT.Char);
        Assert.Equal('╦', table.TopT.Char);
        Assert.Equal('╬', table.Cross.Char);
        Assert.Equal('╚', table.BottomLeft.Char);
        Assert.Equal('╝', table.BottomRight.Char);
        Assert.Equal(table, NKTableStyle.Double);
    }

    [Fact]
    public void GetDoubleHeader_ConstructsExpectedTableStyle() {
        var table = NKTableStyle.GetDoubleHeader();

        Assert.Equal('┌', table.TopLeft.Char);
        Assert.Equal('─', table.Top.Char);
        Assert.Equal('│', table.Left.Char);
        Assert.Equal('┼', table.Cross.Char);
        Assert.Equal('─', table.HorizontalSeparator.Char);

        Assert.Equal('═', table.Header_Bottom.Char);
        Assert.Equal('╞', table.Header_LeftT.Char);
        Assert.Equal('╡', table.Header_RightT.Char);
        Assert.Equal('╪', table.Header_Cross.Char);
        Assert.Equal(table, NKTableStyle.DoubleHeader);
    }

    [Fact]
    public void GetSolid_ConstructsExpectedTableStyle() {
        var color = NKColor.FromRgb(50, 100, 150);
        var table = NKTableStyle.GetSolid(color);

        Assert.Equal(' ', table.TopLeft.Char);
        Assert.Equal(' ', table.Cross.Char);
        Assert.Equal(color, table.TopLeft.Style.BColor);
        Assert.Equal(color, table.Cross.Style.BColor);
    }

    [Fact]
    public void GetBorderless_ConstructsExpectedTableStyle() {
        var table = NKTableStyle.GetBorderless();

        Assert.Equal(' ', table.TopLeft.Char);
        Assert.True(table.IsBorderless);
        Assert.Equal(NKTableStyle.Features.NONE, table.EnabledFeatures);
        Assert.Equal(table, NKTableStyle.Borderless);
    }

    [Fact]
    public void GetMinimal_ConstructsExpectedTableStyle() {
        var table = NKTableStyle.GetMinimal();

        Assert.False(table.HasBorderLeft);
        Assert.False(table.HasBorderRight);
        Assert.False(table.HasBorderTop);
        Assert.False(table.HasBorderBottom);
        Assert.True(table.HasHeaderRow);
        Assert.True(table.HasHeaderSeparator);
        Assert.True(table.HasVerticalSeparators);
        Assert.Equal(table, NKTableStyle.Minimal);
    }

    [Fact]
    public void GetInset_AssignsCorrectShading() {
        var shadow    = new NKStyle(ConsoleColor.DarkGray);
        var highlight = new NKStyle(ConsoleColor.White);

        var table = NKTableStyle.GetInset(shadow, highlight);

        Assert.Equal('┌', table.TopLeft.Char);
        Assert.Equal(shadow, table.TopLeft.Style);
        Assert.Equal('─', table.Top.Char);
        Assert.Equal(shadow, table.Top.Style);
        Assert.Equal('┐', table.TopRight.Char);
        Assert.Equal(highlight, table.TopRight.Style);
        Assert.Equal('│', table.Right.Char);
        Assert.Equal(highlight, table.Right.Style);
        Assert.Equal('┘', table.BottomRight.Char);
        Assert.Equal(highlight, table.BottomRight.Style);
        Assert.Equal('─', table.Bottom.Char);
        Assert.Equal(highlight, table.Bottom.Style);
        Assert.Equal('└', table.BottomLeft.Char);
        Assert.Equal(shadow, table.BottomLeft.Style);
        Assert.Equal('│', table.Left.Char);
        Assert.Equal(shadow, table.Left.Style);
    }

    [Fact]
    public void GetOutset_AssignsCorrectShading() {
        var shadow    = new NKStyle(ConsoleColor.DarkGray);
        var highlight = new NKStyle(ConsoleColor.White);

        var table = NKTableStyle.GetOutset(shadow, highlight);

        Assert.Equal('┌', table.TopLeft.Char);
        Assert.Equal(highlight, table.TopLeft.Style);
        Assert.Equal('─', table.Top.Char);
        Assert.Equal(highlight, table.Top.Style);
        Assert.Equal('┐', table.TopRight.Char);
        Assert.Equal(shadow, table.TopRight.Style);
        Assert.Equal('│', table.Right.Char);
        Assert.Equal(shadow, table.Right.Style);
        Assert.Equal('┘', table.BottomRight.Char);
        Assert.Equal(shadow, table.BottomRight.Style);
        Assert.Equal('─', table.Bottom.Char);
        Assert.Equal(shadow, table.Bottom.Style);
        Assert.Equal('└', table.BottomLeft.Char);
        Assert.Equal(highlight, table.BottomLeft.Style);
        Assert.Equal('│', table.Left.Char);
        Assert.Equal(highlight, table.Left.Style);
    }

    [Fact]
    public void GetInsetThick_AssignsThickCharsAndCorrectShading() {
        var shadow    = new NKStyle(ConsoleColor.DarkGray);
        var highlight = new NKStyle(ConsoleColor.White);

        var table = NKTableStyle.GetInsetThick(shadow, highlight);

        Assert.Equal('┏', table.TopLeft.Char);
        Assert.Equal(shadow, table.TopLeft.Style);
        Assert.Equal('┛', table.BottomRight.Char);
        Assert.Equal(highlight, table.BottomRight.Style);
        Assert.Equal('╋', table.Cross.Char);
    }

    [Fact]
    public void GetOutsetThick_AssignsThickCharsAndCorrectShading() {
        var shadow    = new NKStyle(ConsoleColor.DarkGray);
        var highlight = new NKStyle(ConsoleColor.White);

        var table = NKTableStyle.GetOutsetThick(shadow, highlight);

        Assert.Equal('┏', table.TopLeft.Char);
        Assert.Equal(highlight, table.TopLeft.Style);
        Assert.Equal('┛', table.BottomRight.Char);
        Assert.Equal(shadow, table.BottomRight.Style);
        Assert.Equal('╋', table.Cross.Char);
    }

    #endregion

    #region Feature Manipulation Tests

    [Fact]
    public void Features_EnableDisableWithHasFeature_WorksAsExpected() {
        var style = NKTableStyle.Normal;

        Assert.True(style.HasFeature(NKTableStyle.Features.BORDER_LEFT));
        Assert.True(style.HasFeature(NKTableStyle.Features.BORDER));
        Assert.True(style.HasBorderLeft);
        Assert.True(style.HasBorderRight);
        Assert.True(style.HasBorderTop);
        Assert.True(style.HasBorderBottom);
        Assert.True(style.HasVerticalSeparators);
        Assert.True(style.HasHorizontalSeparators);
        Assert.True(style.HasHeaderRow);
        Assert.True(style.HasHeaderSeparator);

        var modified = style.DisableFeatures(NKTableStyle.Features.BORDER_LEFT | NKTableStyle.Features.HORIZONTAL_ELEMENT_SEPARATOR);
        Assert.False(modified.HasFeature(NKTableStyle.Features.BORDER_LEFT));
        Assert.False(modified.HasBorderLeft);
        Assert.False(modified.HasHorizontalSeparators);
        Assert.True(modified.HasBorderRight);

        var reEnabled = modified.EnableFeatures(NKTableStyle.Features.BORDER_LEFT);
        Assert.True(reEnabled.HasFeature(NKTableStyle.Features.BORDER_LEFT));
        Assert.True(reEnabled.HasBorderLeft);

        var custom = style.WithFeatures(NKTableStyle.Features.NONE);
        Assert.True(custom.IsBorderless);
        Assert.False(custom.HasBorderLeft);
    }

    #endregion

    #region Layout & Drawing Tests

    [Fact]
    public void GetTotalSize_EmptyArray_ReturnsZero() {
        var style = NKTableStyle.Normal;
        Assert.Equal(Size2D.Zero, style.GetTotalSize(new Size2D[0, 0]));
        Assert.Equal(Size2D.Zero, style.GetTotalSize(new Size2D[0, 2]));
        Assert.Equal(Size2D.Zero, style.GetTotalSize(new Size2D[2, 0]));
    }

    [Fact]
    public void GetElementOffsets_EmptyArray_ReturnsEmpty() {
        var style = NKTableStyle.Normal;
        var offsets = style.GetElementOffsets(new Size2D[0, 0]);
        Assert.Equal(0, offsets.GetLength(0));
        Assert.Equal(0, offsets.GetLength(1));
    }

    [Fact]
    public void GetTotalSize_2x2Table_WithBordersAndSeparators_CalculatesCorrectSize() {
        var style = NKTableStyle.Normal; // Default: BORDER | ELEMENT_SEPARATORS | HEADER
        var elements = new Size2D[,] {
            { new(5, 1), new(3, 1) },
            { new(4, 2), new(2, 2) }
        };

        // Col widths: max(5, 4)=5, max(3, 2)=3.
        // Row heights: max(1, 1)=1, max(2, 2)=2.
        // LeftBorder (1) + Col0 (5) + ColSep (1) + Col1 (3) + RightBorder (1) = 11.
        // TopBorder (1) + Row0 (1) + HeaderSep (1) + Row1 (2) + BottomBorder (1) = 6.
        var size = style.GetTotalSize(elements);
        Assert.Equal(new Size2D(11, 6), size);
    }

    [Fact]
    public void GetElementOffsets_2x2Table_CalculatesExactCellOrigins() {
        var style = NKTableStyle.Normal;
        var elements = new Size2D[,] {
            { new(5, 1), new(3, 1) },
            { new(4, 2), new(2, 2) }
        };

        var offsets = style.GetElementOffsets(elements);

        // Col 0 starts at x = 1 (after left border).
        // Col 1 starts at x = 1 + 5 + 1 = 7 (after col 0 + col separator).
        // Row 0 starts at y = 1 (after top border).
        // Row 1 starts at y = 1 + 1 + 1 = 3 (after row 0 + header separator).
        Assert.Equal(new Point2D(1, 1), offsets[0, 0]);
        Assert.Equal(new Point2D(7, 1), offsets[0, 1]);
        Assert.Equal(new Point2D(1, 3), offsets[1, 0]);
        Assert.Equal(new Point2D(7, 3), offsets[1, 1]);
    }

    [Fact]
    public void GetTotalSize_Borderless_ReturnsPureCellDimensions() {
        var style = NKTableStyle.Borderless;
        var elements = new Size2D[,] {
            { new(5, 1), new(3, 1) },
            { new(4, 2), new(2, 2) }
        };

        var size = style.GetTotalSize(elements);
        // Col0: 5, Col1: 3 -> Total W = 8
        // Row0: 1, Row1: 2 -> Total H = 3
        Assert.Equal(new Size2D(8, 3), size);

        var offsets = style.GetElementOffsets(elements);
        Assert.Equal(new Point2D(0, 0), offsets[0, 0]);
        Assert.Equal(new Point2D(5, 0), offsets[0, 1]);
        Assert.Equal(new Point2D(0, 1), offsets[1, 0]);
        Assert.Equal(new Point2D(5, 1), offsets[1, 1]);
    }

    [Fact]
    public void Draw_2x2TableWithHeader_RendersExpectedGridOnCanvas() {
        using var canvas = new NKCharCanvas(15, 10);
        var style = NKTableStyle.Normal;
        var elements = new Size2D[,] {
            { new(5, 1), new(3, 1) },
            { new(5, 1), new(3, 1) }
        };

        // Table size: W = 1 + 5 + 1 + 3 + 1 = 11. H = 1 + 1 + 1 + 1 + 1 = 5.
        style.Draw(new Point2D(1, 1), elements, canvas, 0);

        // Top border at y = 1
        Assert.Equal('┌', canvas.Data[1, 1].Char);
        for (int x = 2; x <= 6; x++) Assert.Equal('─', canvas.Data[x, 1].Char);
        Assert.Equal('┬', canvas.Data[7, 1].Char);
        for (int x = 8; x <= 10; x++) Assert.Equal('─', canvas.Data[x, 1].Char);
        Assert.Equal('┐', canvas.Data[11, 1].Char);

        // Header row at y = 2
        Assert.Equal('│', canvas.Data[1, 2].Char);
        Assert.Equal('│', canvas.Data[7, 2].Char);
        Assert.Equal('│', canvas.Data[11, 2].Char);

        // Header separator at y = 3
        Assert.Equal('├', canvas.Data[1, 3].Char);
        for (int x = 2; x <= 6; x++) Assert.Equal('─', canvas.Data[x, 3].Char);
        Assert.Equal('┼', canvas.Data[7, 3].Char);
        for (int x = 8; x <= 10; x++) Assert.Equal('─', canvas.Data[x, 3].Char);
        Assert.Equal('┤', canvas.Data[11, 3].Char);

        // Data row at y = 4
        Assert.Equal('│', canvas.Data[1, 4].Char);
        Assert.Equal('│', canvas.Data[7, 4].Char);
        Assert.Equal('│', canvas.Data[11, 4].Char);

        // Bottom border at y = 5
        Assert.Equal('└', canvas.Data[1, 5].Char);
        for (int x = 2; x <= 6; x++) Assert.Equal('─', canvas.Data[x, 5].Char);
        Assert.Equal('┴', canvas.Data[7, 5].Char);
        for (int x = 8; x <= 10; x++) Assert.Equal('─', canvas.Data[x, 5].Char);
        Assert.Equal('┘', canvas.Data[11, 5].Char);
    }

    [Fact]
    public void Draw_DoubleHeaderTable_RendersDoubleHeaderLine() {
        using var canvas = new NKCharCanvas(15, 10);
        var style = NKTableStyle.DoubleHeader;
        var elements = new Size2D[,] {
            { new(4, 1), new(3, 1) },
            { new(4, 1), new(3, 1) }
        };

        // Table size: W = 1 + 4 + 1 + 3 + 1 = 10. H = 5.
        style.Draw(Point2D.Zero, elements, canvas, 0);

        // Header separator at y = 2
        Assert.Equal('╞', canvas.Data[0, 2].Char);
        for (int x = 1; x <= 4; x++) Assert.Equal('═', canvas.Data[x, 2].Char);
        Assert.Equal('╪', canvas.Data[5, 2].Char);
        for (int x = 6; x <= 8; x++) Assert.Equal('═', canvas.Data[x, 2].Char);
        Assert.Equal('╡', canvas.Data[9, 2].Char);
    }

    [Fact]
    public void Draw_MinimalTable_RendersOnlyHeaderAndVerticalSeparators() {
        using var canvas = new NKCharCanvas(12, 6);
        var style = NKTableStyle.Minimal;
        var elements = new Size2D[,] {
            { new(4, 1), new(3, 1) },
            { new(4, 1), new(3, 1) }
        };

        // Minimal has NO outer borders:
        // Col0 at x = 0..3
        // Vertical sep at x = 4
        // Col1 at x = 5..7
        // Header at y = 0
        // Header sep at y = 1
        // Data row at y = 2
        style.Draw(Point2D.Zero, elements, canvas, 0);

        // Header row vertical separator
        Assert.Equal('│', canvas.Data[4, 0].Char);

        // Header separator line
        for (int x = 0; x <= 3; x++) Assert.Equal('─', canvas.Data[x, 1].Char);
        Assert.Equal('┼', canvas.Data[4, 1].Char);
        for (int x = 5; x <= 7; x++) Assert.Equal('─', canvas.Data[x, 1].Char);

        // Data row vertical separator
        Assert.Equal('│', canvas.Data[4, 2].Char);
    }

    [Fact]
    public void GetContentBox_And_GetBorderBox_WorkCorrectly() {
        var style = NKTableStyle.Normal;
        var content = style.GetContentBox(new Size2D(10, 10));
        Assert.Equal(new Rect2D(1, 1, 9, 9), content);

        var border = style.GetBorderBox(new Size2D(8, 8));
        Assert.Equal(new Size2D(10, 10), border);

        var borderless = NKTableStyle.Borderless;
        Assert.Equal(new Rect2D(0, 0, 10, 10), borderless.GetContentBox(new Size2D(10, 10)));
        Assert.Equal(new Size2D(10, 10), borderless.GetBorderBox(new Size2D(10, 10)));
    }

    #endregion
}
