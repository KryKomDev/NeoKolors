// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements;

public class Table : IElement {
    public Bindable<IReadOnlyList<IElement>?>        Headers                  { get; init; } = default;
    public Bindable<IReadOnlyList2D<IElement>>       Data                     { get; init; }
    public Bindable<ITableStyle>                     Style                    { get; init; } = NKTableStyle.GetRounded();
    public Bindable<IReadOnlyList<Dimension>?>       ColumnWidths             { get; init; } = default;
    public Bindable<IReadOnlyList<HorizontalAlign>?> ColumnAlignments         { get; init; } = default;
    public Bindable<IReadOnlyList<HorizontalAlign>?> HeaderAlignments         { get; init; } = default;
    public Bindable<HorizontalAlign>                 DefaultAlignment         { get; init; } = HorizontalAlign.LEFT;
    public Bindable<IReadOnlyList<VerticalAlign>?>   ColumnVerticalAlignments { get; init; } = default;
    public Bindable<VerticalAlign>                   DefaultVerticalAlignment { get; init; } = VerticalAlign.TOP;
    public Bindable<Spacing>                         CellPadding              { get; init; } = new Spacing(Dimension.Chars(0), Dimension.Chars(1));
    public Bindable<Spacing>                         Margin                   { get; init; } = default;

    /// <summary>
    ///     Convenience alias for <see cref="Data"/>.
    /// </summary>
    public Bindable<IReadOnlyList2D<IElement>> Rows {
        get => Data;
        init => Data = value;
    }

    public Table(IReadOnlyList2D<IElement> data) {
        Data = data;
    }

    public Table(
        IReadOnlyList<IElement>?        headers                  = null,
        IReadOnlyList2D<IElement>?      data                     = null,
        ITableStyle?                    style                    = null,
        IReadOnlyList<Dimension>?       columnWidths             = null,
        IReadOnlyList<HorizontalAlign>? columnAlignments         = null,
        Spacing?                        cellPadding              = null,
        Spacing?                        margin                   = null,
        IReadOnlyList<HorizontalAlign>? headerAlignments         = null,
        IReadOnlyList<VerticalAlign>?   columnVerticalAlignments = null
    ) {
        Headers = headers;
        Data    = data ?? new List2D<IElement>();

        if (style != null)
            Style = style;

        if (columnWidths != null)
            ColumnWidths = columnWidths;

        if (columnAlignments != null)
            ColumnAlignments = columnAlignments;

        if (cellPadding != null)
            CellPadding = cellPadding.Value;

        if (margin != null)
            Margin = margin.Value;

        if (headerAlignments != null)
            HeaderAlignments = headerAlignments;

        if (columnVerticalAlignments != null)
            ColumnVerticalAlignments = columnVerticalAlignments;
    }

    public Table(
        Bindable<IReadOnlyList2D<IElement>>       data,
        Bindable<IReadOnlyList<IElement>?>        headers                  = default,
        Bindable<ITableStyle>                     style                    = default,
        Bindable<IReadOnlyList<Dimension>?>       columnWidths             = default,
        Bindable<IReadOnlyList<HorizontalAlign>?> columnAlignments         = default,
        Bindable<Spacing>                         cellPadding              = default,
        Bindable<Spacing>                         margin                   = default,
        Bindable<IReadOnlyList<HorizontalAlign>?> headerAlignments         = default,
        Bindable<IReadOnlyList<VerticalAlign>?>   columnVerticalAlignments = default
    ) {
        Data = data;

        if (headers.Value is not null)
            Headers = headers;

        if (style.Value is not null)
            Style = style;

        if (columnWidths.Value is not null)
            ColumnWidths = columnWidths;

        if (columnAlignments.Value is not null)
            ColumnAlignments = columnAlignments;

        if (cellPadding.Value is not null)
            CellPadding = cellPadding;

        if (margin.Value is not null)
            Margin = margin;

        if (headerAlignments.Value is not null)
            HeaderAlignments = headerAlignments;

        if (columnVerticalAlignments.Value is not null)
            ColumnVerticalAlignments = columnVerticalAlignments;
    }

    public Table(IElement[,] data, ITableStyle? style = null)
        : this(null, new List2D<IElement>(data), style) { }

    public Table(IReadOnlyList<IElement>? headers, IElement[,] data, ITableStyle? style = null)
        : this(headers, new List2D<IElement>(data), style) { }

    public Table(IReadOnlyList<IReadOnlyList<IElement>> rows)
        : this(null, FromRows(rows)) { }

    public Table(
        IReadOnlyList<IElement>?               headers,
        IReadOnlyList<IReadOnlyList<IElement>> rows,
        ITableStyle?                           style                    = null,
        IReadOnlyList<Dimension>?              columnWidths             = null,
        IReadOnlyList<HorizontalAlign>?        columnAlignments         = null,
        Spacing?                               cellPadding              = null,
        Spacing?                               margin                   = null,
        IReadOnlyList<HorizontalAlign>?        headerAlignments         = null,
        IReadOnlyList<VerticalAlign>?          columnVerticalAlignments = null
    ) : this(headers, FromRows(rows), style, columnWidths, columnAlignments, cellPadding, margin, headerAlignments, columnVerticalAlignments) { }

    public static List2D<IElement> FromRows(IReadOnlyList<IReadOnlyList<IElement>> rows) {
        if (rows == null || rows.Count == 0)
            return new List2D<IElement>();

        int numRows = rows.Count;
        int numCols = 0;

        for (int r = 0; r < numRows; r++) {
            if (rows[r] != null && rows[r].Count > numCols)
                numCols = rows[r].Count;
        }

        var list = new List2D<IElement>(numCols, numRows);
        list.Resize(numCols, numRows);

        for (int r = 0; r < numRows; r++) {
            var row = rows[r];

            if (row == null)
                continue;

            for (int c = 0; c < row.Count; c++) {
                list[c, r] = row[c];
            }
        }

        return list;
    }

    public static List2D<IElement> FromColumns(IReadOnlyList<IReadOnlyList<IElement>> columns) {
        if (columns == null || columns.Count == 0)
            return new List2D<IElement>();

        int numCols = columns.Count;
        int numRows = 0;

        for (int c = 0; c < numCols; c++) {
            if (columns[c] != null && columns[c].Count > numRows)
                numRows = columns[c].Count;
        }

        var list = new List2D<IElement>(numCols, numRows);
        list.Resize(numCols, numRows);

        for (int c = 0; c < numCols; c++) {
            var col = columns[c];

            if (col == null)
                continue;

            for (int r = 0; r < col.Count; r++) {
                list[c, r] = col[r];
            }
        }

        return list;
    }

    public static Table FromText(
        IReadOnlyList<string>?          headers,
        IReadOnlyList2D<string>         data,
        ITableStyle?                    style                    = null,
        IReadOnlyList<Dimension>?       columnWidths             = null,
        IReadOnlyList<HorizontalAlign>? columnAlignments         = null,
        Spacing?                        cellPadding              = null,
        Spacing?                        margin                   = null,
        IReadOnlyList<HorizontalAlign>? headerAlignments         = null,
        IReadOnlyList<VerticalAlign>?   columnVerticalAlignments = null
    ) {
        IElement[]? headerElements = null;

        if (headers != null) {
            headerElements = new IElement[headers.Count];

            for (int i = 0; i < headers.Count; i++) {
                var hAlign = (headerAlignments != null && i < headerAlignments.Count)
                    ? headerAlignments[i]
                    : (columnAlignments != null && i < columnAlignments.Count)
                        ? columnAlignments[i]
                        : HorizontalAlign.LEFT;

                headerElements[i] = new Text(
                    new AnsiString(headers[i]),
                    textAlign: new NKAlign(hAlign, VerticalAlign.TOP)
                );
            }
        }

        var dataElements = new List2D<IElement>(data.XCount, data.YCount);
        dataElements.Resize(data.XCount, data.YCount);

        for (int c = 0; c < data.XCount; c++) {
            var colAlign = (columnAlignments != null && c < columnAlignments.Count)
                ? columnAlignments[c]
                : HorizontalAlign.LEFT;

            var vAlign = (columnVerticalAlignments != null && c < columnVerticalAlignments.Count)
                ? columnVerticalAlignments[c]
                : VerticalAlign.TOP;

            for (int r = 0; r < data.YCount; r++) {
                dataElements[c, r] = new Text(
                    new AnsiString(data[c, r]),
                    textAlign: new NKAlign(colAlign, vAlign)
                );
            }
        }

        return new Table(
            headers: headerElements,
            data: dataElements,
            style: style,
            columnWidths: columnWidths,
            columnAlignments: columnAlignments,
            cellPadding: cellPadding,
            margin: margin,
            headerAlignments: headerAlignments,
            columnVerticalAlignments: columnVerticalAlignments
        );
    }

    public static Table FromText(
        IReadOnlyList2D<string>         data,
        ITableStyle?                    style                    = null,
        IReadOnlyList<Dimension>?       columnWidths             = null,
        IReadOnlyList<HorizontalAlign>? columnAlignments         = null,
        Spacing?                        cellPadding              = null,
        Spacing?                        margin                   = null,
        IReadOnlyList<VerticalAlign>?   columnVerticalAlignments = null
    ) =>
        FromText(null, data, style, columnWidths, columnAlignments, cellPadding, margin, null, columnVerticalAlignments);

    public static Table FromText(
        IReadOnlyList<string>?               headers,
        IReadOnlyList<IReadOnlyList<string>> rows,
        ITableStyle?                         style                    = null,
        IReadOnlyList<Dimension>?            columnWidths             = null,
        IReadOnlyList<HorizontalAlign>?      columnAlignments         = null,
        Spacing?                             cellPadding              = null,
        Spacing?                             margin                   = null,
        IReadOnlyList<HorizontalAlign>?      headerAlignments         = null,
        IReadOnlyList<VerticalAlign>?        columnVerticalAlignments = null
    ) {
        IElement[]? headerElements = null;

        if (headers != null) {
            headerElements = new IElement[headers.Count];

            for (int i = 0; i < headers.Count; i++) {
                var hAlign = (headerAlignments != null && i < headerAlignments.Count)
                    ? headerAlignments[i]
                    : (columnAlignments != null && i < columnAlignments.Count)
                        ? columnAlignments[i]
                        : HorizontalAlign.LEFT;

                headerElements[i] = new Text(
                    new AnsiString(headers[i]),
                    textAlign: new NKAlign(hAlign, VerticalAlign.TOP)
                );
            }
        }

        int numRows = rows.Count;
        int numCols = 0;

        for (int r = 0; r < numRows; r++) {
            if (rows[r] != null && rows[r].Count > numCols)
                numCols = rows[r].Count;
        }

        var dataElements = new List2D<IElement>(numCols, numRows);
        dataElements.Resize(numCols, numRows);

        for (int r = 0; r < numRows; r++) {
            var row = rows[r];

            if (row == null)
                continue;

            for (int c = 0; c < row.Count; c++) {
                var colAlign = (columnAlignments != null && c < columnAlignments.Count)
                    ? columnAlignments[c]
                    : HorizontalAlign.LEFT;

                var vAlign = (columnVerticalAlignments != null && c < columnVerticalAlignments.Count)
                    ? columnVerticalAlignments[c]
                    : VerticalAlign.TOP;

                dataElements[c, r] = new Text(
                    new AnsiString(row[c]),
                    textAlign: new NKAlign(colAlign, vAlign)
                );
            }
        }

        return new Table(
            headers: headerElements,
            data: dataElements,
            style: style,
            columnWidths: columnWidths,
            columnAlignments: columnAlignments,
            cellPadding: cellPadding,
            margin: margin,
            headerAlignments: headerAlignments,
            columnVerticalAlignments: columnVerticalAlignments
        );
    }

    public static Table FromText(
        IReadOnlyList<IReadOnlyList<string>> rows,
        ITableStyle?                         style                    = null,
        IReadOnlyList<Dimension>?            columnWidths             = null,
        IReadOnlyList<HorizontalAlign>?      columnAlignments         = null,
        Spacing?                             cellPadding              = null,
        Spacing?                             margin                   = null,
        IReadOnlyList<VerticalAlign>?        columnVerticalAlignments = null
    ) =>
        FromText(null, rows, style, columnWidths, columnAlignments, cellPadding, margin, null, columnVerticalAlignments);

    private static (IElement?[,] Matrix, int NumRows, int NumCols, bool HasHeader) BuildCellMatrix(
        IReadOnlyList<IElement>?   headers,
        IReadOnlyList2D<IElement>? data
    ) {
        bool hasHeader = headers is { Count: > 0 };
        int  dataCols  = data?.XCount ?? 0;
        int  dataRows  = data?.YCount ?? 0;

        int rowCount = (hasHeader ? 1 : 0) + dataRows;
        int colCount = Math.Max(hasHeader ? headers!.Count : 0, dataCols);

        if (rowCount == 0 || colCount == 0)
            return (new IElement?[0, 0], 0, 0, false);

        var matrix = new IElement?[rowCount, colCount];

        int startRow = 0;

        if (hasHeader) {
            for (int c = 0; c < headers!.Count; c++)
                matrix[0, c] = headers[c];

            startRow = 1;
        }

        if (data != null) {
            for (int r = 0; r < dataRows; r++) {
                for (int c = 0; c < dataCols; c++) {
                    matrix[startRow + r, c] = data[c, r];
                }
            }
        }

        return (matrix, rowCount, colCount, hasHeader);
    }

    private static Size2D[,] BuildCellSizes(
        IElement?[,]              matrix,
        int                       numRows,
        int                       numCols,
        IReadOnlyList<Dimension>? colDefs,
        ResolvedSpacing           cellPadding,
        LayoutContext             context,
        bool                      ungrown = false
    ) {
        var colWidths  = new int[numCols];
        var rowHeights = new int[numRows];

        for (int r = 0; r < numRows; r++) {
            for (int c = 0; c < numCols; c++) {
                var cell = matrix[r, c];

                if (cell != null) {
                    var desired = cell.Measure(context);
                    int w       = desired.X + cellPadding.Horizontal;
                    int h       = desired.Y + cellPadding.Vertical;

                    if (w > colWidths[c])
                        colWidths[c] = w;

                    if (h > rowHeights[r])
                        rowHeights[r] = h;
                }
                else {
                    if (cellPadding.Horizontal > colWidths[c])
                        colWidths[c] = cellPadding.Horizontal;

                    if (cellPadding.Vertical > rowHeights[r])
                        rowHeights[r] = cellPadding.Vertical;
                }
            }
        }

        if (colDefs != null) {
            for (int c = 0; c < numCols && c < colDefs.Count; c++) {
                var dim      = colDefs[c];
                int resolved = dim.Resolve(context.Parent.X, context.Screen, colWidths[c], ungrown);

                if (resolved > 0)
                    colWidths[c] = resolved;
            }
        }

        for (int r = 0; r < numRows; r++) {
            if (rowHeights[r] < 1)
                rowHeights[r] = 1;
        }

        var elements = new Size2D[numRows, numCols];

        for (int r = 0; r < numRows; r++) {
            for (int c = 0; c < numCols; c++) {
                elements[r, c] = new Size2D(colWidths[c], rowHeights[r]);
            }
        }

        return elements;
    }

    public Size2D Measure(LayoutContext context) {
        var (matrix, numRows, numCols, _) = BuildCellMatrix(Headers.AsT, Data.AsT);

        if (numRows == 0 || numCols == 0)
            return Size2D.Zero;

        var margin      = Margin.AsT.Resolve(context, ungrown: true);
        var cellPadding = CellPadding.AsT.Resolve(context, ungrown: true);

        var elements = BuildCellSizes(
            matrix,
            numRows,
            numCols,
            ColumnWidths.AsT,
            cellPadding,
            context,
            ungrown: true
        );

        var style     = Style.AsT ?? NKTableStyle.GetRounded();
        var tableSize = style.GetTotalSize(elements);

        return new Size2D(
            tableSize.X + margin.Horizontal,
            tableSize.Y + margin.Vertical
        );
    }

    private (HorizontalAlign Horizontal, VerticalAlign Vertical) GetCellAlignment(
        int  col,
        bool isHeader
    ) {
        HorizontalAlign h;

        if (isHeader) {
            var hAligns = HeaderAlignments.AsT;

            if (hAligns != null && col < hAligns.Count) {
                h = hAligns[col];
            }
            else {
                var cAligns = ColumnAlignments.AsT;
                h = (cAligns != null && col < cAligns.Count) ? cAligns[col] : DefaultAlignment.AsT;
            }
        }
        else {
            var cAligns = ColumnAlignments.AsT;
            h = (cAligns != null && col < cAligns.Count) ? cAligns[col] : DefaultAlignment.AsT;
        }

        var vAligns = ColumnVerticalAlignments.AsT;
        var v       = (vAligns != null && col < vAligns.Count) ? vAligns[col] : DefaultVerticalAlignment.AsT;

        return (h, v);
    }

    public void Render(RenderContext context, Size2D parent) {
        var (matrix, numRows, numCols, hasHeader) = BuildCellMatrix(Headers.AsT, Data.AsT);

        if (numRows == 0 || numCols == 0)
            return;

        var layoutContext = new LayoutContext(parent, context.Size);
        var margin        = Margin.AsT.Resolve(layoutContext);
        var cellPadding   = CellPadding.AsT.Resolve(layoutContext);

        var elements = BuildCellSizes(
            matrix,
            numRows,
            numCols,
            ColumnWidths.AsT,
            cellPadding,
            layoutContext,
            ungrown: false
        );

        var style = Style.AsT ?? NKTableStyle.GetRounded();

        var tableOrigin = new Point2D(margin.Left, margin.Top);

        style.Draw(context.Origin + tableOrigin, elements, context.Canvas, context.ZIndex);

        var offsets = style.GetElementOffsets(elements);

        for (int r = 0; r < numRows; r++) {
            bool isHeader = hasHeader && r == 0;

            for (int c = 0; c < numCols; c++) {
                var cell = matrix[r, c];

                if (cell is null)
                    continue;

                var cellBoxOffset = tableOrigin + offsets[r, c];
                var cellBoxSize   = elements[r, c];

                var childAreaOffset = cellBoxOffset + cellPadding.Offset;

                var childAreaSize = new Size2D(
                    Math.Max(0, cellBoxSize.X - cellPadding.Horizontal),
                    Math.Max(0, cellBoxSize.Y - cellPadding.Vertical)
                );

                if (childAreaSize.X <= 0 || childAreaSize.Y <= 0)
                    continue;

                var (hAlign, vAlign) = GetCellAlignment(c, isHeader);

                var desired = cell.Measure(new LayoutContext(childAreaSize, context.Size));

                int renderW = Math.Min(childAreaSize.X, desired.X);
                int renderH = Math.Min(childAreaSize.Y, desired.Y);

                if (renderW <= 0 || renderH <= 0)
                    continue;

                int offsetX = hAlign switch {
                    HorizontalAlign.CENTER => Math.Max(0, childAreaSize.X - renderW) / 2,
                    HorizontalAlign.RIGHT  => Math.Max(0, childAreaSize.X - renderW),
                    _                      => 0
                };

                int offsetY = vAlign switch {
                    VerticalAlign.CENTER => Math.Max(0, childAreaSize.Y - renderH) / 2,
                    VerticalAlign.BOTTOM => Math.Max(0, childAreaSize.Y - renderH),
                    _                    => 0
                };

                var childOffset     = childAreaOffset + new Point2D(offsetX, offsetY);
                var childRenderSize = new Size2D(renderW, renderH);

                using (context.PushScope(
                        offset: childOffset,
                        clip: new Rect2D(Point2D.Zero,        childRenderSize),
                        contextArea: new Rect2D(Point2D.Zero, childRenderSize)
                    )) {
                    cell.Render(context, childRenderSize);
                }
            }
        }
    }
}