using System;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;
using NeoKolors.Console;
using NeoKolors.Console.Input;
using NeoKolors.Common;
using NeoKolors.Tui.Global;
using NeoKolors.Tui.Styles.Properties;
using NeoKolors.Tui.Events;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A slider control that allows the user to select from a range of values by moving a slider thumb.
/// Renders like: ═══╡█╞══════════ 30%
/// </summary>
public class Slider : RangeBase, ISelectableElement<double>, IMouseInteractableElement<double> {
    
    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Width = Dimension.Chars(20),
        Height = Dimension.Chars(1),
        Border = BorderStyle.Borderless,
        ReadOnly = true
    };

    private bool _isDragging;

    public string Unit { get; set; } = "%";

    public bool IsSelected { get; private set; }
    public bool IsSelectable => IsEnabled;

    public event Action<MouseButton> OnClick = delegate { };
    public event Action<MouseButton> OnRelease = delegate { };
    public event Action OnHover = delegate { };
    public event Action OnHoverOut = delegate { };

    public Slider() : base(DefaultStyles) {
        OnClick += HandleClick;
    }

    public void Click(MouseButton button) => OnClick(button);
    public void Release(MouseButton button) => OnRelease(button);
    public void Hover() => OnHover();
    public void HoverOut() => OnHoverOut();

    public void Select() {
        if (!IsSelectable) return;
        if (IsSelected) return;
        IsSelected = true;
        IsFocused = true;
        AppEventBus.KeyEvent += HandleKey;
        AppEventBus.MouseEvent += HandleMouse;
        InvokeElementUpdated();

        if (ElementManager.CurrentlySelected != this) {
            ElementManager.CurrentlySelected = this;
        }
    }

    public void Deselect() {
        if (!IsSelected) return;
        IsSelected = false;
        IsFocused = false;
        AppEventBus.KeyEvent -= HandleKey;
        AppEventBus.MouseEvent -= HandleMouse;
        _isDragging = false;
        InvokeElementUpdated();

        if (ElementManager.CurrentlySelected == this) {
            ElementManager.CurrentlySelected = null;
        }
    }

    protected override Size MeasureOverride(Size availableSize) {
        return new Size(20, 1);
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower;
        var contentPos = pos + RenderLayout.Content.Lower;
        var contentWidth = RenderLayout.Content.Width;

        string pctLabel = string.IsNullOrEmpty(Unit) ? $" {Value:0}" : $" {Value:0}{Unit}";
        int labelLength = pctLabel.Length;

        if (contentWidth <= labelLength + 3) {
            canvas.Place(pctLabel.Trim(), contentPos, contentWidth, HorizontalAlign.LEFT);
            return;
        }

        var barWidth = contentWidth - labelLength;
        double percentage = (Maximum - Minimum) > 0 ? (Value - Minimum) / (Maximum - Minimum) : 0;
        int thumbPos = (int)Math.Round(percentage * (barWidth - 3)); // thumb has size 3: "╡█╞"

        thumbPos = Math.Clamp(thumbPos, 0, barWidth - 3);

        string leftBar = new('═', thumbPos);
        string rightBar = new('═', barWidth - 3 - thumbPos);
        
        var text = leftBar + "╡█╞" + rightBar + pctLabel;
        var styledText = new AnsiString(text, new NKStyle(
            _style.TextColor,
            _style.BackgroundColor,
            _style.TextStyle
        ));

        if (IsSelected) {
            styledText = styledText.ApplyStyle(new NKStyle(
                _style.TextColor,
                _style.BackgroundColor,
                _style.TextStyle | TextStyles.NEGATIVE
            ), leftBar.Length, 3);
        }

        canvas.Place(styledText, contentPos, contentWidth, HorizontalAlign.LEFT);
    }

    private void HandleClick(MouseButton button) {
        if (button == MouseButton.LEFT) {
            Select();
        }
    }

    private void HandleMouse(MouseEventArgs m) {
        if (!Style.Visible || !IsEnabled) return;

        if (m.IsPress && m.Button == MouseButton.LEFT) {
            if (RenderBounds.Contains(m.Position.X, m.Position.Y)) {
                _isDragging = true;
                Select();
                UpdateValueFromX(m.Position.X);
            }
        }
        else if (_isDragging) {
            if (m.Released || m.IsRelease || m.Button == MouseButton.RELEASE) {
                _isDragging = false;
            }
            else if (m.Moved) {
                UpdateValueFromX(m.Position.X);
            }
        }
    }

    private void HandleKey(KeyEventArgs keyInfo) {
        if (!IsEnabled || !keyInfo.Down) return;

        switch (keyInfo.Key) {
            case KeyCode.ARROW_LEFT:
            case KeyCode.ARROW_DOWN:
                Value -= SmallChange;
                break;
            case KeyCode.ARROW_RIGHT:
            case KeyCode.ARROW_UP:
                Value += SmallChange;
                break;
            case KeyCode.PAGE_DOWN:
                Value -= LargeChange;
                break;
            case KeyCode.PAGE_UP:
                Value += LargeChange;
                break;
            case KeyCode.HOME:
                Value = Minimum;
                break;
            case KeyCode.END:
                Value = Maximum;
                break;
        }
    }

    private void UpdateValueFromX(int absoluteX) {
        var pos = RenderBounds.Lower;
        var contentPos = pos + RenderLayout.Content.Lower;
        var contentWidth = RenderLayout.Content.Width;

        string pctLabel = string.IsNullOrEmpty(Unit) ? $" {Value:0}" : $" {Value:0}{Unit}";
        int labelLength = pctLabel.Length;
        var barWidth = contentWidth - labelLength;
        var maxThumbPos = barWidth - 3;
        if (maxThumbPos <= 0) return;

        int startX = contentPos.X;
        int relX = absoluteX - startX;
        int thumbPos = relX - 1; // align center of "╡█╞" to click
        thumbPos = Math.Clamp(thumbPos, 0, maxThumbPos);

        double percentage = (double)thumbPos / maxThumbPos;
        Value = Minimum + percentage * (Maximum - Minimum);
    }

    public override ElementInfo Info { get; } = new();
}
