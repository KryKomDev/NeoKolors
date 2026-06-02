// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Console.Input;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A stateful collapsible selection control.
/// Shows [ Option Selected   ▼ ].
/// </summary>
public class ComboBox : ItemsControl, IMouseInteractableElement<IReadOnlyList<IElement>> {
    
    private bool _isDropDownOpen;
    private int _selectedIndex = -1;
    private object? _selectedItem;

    public bool IsDropDownOpen {
        get => _isDropDownOpen;
        set {
            if (_isDropDownOpen == value) return;
            _isDropDownOpen = value;
            InvokeElementUpdated();
        }
    }

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

    public event Action<MouseButton> OnClick    = delegate { };
    public event Action<MouseButton> OnRelease  = delegate { };
    public event Action              OnHover    = delegate { };
    public event Action              OnHoverOut = delegate { };

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Border = BorderStyle.Borderless,
        Width = Dimension.Chars(25),
        Height = Dimension.Chars(1),
        ReadOnly = true
    };

    public ComboBox() : base(DefaultStyles) {
        ItemsPanel = new StackPanel(Orientation.VERTICAL);
        OnClick += HandleClick;
    }

    private void HandleClick(MouseButton button) {
        if (!IsEnabled) return;
        IsDropDownOpen = !IsDropDownOpen;
    }

    public void Click(MouseButton button) => OnClick(button);
    public void Release(MouseButton button) => OnRelease(button);
    public void Hover() => OnHover();
    public void HoverOut() => OnHoverOut();

    protected override void OnItemsSourceChanged() {
        if (ItemsPanel == null) {
            ItemsPanel = new StackPanel(Orientation.VERTICAL, 0);
        }

        ItemsPanel.ClearChildren();
        if (ItemsSource != null) {
            foreach (var item in ItemsSource) {
                if (item is IElement element) {
                    ItemsPanel.AddChild(element);
                }
                else {
                    var tb = new TextBlock(item?.ToString() ?? string.Empty);
                    tb.Style.Padding = new Spacing(1, 0, 0, 0);
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
        return new Size(25, 1);
    }

    protected override Size ArrangeOverride(Size finalSize) {
        if (ItemsPanel != null) {
            var contentWidth = RenderLayout.Content.Width;
            var dropdownHeight = Math.Min(10, ItemsPanel.Children.Count);
            var dropdownPos = RenderBounds.Lower + RenderLayout.Content.Lower + new Point(0, 1);
            var dropdownInnerRect = new Rectangle(dropdownPos + new Point(1, 1), new Size(contentWidth - 2, dropdownHeight));

            int offset = 0;
            for (int i = 0; i < ItemsPanel.Children.Count && offset < dropdownHeight; i++) {
                var child = ItemsPanel.Children[i];
                var childSize = new Size(contentWidth - 2, 1);
                var childPos = dropdownInnerRect.Lower + new Point(0, offset);

                child.Measure(childSize);
                child.Arrange(new Rectangle(childPos, childSize));
                offset++;
            }
        }
        return finalSize;
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower;
        var contentPos = pos + RenderLayout.Content.Lower;
        var contentWidth = RenderLayout.Content.Width;

        string displayText = SelectedItem?.ToString() ?? "Select option...";
        
        int maxTextLength = Math.Max(0, contentWidth - 6);
        if (displayText.Length > maxTextLength && maxTextLength > 3) {
            displayText = displayText.Substring(0, maxTextLength - 3) + "...";
        }

        string trigger = $"[ {displayText.PadRight(maxTextLength)} ▼ ]";
        canvas.Place(trigger, contentPos, contentWidth, HorizontalAlign.LEFT);

        if (IsDropDownOpen && ItemsPanel != null && ItemsPanel.Children.Count > 0) {
            var dropdownHeight = Math.Min(10, ItemsPanel.Children.Count);
            var dropdownSize = new Size(contentWidth, dropdownHeight + 2);
            var dropdownPos = contentPos + new Point(0, 1);
            var dropdownRect = new Rectangle(dropdownPos, dropdownSize);

            canvas.StyleBackground(dropdownRect, NKColor.Default);
            canvas.PlaceRectangle(dropdownRect, BorderStyle.GetNormal());

            var dropdownInnerRect = new Rectangle(dropdownPos + new Point(1, 1), new Size(contentWidth - 2, dropdownHeight));
            
            int offset = 0;
            for (int i = 0; i < ItemsPanel.Children.Count && offset < dropdownHeight; i++) {
                var child = ItemsPanel.Children[i];
                var childSize = new Size(contentWidth - 2, 1);
                var childPos = dropdownInnerRect.Lower + new Point(0, offset);

                if (i == SelectedIndex) {
                    canvas.StyleBackground(new Rectangle(childPos, childSize), NKColor.Inherit);
                }

                child.Render(canvas);
                offset++;
            }
        }
    }

    public override ElementInfo Info { get; } = new();
}
