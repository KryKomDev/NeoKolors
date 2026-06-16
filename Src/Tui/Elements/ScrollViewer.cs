// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Specifies the visibility of scrollbars within a ScrollViewer.
/// </summary>
public enum ScrollBarVisibility {
    Disabled,
    Auto,
    Hidden,
    Visible
}

/// <summary>
/// A control that supports scrolling of its content when it overflows the viewport.
/// Renders scroll tracks using block chars (░ and █).
/// </summary>
public class ScrollViewer : ContentControl {
    
    private ScrollBarVisibility _horizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    private ScrollBarVisibility _verticalScrollBarVisibility = ScrollBarVisibility.Auto;
    private int _scrollX;
    private int _scrollY;

    public ScrollBarVisibility HorizontalScrollBarVisibility {
        get => _horizontalScrollBarVisibility;
        set {
            if (_horizontalScrollBarVisibility == value) return;
            _horizontalScrollBarVisibility = value;
            InvokeElementUpdated();
        }
    }

    public ScrollBarVisibility VerticalScrollBarVisibility {
        get => _verticalScrollBarVisibility;
        set {
            if (_verticalScrollBarVisibility == value) return;
            _verticalScrollBarVisibility = value;
            InvokeElementUpdated();
        }
    }

    public int ScrollX {
        get => _scrollX;
        set {
            if (_scrollX == value) return;
            _scrollX = value;
            InvokeElementUpdated();
        }
    }

    public int ScrollY {
        get => _scrollY;
        set {
            if (_scrollY == value) return;
            _scrollY = value;
            InvokeElementUpdated();
        }
    }

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Border = BorderStyle.Borderless,
        ReadOnly = true
    };

    public ScrollViewer() : base(DefaultStyles) { }

    public ScrollViewer(object content) : base(DefaultStyles) {
        Content = content;
    }

    protected override Size2D MeasureOverride(Size2D availableSize) {
        var child = GetChildNode();
        if (child != null) {
            child.Measure(availableSize);
            return child.DesiredSize;
        }
        return Size2D.One;
    }

    protected override Size2D ArrangeOverride(Size2D finalSize) {
        var child = GetChildNode();
        if (child == null) return finalSize;

        var childSize = child.DesiredSize;
        var contentWidth = RenderLayout.Content.SizeX;
        var contentHeight = RenderLayout.Content.SizeY;

        bool showVertical = VerticalScrollBarVisibility == ScrollBarVisibility.Visible ||
                            (VerticalScrollBarVisibility == ScrollBarVisibility.Auto && childSize.Y > contentHeight);

        bool showHorizontal = HorizontalScrollBarVisibility == ScrollBarVisibility.Visible ||
                              (HorizontalScrollBarVisibility == ScrollBarVisibility.Auto && childSize.X > contentWidth);

        int viewportWidth = contentWidth - (showVertical ? 1 : 0);
        int viewportHeight = contentHeight - (showHorizontal ? 1 : 0);

        _scrollX = Math.Clamp(_scrollX, 0, Math.Max(0, childSize.X - viewportWidth));
        _scrollY = Math.Clamp(_scrollY, 0, Math.Max(0, childSize.Y - viewportHeight));

        var contentPos = RenderBounds.Lower + RenderLayout.Content.Lower;
        var renderRect = new Area2D(contentPos - new Point2D(_scrollX, _scrollY), childSize);
        child.Arrange(renderRect);

        return finalSize;
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var child = GetChildNode();
        if (child == null) return;

        var childSize = child.DesiredSize;
        var contentPos = RenderBounds.Lower + RenderLayout.Content.Lower;
        var contentWidth = RenderLayout.Content.SizeX;
        var contentHeight = RenderLayout.Content.SizeY;

        bool showVertical = VerticalScrollBarVisibility == ScrollBarVisibility.Visible ||
                            (VerticalScrollBarVisibility == ScrollBarVisibility.Auto && childSize.Y > contentHeight);

        bool showHorizontal = HorizontalScrollBarVisibility == ScrollBarVisibility.Visible ||
                              (HorizontalScrollBarVisibility == ScrollBarVisibility.Auto && childSize.X > contentWidth);

        int viewportWidth = contentWidth - (showVertical ? 1 : 0);
        int viewportHeight = contentHeight - (showHorizontal ? 1 : 0);

        child.Render(canvas);

        if (showVertical && contentHeight > 0) {
            var scrollBarX = contentPos.X + contentWidth - 1;
            var trackHeight = viewportHeight;

            for (int y = 0; y < trackHeight; y++) {
                canvas.Place("░", new Point2D(scrollBarX, contentPos.Y + y));
            }

            if (childSize.Y > 0) {
                double visibleRatio = (double)viewportHeight / childSize.Y;
                int thumbHeight = Math.Max(1, (int)Math.Round(visibleRatio * trackHeight));
                double scrollRatio = (double)_scrollY / Math.Max(1, childSize.Y - viewportHeight);
                int thumbY = (int)Math.Round(scrollRatio * (trackHeight - thumbHeight));
                thumbY = Math.Clamp(thumbY, 0, trackHeight - thumbHeight);

                for (int y = 0; y < thumbHeight; y++) {
                    canvas.Place("█", new Point2D(scrollBarX, contentPos.Y + thumbY + y));
                }
            }
        }

        if (showHorizontal && contentWidth > 0) {
            var scrollBarY = contentPos.Y + contentHeight - 1;
            var trackWidth = viewportWidth;

            for (int x = 0; x < trackWidth; x++) {
                canvas.Place("░", new Point2D(contentPos.X + x, scrollBarY));
            }

            if (childSize.X > 0) {
                double visibleRatio = (double)viewportWidth / childSize.X;
                int thumbWidth = Math.Max(1, (int)Math.Round(visibleRatio * trackWidth));
                double scrollRatio = (double)_scrollX / Math.Max(1, childSize.X - viewportWidth);
                int thumbX = (int)Math.Round(scrollRatio * (trackWidth - thumbWidth));
                thumbX = Math.Clamp(thumbX, 0, trackWidth - thumbWidth);

                for (int x = 0; x < thumbWidth; x++) {
                    canvas.Place("█", new Point2D(contentPos.X + thumbX + x, scrollBarY));
                }
            }
        }
    }

    public override ElementInfo Info { get; } = new();
}
