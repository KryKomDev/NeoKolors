// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A control that presents a collection of items wrapping horizontally on a grid.
/// </summary>
public class GridView : ItemsControl {
    
    private int _itemWidth = 10;
    private int _itemHeight = 3;

    public int ItemWidth {
        get => _itemWidth;
        set {
            if (_itemWidth == value) return;
            _itemWidth = Math.Max(1, value);
            InvokeElementUpdated();
        }
    }

    public int ItemHeight {
        get => _itemHeight;
        set {
            if (_itemHeight == value) return;
            _itemHeight = Math.Max(1, value);
            InvokeElementUpdated();
        }
    }

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Border = BorderStyle.Borderless,
        ReadOnly = true
    };

    public GridView() : base(DefaultStyles) { }

    protected override void OnItemsSourceChanged() {
        if (ItemsPanel == null) {
            // GridView wraps items so we don't have to use standard StackPanel
            ItemsPanel = new StackPanel(Orientation.HORIZONTAL);
        }

        ItemsPanel.ClearChildren();
        if (ItemsSource != null) {
            foreach (var item in ItemsSource) {
                if (item is IElement element) {
                    ItemsPanel.AddChild(element);
                }
                else {
                    var box = new GroupBox(string.Empty, new TextBlock(item?.ToString() ?? string.Empty));
                    box.Style.Width = Dimension.Chars(ItemWidth);
                    box.Style.Height = Dimension.Chars(ItemHeight);
                    box.Style.Border = BorderStyle.GetNormal();
                    ItemsPanel.AddChild(box);
                }
            }
        }
    }

    protected override Size2D MeasureOverride(Size2D availableSize) {
        int contentWidth = availableSize.X;
        int currentX = 0;
        int currentY = 0;
        int maxRowHeight = ItemHeight;

        if (ItemsPanel != null) {
            foreach (var child in ItemsPanel.Children) {
                if (child == null) continue;
                
                int childW = child.Style.Width.IsAuto ? ItemWidth : child.Style.Width.ToScalar(contentWidth);
                int childH = child.Style.Height.IsAuto ? ItemHeight : child.Style.Height.ToScalar(availableSize.Y);

                child.Measure(new Size2D(childW, childH));

                if (currentX + childW > contentWidth && currentX > 0) {
                    currentX = 0;
                    currentY += maxRowHeight;
                    maxRowHeight = childH;
                }
                else {
                    maxRowHeight = Math.Max(maxRowHeight, childH);
                }
                currentX += childW;
            }
        }

        int finalHeight = currentY + ((ItemsPanel?.Children.Count ?? 0) > 0 ? maxRowHeight : 0);
        return new Size2D(contentWidth, finalHeight);
    }

    protected override Size2D ArrangeOverride(Size2D finalSize) {
        if (ItemsPanel == null) return finalSize;

        var contentPos = RenderBounds.Lower + RenderLayout.Content.Lower;
        var contentWidth = RenderLayout.Content.SizeX;

        int currentX = 0;
        int currentY = 0;

        foreach (var child in ItemsPanel.Children) {
            if (child == null) continue;

            int childW = child.Style.Width.IsAuto ? ItemWidth : child.Style.Width.ToScalar(contentWidth);
            int childH = child.Style.Height.IsAuto ? ItemHeight : child.Style.Height.ToScalar(RenderLayout.Content.SizeY);

            if (currentX + childW > contentWidth && currentX > 0) {
                currentX = 0;
                currentY += childH;
            }

            var childBounds = new Area2D(contentPos + new Point2D(currentX, currentY), new Size2D(childW, childH));
            child.Arrange(childBounds);

            currentX += childW;
        }

        return finalSize;
    }

    protected override void RenderCore(ICharCanvas canvas) {
        if (ItemsPanel != null) {
            foreach (var child in ItemsPanel.Children) {
                child.Render(canvas);
            }
        }
    }

    public override ElementInfo Info { get; } = new();
}
