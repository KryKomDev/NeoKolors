// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Specifies the orientation of a StackPanel.
/// </summary>
public enum Orientation {
    VERTICAL,
    HORIZONTAL
}

/// <summary>
/// Arranges child elements into a single line that can be oriented horizontally or vertically.
/// </summary>
public class StackPanel : Panel {
    
    private Orientation _orientation = Orientation.VERTICAL;
    private int _spacing;

    public Orientation Orientation {
        get => _orientation;
        set {
            if (_orientation == value) return;
            _orientation = value;
            InvokeElementUpdated();
        }
    }

    public int Spacing {
        get => _spacing;
        set {
            if (_spacing == value) return;
            _spacing = value;
            InvokeElementUpdated();
        }
    }

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        ReadOnly = true
    };

    public StackPanel() : base(DefaultStyles) { }

    public StackPanel(Orientation orientation, int spacing = 0) : base(DefaultStyles) {
        _orientation = orientation;
        _spacing = spacing;
    }

    protected override Size MeasureOverride(Size availableSize) {
        int width = 0;
        int height = 0;

        for (int i = 0; i < _children.Count; i++) {
            var child = _children[i];
            child.Measure(availableSize);
            var childSize = child.DesiredSize;

            if (_orientation == Orientation.VERTICAL) {
                height += childSize.Height;
                if (i > 0) height += _spacing;
                width = Math.Max(width, childSize.Width);
            }
            else {
                width += childSize.Width;
                if (i > 0) width += _spacing;
                height = Math.Max(height, childSize.Height);
            }
        }

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize) {
        int offset = 0;
        var pos = RenderBounds.Lower;

        foreach (var child in _children) {
            var childSize = child.DesiredSize;
            Point childPos;

            if (_orientation == Orientation.VERTICAL) {
                childPos = pos + RenderLayout.Content.Lower + new Point(0, offset);
                offset += childSize.Height + _spacing;
            }
            else {
                childPos = pos + RenderLayout.Content.Lower + new Point(offset, 0);
                offset += childSize.Width + _spacing;
            }

            child.Arrange(new Rectangle(childPos, childSize));
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
