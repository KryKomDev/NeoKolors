// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a top horizontal utility menu strip.
/// Shows menu buttons side-by-side: e.g. [ File ] [ Edit ] [ Help ].
/// </summary>
public class MenuBar : Panel {
    
    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Border = BorderStyle.Borderless,
        Height = Dimension.Chars(1),
        Width = Dimension.Auto,
        ReadOnly = true
    };

    public MenuBar() : base(DefaultStyles) { }

    protected override Size2D MeasureOverride(Size2D availableSize) {
        int width = 2;
        int height = 1;

        foreach (var child in _children) {
            if (child == null) continue;
            child.Measure(availableSize);
            var childSize = child.DesiredSize;
            width += childSize.X + 2;
            height = Math.Max(height, childSize.Y);
        }

        return new Size2D(width, height);
    }

    protected override Size2D ArrangeOverride(Size2D finalSize) {
        var pos = RenderBounds.Lower;
        int offset = 1;

        foreach (var child in _children) {
            if (child == null) continue;

            var childSize = child.DesiredSize;
            var childPos = pos + RenderLayout.Content.Lower + new Point2D(offset, 0);

            child.Arrange(new Area2D(childPos, childSize));
            offset += childSize.X + 2;
        }

        return finalSize;
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower;
        var fullBarRect = new Area2D(pos, new Size2D(RenderBounds.SizeX, RenderLayout.Border.SizeY));
        canvas.StyleBackground(fullBarRect, NKColor.Default);
        
        for (int x = 0; x < RenderBounds.SizeX; x++) {
            canvas.Place("─", new Point2D(pos.X + x, pos.Y + RenderLayout.Border.SizeY - 1));
        }

        foreach (var child in _children) {
            child.Render(canvas);
        }
    }

    public override ElementInfo Info { get; } = new();
}
