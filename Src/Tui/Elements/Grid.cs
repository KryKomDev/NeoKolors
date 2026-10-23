// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements;

public class Grid : IElement {
    public Bindable<IReadOnlyList<Dimension>> Columns   { get; init; } = Array.Empty<Dimension>();
    public Bindable<IReadOnlyList<Dimension>> Rows      { get; init; } = Array.Empty<Dimension>();
    public Bindable<IReadOnlyList<GridCell>>  Children  { get; init; } = Array.Empty<GridCell>();
    public Bindable<int>                      RowGap    { get; init; } = 0;
    public Bindable<int>                      ColumnGap { get; init; } = 0;
    public Bindable<Spacing>                  Padding   { get; init; } = default;
    public Bindable<Spacing>                  Margin    { get; init; } = default;

    public Grid(params GridCell[] children) {
        Children = children;
    }

    public Grid(
        IReadOnlyList<Dimension> columns,
        IReadOnlyList<Dimension> rows,
        params GridCell[]        children
    ) {
        Columns  = columns;
        Rows     = rows;
        Children = children;
    }

    public Grid(
        Bindable<IReadOnlyList<GridCell>>  children,
        Bindable<IReadOnlyList<Dimension>> columns   = default,
        Bindable<IReadOnlyList<Dimension>> rows      = default,
        Bindable<int>                      rowGap    = default,
        Bindable<int>                      columnGap = default,
        Bindable<Spacing>                  padding   = default,
        Bindable<Spacing>                  margin    = default
    ) {
        Children = children;

        if (columns.Value is not null)
            Columns = columns;

        if (rows.Value is not null)
            Rows = rows;

        if (rowGap.Value is not null)
            RowGap = rowGap;

        if (columnGap.Value is not null)
            ColumnGap = columnGap;

        if (padding.Value is not null)
            Padding = padding;

        if (margin.Value is not null)
            Margin = margin;
    }

    public static GridCell Cell(IElement element, int row = 0, int column = 0, int rowSpan = 1, int columnSpan = 1) => new(element, row, column, rowSpan, columnSpan);

    private int GetNumColumns(IReadOnlyList<GridCell> cells, IReadOnlyList<Dimension>? cols) {
        int maxFromCells = 0;

        for (int i = 0; i < cells.Count; i++) {
            int end = cells[i].Column + cells[i].ColumnSpan;

            if (end > maxFromCells)
                maxFromCells = end;
        }

        int fromCols = cols?.Count ?? 0;

        return Math.Max(1, Math.Max(maxFromCells, fromCols));
    }

    private int GetNumRows(IReadOnlyList<GridCell> cells, IReadOnlyList<Dimension>? rows) {
        int maxFromCells = 0;

        for (int i = 0; i < cells.Count; i++) {
            int end = cells[i].Row + cells[i].RowSpan;

            if (end > maxFromCells)
                maxFromCells = end;
        }

        int fromRows = rows?.Count ?? 0;

        return Math.Max(1, Math.Max(maxFromCells, fromRows));
    }

    private int[] ResolveTracks(
        IReadOnlyList<Dimension>? trackDefs,
        int                       numTracks,
        int                       availableSpace,
        int                       gap,
        int[]                     autoSizes,
        Size2D                    screen,
        bool                      ungrown = false
    ) {
        var resolved   = new int[numTracks];
        int totalGrow  = 0;
        int nonGrowSum = 0;

        for (int t = 0; t < numTracks; t++) {
            var dim = (trackDefs != null && t < trackDefs.Count) ? trackDefs[t] : Dimension.Auto;

            if (dim.Value is Dimension.Grow grow) {
                if (ungrown) {
                    resolved[t] =  grow.Min.ToSize(availableSpace, screen);
                    nonGrowSum  += resolved[t];
                }
                else {
                    totalGrow += grow.Weight;
                }
            }
            else if (dim.Value is Dimension.Fit fit) {
                resolved[t] =  Math.Clamp(autoSizes[t], fit.Min.ToSize(availableSpace, screen), fit.Max.ToSize(availableSpace, screen));
                nonGrowSum  += resolved[t];
            }
            else if (dim.Value is Dimension.Fixed f) {
                resolved[t] =  f.ToSize(availableSpace, screen);
                nonGrowSum  += resolved[t];
            }
            else {
                resolved[t] =  autoSizes[t];
                nonGrowSum  += resolved[t];
            }
        }

        int totalGaps = numTracks > 1 ? (numTracks - 1) * gap : 0;
        int remaining = Math.Max(0, availableSpace - nonGrowSum - totalGaps);

        if (!ungrown && totalGrow > 0 && remaining > 0) {
            for (int t = 0; t < numTracks; t++) {
                var dim = (trackDefs != null && t < trackDefs.Count) ? trackDefs[t] : Dimension.Auto;

                if (dim.Value is Dimension.Grow grow) {
                    int share = (int)MathF.Round((float)remaining * grow.Weight / totalGrow);
                    resolved[t] = Math.Clamp(share, grow.Min.ToSize(availableSpace, screen), grow.Max.ToSize(availableSpace, screen));
                }
            }
        }

        return resolved;
    }

    public Size2D Measure(LayoutContext context) {
        var cells = Children.AsT ?? Array.Empty<GridCell>();
        var cols  = Columns.AsT;
        var rows  = Rows.AsT;

        var margin  = Margin.AsT.Resolve(context, ungrown: true);
        var padding = Padding.AsT.Resolve(context, ungrown: true);

        var overheadX = margin.Horizontal + padding.Horizontal;
        var overheadY = margin.Vertical   + padding.Vertical;

        int numCols = GetNumColumns(cells, cols);
        int numRows = GetNumRows(cells, rows);

        int colGap = ColumnGap.AsT;
        int rowGap = RowGap.AsT;

        var autoColWidths  = new int[numCols];
        var autoRowHeights = new int[numRows];

        // Measure intrinsic sizes of cells
        for (int i = 0; i < cells.Count; i++) {
            var cell = cells[i];

            if (cell.Element.AsT is null)
                continue;

            var desired = cell.Measure(context);

            if (cell.ColumnSpan == 1 && cell.Column < numCols) {
                if (desired.X > autoColWidths[cell.Column])
                    autoColWidths[cell.Column] = desired.X;
            }

            if (cell.RowSpan == 1 && cell.Row < numRows) {
                if (desired.Y > autoRowHeights[cell.Row])
                    autoRowHeights[cell.Row] = desired.Y;
            }
        }

        var colWidths  = ResolveTracks(cols, numCols, Math.Max(0, context.Parent.X - overheadX), colGap, autoColWidths,  context.Screen, ungrown: true);
        var rowHeights = ResolveTracks(rows, numRows, Math.Max(0, context.Parent.Y - overheadY), rowGap, autoRowHeights, context.Screen, ungrown: true);

        int totalWidth = 0;

        for (int c = 0; c < numCols; c++)
            totalWidth += colWidths[c];

        if (numCols > 1)
            totalWidth += (numCols - 1) * colGap;

        int totalHeight = 0;

        for (int r = 0; r < numRows; r++)
            totalHeight += rowHeights[r];

        if (numRows > 1)
            totalHeight += (numRows - 1) * rowGap;

        return new Size2D(totalWidth + overheadX, totalHeight + overheadY);
    }

    public void Render(RenderContext context, Size2D parent) {
        var cells = Children.AsT ?? Array.Empty<GridCell>();

        if (cells.Count == 0)
            return;

        var cols = Columns.AsT;
        var rows = Rows.AsT;

        var layoutContext = new LayoutContext(parent, context.Size);
        var margin        = Margin.AsT.Resolve(layoutContext);
        var padding       = Padding.AsT.Resolve(layoutContext);

        var contentOrigin = new Point2D(margin.Left + padding.Left, margin.Top + padding.Top);

        var contentSize = new Size2D(
            Math.Max(0, parent.X - margin.Horizontal - padding.Horizontal),
            Math.Max(0, parent.Y - margin.Vertical   - padding.Vertical)
        );

        if (contentSize.X <= 0 || contentSize.Y <= 0)
            return;

        int numCols = GetNumColumns(cells, cols);
        int numRows = GetNumRows(cells, rows);

        int colGap = ColumnGap.AsT;
        int rowGap = RowGap.AsT;

        var autoColWidths  = new int[numCols];
        var autoRowHeights = new int[numRows];

        for (int i = 0; i < cells.Count; i++) {
            var cell = cells[i];

            if (cell.Element.AsT is null)
                continue;

            var desired = cell.Measure(new LayoutContext(contentSize, context.Size));

            if (cell.ColumnSpan == 1 && cell.Column < numCols && desired.X > autoColWidths[cell.Column])
                autoColWidths[cell.Column] = desired.X;

            if (cell.RowSpan == 1 && cell.Row < numRows && desired.Y > autoRowHeights[cell.Row])
                autoRowHeights[cell.Row] = desired.Y;
        }

        var colWidths  = ResolveTracks(cols, numCols, contentSize.X, colGap, autoColWidths,  context.Size, ungrown: false);
        var rowHeights = ResolveTracks(rows, numRows, contentSize.Y, rowGap, autoRowHeights, context.Size, ungrown: false);

        // Calculate track offsets
        var colOffsets = new int[numCols];
        int currentX   = 0;

        for (int c = 0; c < numCols; c++) {
            colOffsets[c] =  currentX;
            currentX      += colWidths[c] + colGap;
        }

        var rowOffsets = new int[numRows];
        int currentY   = 0;

        for (int r = 0; r < numRows; r++) {
            rowOffsets[r] =  currentY;
            currentY      += rowHeights[r] + rowGap;
        }

        // Render each cell
        for (int i = 0; i < cells.Count; i++) {
            var cell = cells[i];

            if (cell.Element.AsT is null)
                continue;

            int col     = Math.Clamp(cell.Column,     0, numCols - 1);
            int row     = Math.Clamp(cell.Row,        0, numRows - 1);
            int colSpan = Math.Clamp(cell.ColumnSpan, 1, numCols - col);
            int rowSpan = Math.Clamp(cell.RowSpan,    1, numRows - row);

            int cellX = colOffsets[col];
            int cellY = rowOffsets[row];

            int cellW = 0;

            for (int c = 0; c < colSpan; c++)
                cellW += colWidths[col + c];

            if (colSpan > 1)
                cellW += (colSpan - 1) * colGap;

            int cellH = 0;

            for (int r = 0; r < rowSpan; r++)
                cellH += rowHeights[row + r];

            if (rowSpan > 1)
                cellH += (rowSpan - 1) * rowGap;

            var childOffset = contentOrigin + new Point2D(cellX, cellY);
            var childSize   = new Size2D(cellW, cellH);

            if (childSize.X <= 0 || childSize.Y <= 0)
                continue;

            using (context.PushScope(
                    offset: childOffset,
                    clip: new Rect2D(Point2D.Zero,       childSize),
                    contextArea: new Rect2D(Point2D.Zero, childSize)
                )) {
                cell.Render(context, childSize);
            }
        }
    }
}