// NeoKolors.Test
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Elements;

namespace NeoKolors.Tui.Tests.Elements;

using CoreDimension = Dimension;
using CoreSpacing = Spacing;
using Element = IElement;
using ElementGrid = Grid;
using ElementList = List;
using ElementTable = Table;

public class ComplexElementsTests {

    [Fact]
    public void List_Bullet_MeasuresAndRendersCorrectly() {
        var screen  = new Size2D(100, 50);
        var context = new LayoutContext(new Size2D(100, 50), screen);

        var item1 = new Text(new AnsiString("First"));  // Length = 5, Height = 1
        var item2 = new Text(new AnsiString("Second")); // Length = 6, Height = 1

        var list = new ElementList(
            items: new Element[] { item1, item2 },
            pointGenerator: ListPoints.Bullet, // "•" (len 1)
            pointGap: 1,
            gap: 1
        );

        var measured = list.Measure(context);

        // Bullet width = 1, pointGap = 1 => prefix = 2
        // Max item width = 6 => total width = 2 + 6 = 8
        // Height = 1 + 1 + gap(1) = 3
        Assert.Equal(8, measured.X);
        Assert.Equal(3, measured.Y);

        using var canvas        = new NKCharCanvas(20, 10);
        var       renderContext = new RenderContext(canvas);
        list.Render(renderContext, measured);

        // Bullet at (0, 0)
        Assert.Equal('•', canvas.Data[0, 0].Char);

        // "First" at (2, 0)
        Assert.Equal('F', canvas.Data[2, 0].Char);

        // Second item at row 2 (due to gap 1)
        Assert.Equal('•', canvas.Data[0, 2].Char);
        Assert.Equal('S', canvas.Data[2, 2].Char);
    }

    [Fact]
    public void List_OrderedNumbers_AlignsBulletsAndCustomItems() {
        var screen  = new Size2D(100, 50);
        var context = new LayoutContext(new Size2D(100, 50), screen);

        var item1 = new Text(new AnsiString("Alpha"));
        var item2 = ElementList.Item(new Text(new AnsiString("Beta")), point: new AnsiString("->"));

        var list = new ElementList(
            items: new Element[] { item1, item2 },
            pointGenerator: ListPoints.NumberDot, // "1." (len 2)
            pointGap: 1
        );

        var measured = list.Measure(context);

        // Point lengths: item 1 is "1." (len 2), item 2 is "->" (len 2) => max point = 2
        // pointGap = 1 => pointPrefix = 3
        // Max content width: Alpha (5), Beta (4) => 5
        // Total width = 3 + 5 = 8
        // Height = 1 + 1 + 0 gap = 2
        Assert.Equal(8, measured.X);
        Assert.Equal(2, measured.Y);

        using var canvas        = new NKCharCanvas(20, 10);
        var       renderContext = new RenderContext(canvas);
        list.Render(renderContext, measured);

        // "1." at (0, 0) and (1, 0)
        Assert.Equal('1', canvas.Data[0, 0].Char);
        Assert.Equal('.', canvas.Data[1, 0].Char);
        Assert.Equal('A', canvas.Data[3, 0].Char);

        // "->" at (0, 1) and (1, 1)
        Assert.Equal('-', canvas.Data[0, 1].Char);
        Assert.Equal('>', canvas.Data[1, 1].Char);
        Assert.Equal('B', canvas.Data[3, 1].Char);
    }

    [Fact]
    public void Grid_FixedAndGrowColumns_MeasuresAndRendersCells() {
        var screen  = new Size2D(100, 50);
        var context = new LayoutContext(new Size2D(50, 20), screen);

        var cell1        = ElementGrid.Cell(new Text(new AnsiString("FixedCol")), row: 0, column: 0);
        var cell2        = ElementGrid.Cell(new Text(new AnsiString("GrowCol")),  row: 0, column: 1);
        var cellSpanning = ElementGrid.Cell(new Text(new AnsiString("Span")),     row: 1, column: 0, columnSpan: 2);

        var grid = new ElementGrid(
            columns: [
                CoreDimension.Chars(12),
                CoreDimension.Fr(1, minChars: 5, maxChars: 30)
            ],
            rows: [
                CoreDimension.Chars(1),
                CoreDimension.Chars(1)
            ],
            children: [cell1, cell2, cellSpanning]
        );

        var measured = grid.Measure(context);

        // Ungrown: Col 0 = 12, Col 1 = min(5) => total width = 17
        // Rows: 1 + 1 = 2
        Assert.Equal(17, measured.X);
        Assert.Equal(2,  measured.Y);

        using var canvas        = new NKCharCanvas(50, 10);
        var       renderContext = new RenderContext(canvas);

        // Render at 30 width:
        // Col 0 = 12, Remaining = 30 - 12 = 18 => Col 1 gets 18
        grid.Render(renderContext, new Size2D(30, 10));

        // cell1 at (0, 0)
        Assert.Equal('F', canvas.Data[0, 0].Char);

        // cell2 at col 1 offset (12, 0)
        Assert.Equal('G', canvas.Data[12, 0].Char);

        // cellSpanning at row 1 offset (0, 1)
        Assert.Equal('S', canvas.Data[0, 1].Char);
    }

    [Fact]
    public void Grid_GapsAndPadding_OffsetCellsAccurately() {
        var screen  = new Size2D(100, 50);
        var context = new LayoutContext(new Size2D(100, 50), screen);

        var c1 = ElementGrid.Cell(new Text(new AnsiString("A")), row: 0, column: 0);
        var c2 = ElementGrid.Cell(new Text(new AnsiString("B")), row: 0, column: 1);

        var grid = new ElementGrid(
            children: new[] { c1, c2 },
            columns: new[] { CoreDimension.Chars(5), CoreDimension.Chars(5) },
            rows: new[] { CoreDimension.Chars(2) },
            columnGap: 2,
            rowGap: 1,
            padding: new CoreSpacing(CoreDimension.Chars(1))
        );

        var size = grid.Measure(context);

        // Columns: 5 + 5 + gap(2) = 12. Padding: +2 horizontally = 14
        // Rows: 2. Padding: +2 vertically = 4
        Assert.Equal(14, size.X);
        Assert.Equal(4,  size.Y);

        using var canvas        = new NKCharCanvas(20, 10);
        var       renderContext = new RenderContext(canvas);
        grid.Render(renderContext, size);

        // Padding is 1 => origin is (1, 1)
        // c1 at (1, 1)
        Assert.Equal('A', canvas.Data[1, 1].Char);

        // c2 at (1 + 5 + 2 = 8, 1)
        Assert.Equal('B', canvas.Data[8, 1].Char);
    }

    [Fact]
    public void Table_FromText_MeasuresAndRendersWithBorders() {
        var headers = new[] { "Name", "Score" };

        var rows = new[] {
            new[] { "Alice", "100" },
            new[] { "Bob", "95" }
        };

        var table = ElementTable.FromText(headers, rows);

        var screen  = new Size2D(100, 50);
        var context = new LayoutContext(new Size2D(100, 50), screen);
        var size    = table.Measure(context);

        Assert.True(size.X > 0);
        Assert.True(size.Y > 0);

        using var canvas        = new NKCharCanvas(size.X, size.Y);
        var       renderContext = new RenderContext(canvas);
        table.Render(renderContext, size);

        // Verify that canvas has content rendered
        bool foundAlice = false;

        for (int y = 0; y < canvas.Height; y++) {
            for (int x = 0; x < canvas.Width - 4; x++) {
                if (canvas.Data[x + 0, y].Char == 'A' &&
                    canvas.Data[x + 1, y].Char == 'l' &&
                    canvas.Data[x + 2, y].Char == 'i' &&
                    canvas.Data[x + 3, y].Char == 'c' &&
                    canvas.Data[x + 4, y].Char == 'e'
                ) {
                    foundAlice = true;

                    break;
                }
            }
        }

        Assert.True(foundAlice, "Expected 'Alice' to be rendered inside the table canvas.");
    }

    [Fact]
    public void Table_WithIReadOnlyList2D_MeasuresAndRendersCorrectly() {
        var data = new List2D<Element>(2, 2); // 2 cols, 2 rows
        data.Resize(2, 2);
        data[0, 0] = new Text(new AnsiString("A1"));
        data[1, 0] = new Text(new AnsiString("B1"));
        data[0, 1] = new Text(new AnsiString("A2"));
        data[1, 1] = new Text(new AnsiString("B2"));

        IReadOnlyList2D<Element> roData = data;

        var table = new ElementTable(
            headers: [new Text(new AnsiString("ColA")), new Text(new AnsiString("ColB"))],
            data: roData
        );

        var screen  = new Size2D(100, 50);
        var context = new LayoutContext(new Size2D(100, 50), screen);
        var size    = table.Measure(context);

        Assert.True(size.X > 0);
        Assert.True(size.Y > 0);

        using var canvas        = new NKCharCanvas(size.X, size.Y);
        var       renderContext = new RenderContext(canvas);
        table.Render(renderContext, size);

        // Verify that canvas has content rendered for header and cells
        bool foundColA = false;
        bool foundB2   = false;

        for (int y = 0; y < canvas.Height; y++) {
            for (int x = 0; x < canvas.Width - 3; x++) {
                if (canvas.Data[x, y].Char     == 'C' &&
                    canvas.Data[x + 1, y].Char == 'o' &&
                    canvas.Data[x + 2, y].Char == 'l' &&
                    canvas.Data[x + 3, y].Char == 'A') {
                    foundColA = true;
                }

                if (canvas.Data[x, y].Char     == 'B' &&
                    canvas.Data[x + 1, y].Char == '2') {
                    foundB2 = true;
                }
            }
        }

        Assert.True(foundColA, "Expected 'ColA' header to be rendered.");
        Assert.True(foundB2,   "Expected 'B2' cell to be rendered.");
    }

    [Fact]
    public void NestedBorderInsideFlex_DrawsBorderAndContentAtCorrectOffset() {
        var button1 = new Border(new Text(new AnsiString("OK")),     style: NKBorderStyle.GetAscii());
        var button2 = new Border(new Text(new AnsiString("Cancel")), style: NKBorderStyle.GetAscii());

        var container = new Flex(
            direction: FlexDirection.COLUMN,
            gap: 1,
            children: new Element[] { button1, button2 }
        );

        using var canvas        = new NKCharCanvas(30, 15);
        var       renderContext = new RenderContext(canvas);

        container.Render(renderContext, new Size2D(30, 15));

        // button1 is 4 wide ("OK" = 2 + 2 border), 3 high (1 + 2 border)
        // at (0, 0)
        Assert.Equal('+', canvas.Data[0, 0].Char);
        Assert.Equal('+', canvas.Data[3, 0].Char);
        Assert.Equal('O', canvas.Data[1, 1].Char);
        Assert.Equal('K', canvas.Data[2, 1].Char);
        Assert.Equal('+', canvas.Data[0, 2].Char);
        Assert.Equal('+', canvas.Data[3, 2].Char);

        // button2 is at Y = 4 (due to button1 height 3 + gap 1 = 4)
        // button2: "Cancel" is 6 wide + 2 border = 8 wide (0..7), 3 high (4..6)
        Assert.Equal('+', canvas.Data[0, 4].Char);
        Assert.Equal('+', canvas.Data[7, 4].Char);
        Assert.Equal('C', canvas.Data[1, 5].Char);
        Assert.Equal('a', canvas.Data[2, 5].Char);
        Assert.Equal('n', canvas.Data[3, 5].Char);
        Assert.Equal('c', canvas.Data[4, 5].Char);
        Assert.Equal('e', canvas.Data[5, 5].Char);
        Assert.Equal('l', canvas.Data[6, 5].Char);
        Assert.Equal('+', canvas.Data[0, 6].Char);
        Assert.Equal('+', canvas.Data[7, 6].Char);
    }

    [Fact]
    public void Text_MultiLine_MeasuresAndRendersCorrectly() {
        var screen  = new Size2D(100, 50);
        var context = new LayoutContext(new Size2D(100, 50), screen);

        var text     = new Text(new AnsiString("Hello\nWorld!\nNeoKolors"));
        var measured = text.Measure(context);

        // "NeoKolors" is length 9, 3 lines => (9, 3)
        Assert.Equal(9, measured.X);
        Assert.Equal(3, measured.Y);

        using var canvas        = new NKCharCanvas(20, 10);
        var       renderContext = new RenderContext(canvas);
        text.Render(renderContext, measured);

        Assert.Equal('H', canvas.Data[0, 0].Char);
        Assert.Equal('o', canvas.Data[4, 0].Char);
        Assert.Equal('W', canvas.Data[0, 1].Char);
        Assert.Equal('!', canvas.Data[5, 1].Char);
        Assert.Equal('N', canvas.Data[0, 2].Char);
        Assert.Equal('s', canvas.Data[8, 2].Char);
    }
}