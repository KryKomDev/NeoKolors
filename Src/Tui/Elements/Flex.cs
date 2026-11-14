// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements;

public class Flex : IElement {
    public Bindable<IReadOnlyList<IElement>> Children       { get; init; }
    public Bindable<FlexDirection>           Direction      { get; init; } = FlexDirection.ROW;
    public Bindable<JustifyContent>          JustifyContent { get; init; } = NeoKolors.Tui.Core.JustifyContent.START;
    public Bindable<FlexAlign>               AlignItems     { get; init; } = FlexAlign.START;
    public Bindable<int>                     Gap            { get; init; } = 0;
    public Bindable<Spacing>                 Padding        { get; init; } = default;
    public Bindable<Spacing>                 Margin         { get; init; } = default;
    public Bindable<Sizing>                  Sizing         { get; init; } = default;

    public Flex(params IElement[] children) {
        Children = children;
    }

    public Flex(
        Bindable<IReadOnlyList<IElement>> children,
        Bindable<FlexDirection>           direction      = default,
        Bindable<JustifyContent>          justifyContent = default,
        Bindable<FlexAlign>               alignItems     = default,
        Bindable<int>                     gap            = default,
        Bindable<Spacing>                 padding        = default,
        Bindable<Spacing>                 margin         = default,
        Bindable<Sizing>                  sizing         = default
    ) {
        Children = children;

        if (direction.Value is not null)
            Direction = direction;

        if (justifyContent.Value is not null)
            JustifyContent = justifyContent;

        if (alignItems.Value is not null)
            AlignItems = alignItems;

        if (gap.Value is not null)
            Gap = gap;

        if (padding.Value is not null)
            Padding = padding;

        if (margin.Value is not null)
            Margin = margin;

        if (sizing.Value is not null)
            Sizing = sizing;
    }

    public static Flex Row(params IElement[] children) => new(children) { Direction = FlexDirection.ROW };

    public static Flex Column(params IElement[] children) => new(children) { Direction = FlexDirection.COLUMN };

    public static Flex Row(Bindable<Sizing> sizing, params IElement[] children) => new(children) { Direction = FlexDirection.ROW, Sizing = sizing };

    public static Flex Column(Bindable<Sizing> sizing, params IElement[] children) => new(children) { Direction = FlexDirection.COLUMN, Sizing = sizing };

    public static Flex Row(Sizing sizing, params IElement[] children) => new(children) { Direction = FlexDirection.ROW, Sizing = sizing };

    public static Flex Column(Sizing sizing, params IElement[] children) => new(children) { Direction = FlexDirection.COLUMN, Sizing = sizing };

    public static FlexItem Item(IElement child, int grow = 0, Dimension? basis = null, FlexAlign? alignSelf = null) => new(child, grow, basis, alignSelf);

    public Size2D Measure(LayoutContext context) {
        var margin  = Margin.AsT.Resolve(context, true);
        var padding = Padding.AsT.Resolve(context, true);

        var overheadX = margin.Horizontal + padding.Horizontal;
        var overheadY = margin.Vertical   + padding.Vertical;

        var children = Children.AsT;

        if (children is null || children.Count == 0) {
            var s = Sizing.AsT;

            if (s != default)
                return s.ToSize(context, new Size2D(overheadX, overheadY), false);

            return Size2D.Zero;
        }

        var availableParent = context.Parent;
        var sizing          = Sizing.AsT;

        if (sizing != default) {
            if (sizing.Width.Value is Dimension.Fixed fixedW)
                availableParent = availableParent with { X = fixedW.ToSize(context.Parent.X, context.Screen) };

            if (sizing.Height.Value is Dimension.Fixed fixedH)
                availableParent = availableParent with { Y = fixedH.ToSize(context.Parent.Y, context.Screen) };
        }

        var available = new Size2D(
            Math.Max(0, availableParent.X - overheadX),
            Math.Max(0, availableParent.Y - overheadY)
        );

        var dir   = Direction.AsT;
        var isRow = dir is FlexDirection.ROW or FlexDirection.ROW_REVERSE;
        var gap   = Gap.AsT;

        var totalMain = 0;
        var maxCross  = 0;
        var count     = children.Count;

        for (var i = 0; i < count; i++) {
            var child = children[i];

            if (child is null)
                continue;

            Size2D childDesired;

            if (child is FlexItem { Basis.AsT: { } basis }) {
                var basisVal = basis.Resolve(
                    isRow ? available.X : available.Y,
                    context.Screen,
                    ungrown: true
                );

                var subLayout = isRow
                    ? new LayoutContext(new Size2D(basisVal,    available.Y), context.Screen)
                    : new LayoutContext(new Size2D(available.X, basisVal),    context.Screen);

                childDesired = child.Measure(subLayout);

                childDesired = isRow
                    ? new Size2D(basisVal,       childDesired.Y)
                    : new Size2D(childDesired.X, basisVal);
            }
            else {
                childDesired = child.Measure(new LayoutContext(available, context.Screen));
            }

            var childMain  = isRow ? childDesired.X : childDesired.Y;
            var childCross = isRow ? childDesired.Y : childDesired.X;

            totalMain += childMain;

            if (childCross > maxCross)
                maxCross = childCross;
        }

        if (count > 1)
            totalMain += (count - 1) * gap;

        var naturalSize = isRow
            ? new Size2D(totalMain + overheadX, maxCross  + overheadY)
            : new Size2D(maxCross  + overheadX, totalMain + overheadY);

        if (sizing != default)
            return sizing.ToSize(context, naturalSize, false);

        return naturalSize;
    }

    public void Render(RenderContext context, Size2D parent) {
        var children = Children.AsT;

        if (children is null || children.Count == 0)
            return;

        var layoutContext = new LayoutContext(parent, context.Size);
        var actualParent  = parent;
        var sizing        = Sizing.AsT;

        if (sizing != default)
            actualParent = sizing.ToSize(layoutContext, parent, false);

        var margin  = Margin.AsT.Resolve(layoutContext);
        var padding = Padding.AsT.Resolve(layoutContext);

        var contentOrigin = new Point2D(margin.Left + padding.Left, margin.Top + padding.Top);

        var contentSize = new Size2D(
            Math.Max(0, actualParent.X - margin.Horizontal - padding.Horizontal),
            Math.Max(0, actualParent.Y - margin.Vertical   - padding.Vertical)
        );

        if (contentSize.X <= 0 || contentSize.Y <= 0)
            return;

        var dir       = Direction.AsT;
        var isRow     = dir is FlexDirection.ROW or FlexDirection.ROW_REVERSE;
        var isReverse = dir is FlexDirection.ROW_REVERSE or FlexDirection.COLUMN_REVERSE;

        var availableMain  = isRow ? contentSize.X : contentSize.Y;
        var availableCross = isRow ? contentSize.Y : contentSize.X;
        var gap            = Gap.AsT;
        var count          = children.Count;

        var items = new (IElement Element, int Grow, Size2D Desired, int BaseMain, FlexAlign Align)[count];

        var totalGrow   = 0;
        var sumBaseMain = 0;

        for (var i = 0; i < count; i++) {
            var        child = children[i];
            var        grow  = 0;
            var        align = AlignItems.AsT;
            Dimension? basis = null;

            if (child is FlexItem flexItem) {
                grow  = flexItem.Grow.AsT;
                align = flexItem.AlignSelf.AsT ?? AlignItems.AsT;
                basis = flexItem.Basis.AsT;
            }

            var childContext = new LayoutContext(contentSize, context.Size);
            var desired      = child.Measure(childContext);

            int baseMain;

            if (basis.HasValue)
                baseMain = basis.Value.Resolve(availableMain, context.Size, ungrown: true);
            else
                baseMain = isRow ? desired.X : desired.Y;

            items[i]    =  (child, grow, desired, baseMain, align);
            totalGrow   += grow;
            sumBaseMain += baseMain;
        }

        var totalGaps     = count > 1 ? (count        - 1) * gap : 0;
        var remainingMain = Math.Max(0, availableMain - sumBaseMain - totalGaps);

        var allocatedMain      = new int[count];
        var allocatedCross     = new int[count];
        var totalAllocatedMain = 0;

        for (var i = 0; i < count; i++) {
            var mainSize = items[i].BaseMain;

            if (items[i].Grow > 0 && totalGrow > 0 && remainingMain > 0) {
                var flexShare = (int)MathF.Round((float)remainingMain * items[i].Grow / totalGrow);
                mainSize += flexShare;
            }

            allocatedMain[i]   =  mainSize;
            totalAllocatedMain += mainSize;

            if (items[i].Align == FlexAlign.STRETCH) {
                allocatedCross[i] = availableCross;
            }
            else {
                var childCross = isRow ? items[i].Desired.Y : items[i].Desired.X;
                allocatedCross[i] = Math.Min(availableCross, childCross);
            }
        }

        var freeSpace    = Math.Max(0, availableMain - totalAllocatedMain - totalGaps);
        var startOffset  = 0;
        var effectiveGap = gap;

        var justify = JustifyContent.AsT;

        if (freeSpace > 0 && totalGrow == 0)
            switch (justify) {
                case NeoKolors.Tui.Core.JustifyContent.START:  startOffset = 0; break;
                case NeoKolors.Tui.Core.JustifyContent.END:    startOffset = freeSpace; break;
                case NeoKolors.Tui.Core.JustifyContent.CENTER: startOffset = freeSpace / 2; break;
                case NeoKolors.Tui.Core.JustifyContent.SPACE_BETWEEN: {
                    startOffset = 0;

                    if (count > 1)
                        effectiveGap = gap + freeSpace / (count - 1);

                    break;
                }
                case NeoKolors.Tui.Core.JustifyContent.SPACE_AROUND: {
                    if (count > 0) {
                        var unit = freeSpace / count;
                        startOffset  = unit / 2;
                        effectiveGap = gap + unit;
                    }

                    break;
                }
                case NeoKolors.Tui.Core.JustifyContent.SPACE_EVENLY: {
                    if (count > 0) {
                        var unit = freeSpace / (count + 1);
                        startOffset  = unit;
                        effectiveGap = gap + unit;
                    }

                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(justify));
            }

        var currentMain = startOffset;

        for (var step = 0; step < count; step++) {
            var i = isReverse ? count - 1 - step : step;

            var mainSize  = allocatedMain[i];
            var crossSize = allocatedCross[i];

            var crossOffset = items[i].Align switch {
                FlexAlign.END    => Math.Max(0, availableCross - crossSize),
                FlexAlign.CENTER => Math.Max(0, availableCross - crossSize) / 2,
                _                => 0
            };

            var childRelativePos = isRow
                ? new Point2D(currentMain, crossOffset)
                : new Point2D(crossOffset, currentMain);

            var childSize = isRow
                ? new Size2D(mainSize,  crossSize)
                : new Size2D(crossSize, mainSize);

            var childGlobalOffset = contentOrigin + childRelativePos;

            using (context.PushScope(
                    childGlobalOffset,
                    new Rect2D(Point2D.Zero,              childSize),
                    contextArea: new Rect2D(Point2D.Zero, childSize)
                )) {
                items[i].Element.Render(context, childSize);
            }

            currentMain += mainSize + effectiveGap;
        }
    }
}

public class FlexBox : Flex {
    public FlexBox(params IElement[] children) : base(children) { }

    public FlexBox(
        Bindable<IReadOnlyList<IElement>> children,
        Bindable<FlexDirection>           direction      = default,
        Bindable<JustifyContent>          justifyContent = default,
        Bindable<FlexAlign>               alignItems     = default,
        Bindable<int>                     gap            = default,
        Bindable<Spacing>                 padding        = default,
        Bindable<Spacing>                 margin         = default,
        Bindable<Sizing>                  sizing         = default
    ) : base(children, direction, justifyContent, alignItems, gap, padding, margin, sizing) { }

    public FlexBox(Bindable<Sizing> sizing, params IElement[] children) : base(children) {
        Sizing = sizing;
    }

    public FlexBox(Sizing sizing, params IElement[] children) : base(children) {
        Sizing = sizing;
    }

    public new static FlexBox Row(params IElement[] children) => new(children) { Direction = FlexDirection.ROW };

    public new static FlexBox Column(params IElement[] children) => new(children) { Direction = FlexDirection.COLUMN };

    public new static FlexBox Row(Bindable<Sizing> sizing, params IElement[] children) => new(sizing, children) { Direction = FlexDirection.ROW };

    public new static FlexBox Column(Bindable<Sizing> sizing, params IElement[] children) => new(sizing, children) { Direction = FlexDirection.COLUMN };

    public new static FlexBox Row(Sizing sizing, params IElement[] children) => new(sizing, children) { Direction = FlexDirection.ROW };

    public new static FlexBox Column(Sizing sizing, params IElement[] children) => new(sizing, children) { Direction = FlexDirection.COLUMN };
}