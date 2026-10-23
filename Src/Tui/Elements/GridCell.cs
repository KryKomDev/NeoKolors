// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements;

public class GridCell : IElement {
    public required Bindable<IElement> Element    { get; init; }
    public          int               Row        { get; init; } = 0;
    public          int               Column     { get; init; } = 0;
    public          int               RowSpan    { get; init; } = 1;
    public          int               ColumnSpan { get; init; } = 1;

    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public GridCell(IElement element, int row = 0, int column = 0, int rowSpan = 1, int columnSpan = 1) {
        Element    = element;
        Row        = row;
        Column     = column;
        RowSpan    = Math.Max(1, rowSpan);
        ColumnSpan = Math.Max(1, columnSpan);
    }

    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public GridCell(
        Bindable<IElement> element,
        int                row        = 0,
        int                column     = 0,
        int                rowSpan    = 1,
        int                columnSpan = 1
    ) {
        Element    = element;
        Row        = row;
        Column     = column;
        RowSpan    = Math.Max(1, rowSpan);
        ColumnSpan = Math.Max(1, columnSpan);
    }

    public Size2D Measure(LayoutContext context) {
        return Element.AsT?.Measure(context) ?? Size2D.Zero;
    }

    public void Render(RenderContext context, Size2D parent) {
        Element.AsT?.Render(context, parent);
    }
}
