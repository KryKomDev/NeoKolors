// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements;

public class FlexItem : IElement {
    public required Bindable<IElement>   Element   { get; init; }
    public          Bindable<int>        Grow      { get; init; } = 0;
    public          Bindable<Dimension?> Basis     { get; init; } = default;
    public          Bindable<FlexAlign?> AlignSelf { get; init; } = default;

    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public FlexItem(IElement element, int grow = 0, Dimension? basis = null, FlexAlign? alignSelf = null) {
        Element   = element;
        Grow      = grow;
        Basis     = basis;
        AlignSelf = alignSelf;
    }

    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public FlexItem(
        Bindable<IElement>   element,
        Bindable<int>        grow      = default,
        Bindable<Dimension?> basis     = default,
        Bindable<FlexAlign?> alignSelf = default
    ) {
        Element   = element;
        Grow      = grow;
        Basis     = basis;
        AlignSelf = alignSelf;
    }

    public Size2D Measure(LayoutContext context) {
        return Element.AsT?.Measure(context) ?? Size2D.Zero;
    }

    public void Render(RenderContext context, Size2D parent) {
        Element.AsT?.Render(context, parent);
    }
}
