// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements;

public class List : IElement {
    public Bindable<IReadOnlyList<IElement>> Items          { get; init; }
    public Bindable<ListPointGenerator>      PointGenerator { get; init; } = ListPoints.Bullet;
    public Bindable<NKStyle?>                PointStyle     { get; init; } = default;
    public Bindable<int>                     PointGap       { get; init; } = 1;
    public Bindable<int>                     Gap            { get; init; } = 0;
    public Bindable<Spacing>                 Padding        { get; init; } = default;
    public Bindable<Spacing>                 Margin         { get; init; } = default;

    public List(params IElement[] items) {
        Items = items;
    }

    public List(
        Bindable<IReadOnlyList<IElement>> items,
        Bindable<ListPointGenerator>      pointGenerator = default,
        Bindable<NKStyle?>                pointStyle     = default,
        Bindable<int>                     pointGap       = default,
        Bindable<int>                     gap            = default,
        Bindable<Spacing>                 padding        = default,
        Bindable<Spacing>                 margin         = default
    ) {
        Items = items;

        if (pointGenerator.Value is not null)
            PointGenerator = pointGenerator;

        if (pointStyle.Value is not null)
            PointStyle = pointStyle;

        if (pointGap.Value is not null)
            PointGap = pointGap;

        if (gap.Value is not null)
            Gap = gap;

        if (padding.Value is not null)
            Padding = padding;

        if (margin.Value is not null)
            Margin = margin;
    }

    public static ListItem Item(IElement element, AnsiString? point = null, NKStyle? pointStyle = null) => new(element, point, pointStyle);

    private AnsiString GetPointFor(IElement item, int index1Based) {
        if (item is ListItem { Point.AsT: { Length: > 0 } customPt })
            return customPt;

        var generator = PointGenerator.AsT ?? ListPoints.Bullet;

        return generator(index1Based);
    }

    public Size2D Measure(LayoutContext context) {
        var items = Items.AsT;

        if (items is null || items.Count == 0)
            return Size2D.Zero;

        var margin  = Margin.AsT.Resolve(context, ungrown: true);
        var padding = Padding.AsT.Resolve(context, ungrown: true);

        var overheadX = margin.Horizontal + padding.Horizontal;
        var overheadY = margin.Vertical   + padding.Vertical;

        int pointGap = PointGap.AsT;
        int gap      = Gap.AsT;

        int count         = items.Count;
        int maxPointWidth = 0;

        for (int i = 0; i < count; i++) {
            var pt = GetPointFor(items[i], i + 1);

            if (pt.Length > maxPointWidth)
                maxPointWidth = pt.Length;
        }

        int pointPrefix = maxPointWidth + pointGap;

        var availableForChild = new Size2D(
            Math.Max(0, context.Parent.X - overheadX - pointPrefix),
            Math.Max(0, context.Parent.Y - overheadY)
        );

        int maxContentWidth = 0;
        int totalHeight     = 0;

        for (int i = 0; i < count; i++) {
            var item = items[i];

            if (item is null)
                continue;

            var childDesired = item.Measure(new LayoutContext(availableForChild, context.Screen));

            if (childDesired.X > maxContentWidth)
                maxContentWidth = childDesired.X;

            totalHeight += childDesired.Y;
        }

        if (count > 1) {
            totalHeight += (count - 1) * gap;
        }

        return new Size2D(
            maxContentWidth + pointPrefix + overheadX,
            totalHeight     + overheadY
        );
    }

    public void Render(RenderContext context, Size2D parent) {
        var items = Items.AsT;

        if (items is null || items.Count == 0)
            return;

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

        int pointGap = PointGap.AsT;
        int gap      = Gap.AsT;
        int count    = items.Count;

        int maxPointWidth = 0;
        var pointStrings  = new AnsiString[count];

        for (int i = 0; i < count; i++) {
            pointStrings[i] = GetPointFor(items[i], i + 1);

            if (pointStrings[i].Length > maxPointWidth)
                maxPointWidth = pointStrings[i].Length;
        }

        int pointPrefix = maxPointWidth + pointGap;
        var childWidth  = Math.Max(0, contentSize.X - pointPrefix);

        int currentY = 0;

        for (int i = 0; i < count; i++) {
            var item = items[i];

            if (item is null)
                continue;

            var childDesired = item.Measure(new LayoutContext(new Size2D(childWidth, contentSize.Y), context.Size));
            int itemHeight   = Math.Max(1, childDesired.Y);

            // Draw bullet / marker (right-aligned inside maxPointWidth)
            var pt      = pointStrings[i];
            var ptStyle = (item as ListItem)?.PointStyle.AsT ?? PointStyle.AsT;
            var ptPos   = contentOrigin + new Point2D(maxPointWidth - pt.Length, currentY);

            if (ptStyle.HasValue) {
                using var _ = context.PushStyle(ptStyle.Value);
                context.Draw(pt, ptPos.X, ptPos.Y, 0);
            }
            else {
                context.Draw(pt, ptPos.X, ptPos.Y, 0);
            }

            // Render item content
            var childOffset = contentOrigin + new Point2D(pointPrefix, currentY);
            var childSize   = new Size2D(childWidth, itemHeight);

            using (context.PushScope(
                    offset: childOffset,
                    clip: new Rect2D(Point2D.Zero,       childSize),
                    contextArea: new Rect2D(Point2D.Zero, childSize)
                )) {
                item.Render(context, childSize);
            }

            currentY += itemHeight + gap;
        }
    }
}