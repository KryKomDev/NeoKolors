// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements;

public class Border : IElement {
    public required Bindable<IElement>     Element { get; init; }
    public          Bindable<Spacing>      Padding { get; init; }
    public          Bindable<Spacing>      Margin  { get; init; }
    public          Bindable<IBorderStyle> Style   { get; init; }
    public          Bindable<Sizing>       Sizing  { get; init; } = default;

    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public Border(IElement element) {
        Element = element;
    }

    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public Border(
        Bindable<IElement>     element,
        Bindable<Spacing>      padding = default,
        Bindable<Spacing>      margin  = default,
        Bindable<IBorderStyle> style   = default,
        Bindable<Sizing>       sizing  = default
    ) {
        Element = element;
        Padding = padding;
        Margin  = margin;
        Style   = style;
        Sizing  = sizing;
    }

    public void Render(RenderContext context, Size2D parent) {
        var layoutContext = new LayoutContext(parent, context.Size);
        var actualParent  = parent;
        var sizing        = Sizing.AsT;

        if (sizing != default) {
            actualParent = sizing.ToSize(layoutContext, parent, ungrown: false);
        }

        var margin = Margin.AsT.Resolve(layoutContext);
        var style  = Style.AsT;

        var borderOrigin = new Point2D(margin.Left, margin.Top);

        var borderSize = new Size2D(
            Math.Max(0, actualParent.X - margin.Horizontal),
            Math.Max(0, actualParent.Y - margin.Vertical)
        );

        if (style is not null && borderSize is { X: > 0, Y: > 0 }) {
            style.Draw(
                new Rect2D(context.Origin + borderOrigin, borderSize),
                context.Canvas,
                context.ZIndex
            );
        }

        var contentBox  = style?.GetContentBox(borderSize) ?? new Rect2D(Point2D.Zero, borderSize);
        var contentSize = new Size2D(contentBox.SizeX, contentBox.SizeY);

        var padding     = Padding.AsT.Resolve(new LayoutContext(contentSize, context.Size));
        var childOffset = borderOrigin + contentBox.Lower + padding.Offset;

        var childSize = new Size2D(
            Math.Max(0, contentSize.X - padding.Horizontal),
            Math.Max(0, contentSize.Y - padding.Vertical)
        );

        var childElement = Element.AsT;

        if (childElement is null || childSize is not { X: > 0, Y: > 0 })
            return;

        using var _ = context.PushScope(
            offset: childOffset,
            clip: new Rect2D(Point2D.Zero,        childSize),
            contextArea: new Rect2D(Point2D.Zero, childSize)
        );

        childElement.Render(context, childSize);
    }

    public Size2D Measure(LayoutContext context) {
        var margin  = Margin.AsT.Resolve(context, ungrown: true);
        var padding = Padding.AsT.Resolve(context, ungrown: true);
        var style   = Style.AsT;

        var borderOverhead = style?.GetBorderBox(Size2D.Zero) ?? Size2D.Zero;

        var overheadX = margin.Horizontal + padding.Horizontal + borderOverhead.X;
        var overheadY = margin.Vertical   + padding.Vertical   + borderOverhead.Y;

        var availableParent = context.Parent;
        var sizing          = Sizing.AsT;

        if (sizing != default) {
            if (sizing.Width.Value is Dimension.Fixed fixedW) {
                availableParent = availableParent with {
                    X = fixedW.ToSize(context.Parent.X, context.Screen)
                };
            }

            if (sizing.Height.Value is Dimension.Fixed fixedH) {
                availableParent = availableParent with {
                    Y = fixedH.ToSize(context.Parent.Y, context.Screen)
                };
            }
        }

        var childAvailable = new Size2D(
            Math.Max(0, availableParent.X - overheadX),
            Math.Max(0, availableParent.Y - overheadY)
        );

        var childElement = Element.AsT;

        var childDesired = childElement?.Measure(new LayoutContext(childAvailable, context.Screen)) 
            ?? Size2D.Zero;

        var innerBox = new Size2D(
            childDesired.X + padding.Horizontal,
            childDesired.Y + padding.Vertical
        );

        var borderBox = style?.GetBorderBox(innerBox) ?? innerBox;

        var naturalSize = new Size2D(
            borderBox.X + margin.Horizontal,
            borderBox.Y + margin.Vertical
        );

        return sizing != default ? sizing.ToSize(context, naturalSize, ungrown: false) : naturalSize;
    }
}