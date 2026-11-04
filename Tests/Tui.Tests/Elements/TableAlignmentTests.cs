// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Elements.Tests;

public class TableAlignmentTests {

    [Fact]
    public void Table_DefaultAlignment_IsLeftAligned() {
        var table = Table.FromText(
            headers: ["Col"],
            rows: [["Hi"]],
            style: NKTableStyle.GetAscii(),
            columnWidths: [Dimension.Chars(10)]
        );

        var context = new LayoutContext(new Size2D(30, 10), new Size2D(30, 10));
        var size    = table.Measure(context);

        using var canvas        = new NKCharCanvas(size.X, size.Y);
        var       renderContext = new RenderContext(canvas);
        table.Render(renderContext, size);

        // In ASCII table, y=1 is header "Col", y=3 is data row "Hi".
        // Border is '|' at x=0, padding left is 1 at x=1.
        // With left alignment, 'H' should be at x=2, 'i' at x=3.
        Assert.Equal('|', canvas.Data[0, 3].Char);
        Assert.Equal(' ', canvas.Data[1, 3].Char); // padding
        Assert.Equal('H', canvas.Data[2, 3].Char);
        Assert.Equal('i', canvas.Data[3, 3].Char);
    }

    [Fact]
    public void Table_RightAlignedColumn_AlignsTextFlushToRightPadding() {
        var table = Table.FromText(
            headers: ["Price"],
            rows: [["$5"]],
            style: NKTableStyle.GetAscii(),
            columnWidths: [Dimension.Chars(10)],
            columnAlignments: [HorizontalAlign.RIGHT]
        );

        var context = new LayoutContext(new Size2D(30, 10), new Size2D(30, 10));
        var size    = table.Measure(context);

        using var canvas        = new NKCharCanvas(size.X, size.Y);
        var       renderContext = new RenderContext(canvas);
        table.Render(renderContext, size);

        // Column width is 10.
        // Cell padding is horizontal: 1 (left: 1, right: 1).
        // Total column cell box width = 10.
        // Child area width = 10 - 2 = 8.
        // Text "$5" has width 2.
        // Offset inside child area = 8 - 2 = 6.
        // Absolute cell x origin = 0 (left border) + 1 = 1.
        // Padded child area start = 1 + 1 (left padding) = 2.
        // Text start x = 2 + 6 = 8.
        // "$5" is at x=8, x=9.
        // Right padding space is at x=10.
        // Right border '|' is at x=11.
        Assert.Equal('$', canvas.Data[8, 3].Char);
        Assert.Equal('5', canvas.Data[9, 3].Char);
        Assert.Equal(' ', canvas.Data[10, 3].Char);
        Assert.Equal('|', canvas.Data[11, 3].Char);
    }

    [Fact]
    public void Table_CenterAlignedColumn_CentersTextWithinColumn() {
        var table = Table.FromText(
            headers: ["Status"],
            rows: [["OK"]],
            style: NKTableStyle.GetAscii(),
            columnWidths: [Dimension.Chars(10)],
            columnAlignments: [HorizontalAlign.CENTER]
        );

        var context = new LayoutContext(new Size2D(30, 10), new Size2D(30, 10));
        var size    = table.Measure(context);

        using var canvas        = new NKCharCanvas(size.X, size.Y);
        var       renderContext = new RenderContext(canvas);
        table.Render(renderContext, size);

        // Child area width = 8 (10 - 2).
        // Text "OK" width = 2.
        // Offset = (8 - 2) / 2 = 3.
        // Text start x = 2 (border 1 + padding 1) + 3 = 5.
        // "OK" is at x=5, x=6.
        Assert.Equal('O', canvas.Data[5, 3].Char);
        Assert.Equal('K', canvas.Data[6, 3].Char);
    }

    [Fact]
    public void Table_MixedColumnAlignments_AlignsEachColumnIndependently() {
        var table = Table.FromText(
            headers: ["Name", "Qty", "Active"],
            rows: [["Item", "42", "Yes"]],
            style: NKTableStyle.GetAscii(),
            columnWidths: [Dimension.Chars(8), Dimension.Chars(8), Dimension.Chars(8)],
            columnAlignments: [HorizontalAlign.LEFT, HorizontalAlign.RIGHT, HorizontalAlign.CENTER]
        );

        var context = new LayoutContext(new Size2D(50, 10), new Size2D(50, 10));
        var size    = table.Measure(context);

        using var canvas        = new NKCharCanvas(size.X, size.Y);
        var       renderContext = new RenderContext(canvas);
        table.Render(renderContext, size);

        // Col 0: Left aligned. Border at 0. Padding at 1. "Item" at 2..5.
        Assert.Equal('I', canvas.Data[2, 3].Char);
        Assert.Equal('t', canvas.Data[3, 3].Char);
        Assert.Equal('e', canvas.Data[4, 3].Char);
        Assert.Equal('m', canvas.Data[5, 3].Char);

        // Col 1: Right aligned.
        // Col 0 ends at border x=9.
        // Col 1 box is x=10..17 (width 8).
        // Padded area: 11..16 (width 6).
        // Text "42" (width 2) offset = 6 - 2 = 4 -> x = 11 + 4 = 15.
        Assert.Equal('4', canvas.Data[15, 3].Char);
        Assert.Equal('2', canvas.Data[16, 3].Char);
        Assert.Equal(' ', canvas.Data[17, 3].Char); // right padding
        Assert.Equal('|', canvas.Data[18, 3].Char); // separator

        // Col 2: Center aligned.
        // Col 2 box is x=19..26 (width 8).
        // Padded area: 20..25 (width 6).
        // Text "Yes" (width 3) offset = (6 - 3) / 2 = 1 -> x = 20 + 1 = 21.
        Assert.Equal('Y', canvas.Data[21, 3].Char);
        Assert.Equal('e', canvas.Data[22, 3].Char);
        Assert.Equal('s', canvas.Data[23, 3].Char);
    }

    [Fact]
    public void Table_HeaderAlignment_InheritsColumnAlignment_UnlessOverridden() {
        // Table with column 0 right-aligned, but header overridden to center
        var table = Table.FromText(
            headers: ["Price"],
            rows: [["100"]],
            style: NKTableStyle.GetAscii(),
            columnWidths: [Dimension.Chars(12)],
            columnAlignments: [HorizontalAlign.RIGHT],
            headerAlignments: [HorizontalAlign.CENTER]
        );

        var context = new LayoutContext(new Size2D(30, 10), new Size2D(30, 10));
        var size    = table.Measure(context);

        using var canvas        = new NKCharCanvas(size.X, size.Y);
        var       renderContext = new RenderContext(canvas);
        table.Render(renderContext, size);

        // Box width = 12. Padded width = 10 (2..11).
        // Header "Price" (length 5) centered: offset = (10 - 5) / 2 = 2 -> x = 2 + 2 = 4.
        Assert.Equal('P', canvas.Data[4, 1].Char);
        Assert.Equal('r', canvas.Data[5, 1].Char);
        Assert.Equal('i', canvas.Data[6, 1].Char);
        Assert.Equal('c', canvas.Data[7, 1].Char);
        Assert.Equal('e', canvas.Data[8, 1].Char);

        // Data "100" (length 3) right-aligned: offset = 10 - 3 = 7 -> x = 2 + 7 = 9.
        Assert.Equal('1', canvas.Data[9, 3].Char);
        Assert.Equal('0', canvas.Data[10, 3].Char);
        Assert.Equal('0', canvas.Data[11, 3].Char);
    }

    [Fact]
    public void Table_VerticalAlignment_AlignsCellsWithinTallerRows() {
        // Row with 2 lines in col 0 ("Line1\nLine2"), col 1 has single line ("Bot") bottom-aligned
        var data = new List2D<IElement>(2, 1);
        data.Resize(2, 1);
        data[0, 0] = new Text(new AnsiString("A\nB"));
        data[1, 0] = new Text(new AnsiString("Bot"));

        IReadOnlyList2D<IElement> roData = data;

        var table = new Table(
            headers: null,
            data: roData,
            style: NKTableStyle.GetAscii(),
            columnVerticalAlignments: [VerticalAlign.TOP, VerticalAlign.BOTTOM]
        );

        var context = new LayoutContext(new Size2D(30, 10), new Size2D(30, 10));
        var size    = table.Measure(context);

        using var canvas        = new NKCharCanvas(size.X, size.Y);
        var       renderContext = new RenderContext(canvas);
        table.Render(renderContext, size);

        // Row height is 2.
        // Row starts at y=1 (after top border at y=0).
        // Col 0: 'A' at y=1, 'B' at y=2.
        // Col 1: Bottom-aligned -> "Bot" at y=2 (y=1 is space).
        Assert.Equal('A', canvas.Data[2, 1].Char);
        Assert.Equal('B', canvas.Data[2, 2].Char);

        Assert.Equal(' ', canvas.Data[6, 1].Char); // empty row 0 in col 1
        Assert.Equal('B', canvas.Data[6, 2].Char);
        Assert.Equal('o', canvas.Data[7, 2].Char);
        Assert.Equal('t', canvas.Data[8, 2].Char);
    }

    [Fact]
    public void Table_CustomElements_AlignCorrectly() {
        // Button inside right-aligned column
        var button = Button.FromText("OK", borderStyle: NKBorderStyle.GetAscii());
        var data = new List2D<IElement>(1, 1);
        data.Resize(1, 1);
        data[0, 0] = button;

        var table = new Table(
            headers: null,
            data: data,
            style: NKTableStyle.GetAscii(),
            columnWidths: [Dimension.Chars(12)],
            columnAlignments: [HorizontalAlign.RIGHT]
        );

        var context = new LayoutContext(new Size2D(30, 10), new Size2D(30, 10));
        var size    = table.Measure(context);

        using var canvas        = new NKCharCanvas(size.X, size.Y);
        var       renderContext = new RenderContext(canvas);
        table.Render(renderContext, size);

        // Button has border + content "OK" + padding 1.
        // Button measured width = 2 (border) + 2 (text) + 2 (padding) = 6.
        // Cell area width = 12 - 2 = 10.
        // Right offset = 10 - 6 = 4.
        // Cell start = 2 (border 1 + padding 1).
        // Button starts at 2 + 4 = 6.
        // Button left border '+' at x=6.
        Assert.Equal('+', canvas.Data[6, 1].Char);
    }

    [Fact]
    public void Table_ReactiveColumnAlignments_UpdatesOnSignalChange() {
        var alignments = new Signal<IReadOnlyList<HorizontalAlign>?>([HorizontalAlign.LEFT]);

        var table = new Table(
            data: new Signal<IReadOnlyList2D<IElement>>(
                new List2D<IElement>(new IElement[,] { { new Text(new AnsiString("X")) } })
            ),
            style: NKTableStyle.GetAscii(),
            columnWidths: new[] { Dimension.Chars(6) },
            columnAlignments: alignments
        );

        var context = new LayoutContext(new Size2D(20, 10), new Size2D(20, 10));
        var size    = table.Measure(context);

        // 1. Initial: LEFT aligned. 'X' at x=2.
        using (var canvas = new NKCharCanvas(size.X, size.Y)) {
            var rc = new RenderContext(canvas);
            table.Render(rc, size);
            Assert.Equal('X', canvas.Data[2, 1].Char);
        }

        // 2. Change signal to RIGHT aligned.
        alignments.Value = [HorizontalAlign.RIGHT];

        using (var canvas = new NKCharCanvas(size.X, size.Y)) {
            var rc = new RenderContext(canvas);
            table.Render(rc, size);
            // Box width 6, padded width 4. 'X' (len 1) offset = 4 - 1 = 3 -> x = 2 + 3 = 5.
            Assert.Equal('X', canvas.Data[5, 1].Char);
        }
    }
}
