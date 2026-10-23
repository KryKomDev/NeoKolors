// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements;

public class ListItem : IElement {
    public required Bindable<IElement>   Element    { get; init; }
    public          Bindable<AnsiString?> Point      { get; init; } = default;
    public          Bindable<NKStyle?>    PointStyle { get; init; } = default;

    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public ListItem(IElement element, AnsiString? point = null, NKStyle? pointStyle = null) {
        Element    = element;
        Point      = point;
        PointStyle = pointStyle;
    }

    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public ListItem(
        Bindable<IElement>    element,
        Bindable<AnsiString?> point      = default,
        Bindable<NKStyle?>    pointStyle = default
    ) {
        Element    = element;
        Point      = point;
        PointStyle = pointStyle;
    }

    public Size2D Measure(LayoutContext context) {
        return Element.AsT?.Measure(context) ?? Size2D.Zero;
    }

    public void Render(RenderContext context, Size2D parent) {
        Element.AsT?.Render(context, parent);
    }
}
