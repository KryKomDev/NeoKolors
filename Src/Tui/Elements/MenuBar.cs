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

    protected override Size MeasureOverride(Size availableSize) {
        int width = 2;
        int height = 1;

        foreach (var child in _children) {
            if (child == null) continue;
            child.Measure(availableSize);
            var childSize = child.DesiredSize;
            width += childSize.Width + 2;
            height = Math.Max(height, childSize.Height);
        }

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize) {
        var pos = RenderBounds.Lower;
        int offset = 1;

        foreach (var child in _children) {
            if (child == null) continue;

            var childSize = child.DesiredSize;
            var childPos = pos + RenderLayout.Content.Lower + new Point(offset, 0);

            child.Arrange(new Rectangle(childPos, childSize));
            offset += childSize.Width + 2;
        }

        return finalSize;
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower;
        var fullBarRect = new Rectangle(pos, new Size(RenderBounds.Width, RenderLayout.Border.Height));
        canvas.StyleBackground(fullBarRect, NKColor.Default);
        
        for (int x = 0; x < RenderBounds.Width; x++) {
            canvas.Place("─", new Point(pos.X + x, pos.Y + RenderLayout.Border.Height - 1));
        }

        foreach (var child in _children) {
            child.Render(canvas);
        }
    }

    public override ElementInfo Info { get; } = new();
}
