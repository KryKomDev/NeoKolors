// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a control that displays a list of items and allows a single item to be selected.
/// Displays a sidebar cursor indicator (>).
/// </summary>
public class ListView : ItemsControl {
    
    private int _selectedIndex = -1;
    private object? _selectedItem;

    public int SelectedIndex {
        get => _selectedIndex;
        set {
            int count = ItemsPanel?.Children.Count ?? 0;
            int clamped = count > 0 ? Math.Clamp(value, 0, count - 1) : -1;
            if (_selectedIndex == clamped) return;
            _selectedIndex = clamped;
            UpdateSelectedItem();
            InvokeElementUpdated();
        }
    }

    public object? SelectedItem {
        get => _selectedItem;
        set {
            if (_selectedItem == value) return;
            _selectedItem = value;
            UpdateSelectedIndex();
            InvokeElementUpdated();
        }
    }

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Border = BorderStyle.GetNormal(),
        ReadOnly = true
    };

    public ListView() : base(DefaultStyles) {
        ItemsPanel = new StackPanel(Orientation.VERTICAL);
    }

    protected override void OnItemsSourceChanged() {
        ItemsPanel ??= new StackPanel(Orientation.VERTICAL);
        
        ItemsPanel.ClearChildren();
        
        if (ItemsSource != null) {
            foreach (var item in ItemsSource) {
                if (item is IElement element) {
                    ItemsPanel.AddChild(element);
                }
                else {
                    var tb = new TextBlock(item?.ToString() ?? string.Empty);
                    // Add some left padding to make room for selection indicator '>'
                    tb.Style.Padding = new Spacing(2, 0, 0, 0);
                    ItemsPanel.AddChild(tb);
                }
            }
        }

        SelectedIndex = (ItemsPanel.Children.Count > 0) ? 0 : -1;
    }

    private void UpdateSelectedItem() {
        if (ItemsPanel == null || _selectedIndex < 0 || _selectedIndex >= ItemsPanel.Children.Count) {
            _selectedItem = null;
            return;
        }

        var child = ItemsPanel.Children[_selectedIndex];
        if (child is TextBlock tb) {
            _selectedItem = tb.Content.Plain;
        }
        else {
            _selectedItem = child;
        }
    }

    private void UpdateSelectedIndex() {
        if (ItemsPanel == null || _selectedItem == null) {
            _selectedIndex = -1;
            return;
        }

        for (int i = 0; i < ItemsPanel.Children.Count; i++) {
            var child = ItemsPanel.Children[i];
            if (child == _selectedItem || (child is TextBlock tb && tb.Content.Plain == _selectedItem.ToString())) {
                _selectedIndex = i;
                return;
            }
        }
        _selectedIndex = -1;
    }

    protected override Size MeasureOverride(Size availableSize) {
        if (ItemsPanel != null) {
            ItemsPanel.Measure(availableSize);
            var size = ItemsPanel.DesiredSize;
            return size with { Width = size.Width + 2 };
        }
        return Size.One;
    }

    protected override Size ArrangeOverride(Size finalSize) {
        if (ItemsPanel != null) {
            ItemsPanel.Arrange(RenderLayout.Content + RenderBounds.Lower);
        }
        return finalSize;
    }

    protected override void RenderCore(ICharCanvas canvas) {
        if (ItemsPanel != null) {
            ItemsPanel.Render(canvas);

            var pos = RenderBounds.Lower;
            if (SelectedIndex >= 0 && SelectedIndex < ItemsPanel.Children.Count) {
                int relativeY = 0;
                for (int i = 0; i < SelectedIndex; i++) {
                    relativeY += ItemsPanel.Children[i].DesiredSize.Height;
                }

                var indicatorPos = pos + RenderLayout.Content.Lower + new Point(0, relativeY);
                if (RenderLayout.Content.Contains(0, relativeY)) {
                    canvas.Place("> ", indicatorPos, 2, HorizontalAlign.LEFT);
                }
            }
        }
    }

    public override ElementInfo Info { get; } = new();
}
