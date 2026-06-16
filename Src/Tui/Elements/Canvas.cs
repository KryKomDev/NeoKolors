// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A layout panel that supports absolute coordinate-based positioning of its child elements.
/// </summary>
public class Canvas : Panel {
    
    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        ReadOnly = true
    };

    public Canvas() : base(DefaultStyles) { }

    protected override Size2D MeasureOverride(Size2D availableSize) {
        int maxWidth = 0;
        int maxHeight = 0;

        foreach (var child in _children) {
            if (child == null) continue;
            child.Measure(availableSize);
            var childSize = child.DesiredSize;
            var childPos = child.Style.Position;
            int childX = childPos.X.ToScalar(availableSize.X);
            int childY = childPos.Y.ToScalar(availableSize.Y);

            maxWidth = Math.Max(maxWidth, childX + childSize.X);
            maxHeight = Math.Max(maxHeight, childY + childSize.Y);
        }

        return new Size2D(maxWidth, maxHeight);
    }

    protected override Size2D ArrangeOverride(Size2D finalSize) {
        var pos = RenderBounds.Lower;
        foreach (var child in _children) {
            if (child == null) continue;

            var childStyle = child.Style;
            var childPosProp = childStyle.Position;
            
            int childX = childPosProp.X.ToScalar(RenderLayout.Content.SizeX);
            int childY = childPosProp.Y.ToScalar(RenderLayout.Content.SizeY);

            var childPos = pos + RenderLayout.Content.Lower + new Point2D(childX, childY);
            child.Arrange(new Area2D(childPos, child.DesiredSize));
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
