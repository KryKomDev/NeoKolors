// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Specifies the unit type of a GridLength.
/// </summary>
public enum GridUnitType {
    AUTO,
    PIXEL,
    STAR
}

/// <summary>
/// Represents the length/size of a Grid row or column definition.
/// </summary>
public struct GridLength {
    public double Value { get; }
    public GridUnitType GridUnitType { get; }

    public GridLength(double value, GridUnitType unitType) {
        Value = value;
        GridUnitType = unitType;
    }

    public static GridLength Auto => new(1.0, GridUnitType.AUTO);
    public static GridLength Star(double value = 1.0) => new(value, GridUnitType.STAR);
    public static GridLength Chars(double value) => new(value, GridUnitType.PIXEL);
}

/// <summary>
/// Defines coordinate parameters for positioning elements inside a Grid.
/// </summary>
public struct GridPosition {
    public int Row { get; set; }
    public int Column { get; set; }
    public int RowSpan { get; set; }
    public int ColumnSpan { get; set; }

    public GridPosition(int row, int col, int rowSpan = 1, int colSpan = 1) {
        Row = row;
        Column = col;
        RowSpan = rowSpan;
        ColumnSpan = colSpan;
    }
}

/// <summary>
/// Defines a flexible grid layout control that aligns content in rows and columns.
/// </summary>
public class Grid : Panel {
    private readonly Dictionary<IElement, GridPosition> _positions = new();

    public List<GridLength> RowDefinitions { get; } = [];
    public List<GridLength> ColumnDefinitions { get; } = [];

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        ReadOnly = true
    };

    public Grid() : base(DefaultStyles) { }

    public void AddChild(IElement child, int row, int column, int rowSpan = 1, int colSpan = 1) {
        base.AddChild(child);
        _positions[child] = new GridPosition(row, column, rowSpan, colSpan);
    }

    public override void RemoveChild(IElement child) {
        base.RemoveChild(child);
        _positions.Remove(child);
    }

    public override void ClearChildren() {
        base.ClearChildren();
        _positions.Clear();
    }

    private int[]? _lastResolvedColWidths;

    private int[] ResolveLengths(List<GridLength>? definitions, int totalSize, bool isRows) {
        if (definitions == null || definitions.Count == 0) {
            return [totalSize];
        }

        int[] resolved = new int[definitions.Count];
        int remaining = totalSize;
        double starTotal = 0;

        // First pass: Resolve fixed sizes (PIXEL)
        for (int i = 0; i < definitions.Count; i++) {
            var def = definitions[i];

            if (def.GridUnitType == GridUnitType.PIXEL) {
                int size = Math.Min((int)def.Value, remaining);
                resolved[i] = size;
                remaining -= size;
            }
            else if (def.GridUnitType == GridUnitType.STAR) {
                starTotal += def.Value;
            }
        }

        // Second pass: Resolve AUTO sizes based on children
        for (int i = 0; i < definitions.Count; i++) {
            var def = definitions[i];

            if (def.GridUnitType == GridUnitType.AUTO) {
                int maxDesired = 0;

                foreach (var child in _children) {
                    if (child == null) continue;
                    if (!_positions.TryGetValue(child, out var cellPos)) {
                        cellPos = new GridPosition(0, 0);
                    }

                    if (isRows) {
                        if (cellPos.Row == i && cellPos.RowSpan == 1) {
                            int cellW = 0;
                            if (_lastResolvedColWidths != null && cellPos.Column < _lastResolvedColWidths.Length) {
                                for (int col = cellPos.Column; col < cellPos.Column + cellPos.ColumnSpan && col < _lastResolvedColWidths.Length; col++) {
                                    cellW += _lastResolvedColWidths[col];
                                }
                            }
                            else {
                                cellW = remaining;
                            }

                            child.Measure(new Size2D(cellW, remaining));
                            maxDesired = Math.Max(maxDesired, child.DesiredSize.Y);
                        }
                    }
                    else {
                        if (cellPos.Column == i && cellPos.ColumnSpan == 1) {
                            child.Measure(new Size2D(remaining, totalSize));
                            maxDesired = Math.Max(maxDesired, child.DesiredSize.X);
                        }
                    }
                }

                int resolvedSize = Math.Max(1, Math.Min(maxDesired, remaining));
                resolved[i] = resolvedSize;
                remaining -= resolvedSize;
            }
        }

        // Third pass: Distribute remaining space to Star definitions proportional to value
        if (starTotal > 0 && remaining > 0) {
            int starCount = 0;
            for (int i = 0; i < definitions.Count; i++) {
                if (definitions[i].GridUnitType == GridUnitType.STAR) starCount++;
            }

            int starIndex = 0;
            int allocated = 0;
            for (int i = 0; i < definitions.Count; i++) {
                var def = definitions[i];

                if (def.GridUnitType != GridUnitType.STAR)
                    continue;

                starIndex++;
                int size;
                if (starIndex == starCount) {
                    size = remaining - allocated;
                }
                else {
                    size = (int)Math.Round(def.Value / starTotal * remaining);
                }
                resolved[i] = size;
                allocated += size;
            }
        }

        if (!isRows) {
            _lastResolvedColWidths = resolved;
        }

        return resolved;
    }

    protected override Size2D MeasureOverride(Size2D availableSize) {
        _lastResolvedColWidths = null;
        int[] colWidths  = ResolveLengths(ColumnDefinitions, availableSize.X, false);
        int[] rowHeights = ResolveLengths(RowDefinitions, availableSize.Y, true);

        foreach (var child in _children) {
            if (child is null) continue;

            if (!_positions.TryGetValue(child, out var cellPos)) {
                cellPos = new GridPosition(0, 0);
            }

            int cellW = 0;
            int cellH = 0;

            for (int col = 0; col < colWidths.Length; col++) {
                if (col >= cellPos.Column && col < cellPos.Column + cellPos.ColumnSpan) {
                    cellW += colWidths[col];
                }
            }

            for (int r = 0; r < rowHeights.Length; r++) {
                if (r >= cellPos.Row && r < cellPos.Row + cellPos.RowSpan) {
                    cellH += rowHeights[r];
                }
            }

            child.Measure(new Size2D(cellW, cellH));
        }

        int totalColWidth = 0;

        foreach (var w in colWidths)
            totalColWidth += w;

        int totalRowHeight = 0;

        foreach (var h in rowHeights)
            totalRowHeight += h;

        return new Size2D(Math.Max(10, totalColWidth), Math.Max(5, totalRowHeight));
    }

    protected override Size2D ArrangeOverride(Size2D finalSize) {
        var pos = RenderBounds.Lower;
        _lastResolvedColWidths = null;
        int[] colWidths = ResolveLengths(ColumnDefinitions, RenderLayout.Content.SizeX, false);
        int[] rowHeights = ResolveLengths(RowDefinitions, RenderLayout.Content.SizeY, true);

        foreach (var child in _children) {
            if (child == null) continue;

            if (!_positions.TryGetValue(child, out var cellPos)) {
                cellPos = new GridPosition(0, 0);
            }

            int cellX = 0;
            int cellY = 0;
            int cellW = 0;
            int cellH = 0;

            for (int col = 0; col < colWidths.Length; col++) {
                if (col < cellPos.Column) cellX += colWidths[col];

                if (col >= cellPos.Column && col < cellPos.Column + cellPos.ColumnSpan) {
                    cellW += colWidths[col];
                }
            }

            for (int r = 0; r < rowHeights.Length; r++) {
                if (r < cellPos.Row) cellY += rowHeights[r];

                if (r >= cellPos.Row && r < cellPos.Row + cellPos.RowSpan) {
                    cellH += rowHeights[r];
                }
            }

            var childBounds = new Area2D(
                pos + RenderLayout.Content.Lower + new Point2D(cellX, cellY),
                new Size2D(cellW, cellH)
            );

            child.Arrange(childBounds);
        }

        return finalSize;
    }

    protected override void RenderCore(ICharCanvas canvas) {
        foreach (var child in _children) {
            child.Render(canvas);
        }
    }

    public override ElementInfo Info { get; } = new();
}