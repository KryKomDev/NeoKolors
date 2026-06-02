// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// An interactive control that represents a mutually exclusive selection RadioButton.
/// Shows ( ) or (o).
/// </summary>
public class RadioButton : ToggleButton {
    
    private string _groupName = string.Empty;

    public string GroupName {
        get => _groupName;
        set {
            if (_groupName == value) return;
            UnregisterRadio(this);
            _groupName = value ?? string.Empty;
            RegisterRadio(this);
            InvokeElementUpdated();
        }
    }

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Border = BorderStyle.Borderless,
        ReadOnly = true
    };

    private static readonly Dictionary<string, List<WeakReference<RadioButton>>> Groups = new();

    public RadioButton() : base(DefaultStyles) {
        RegisterRadio(this);
    }

    public RadioButton(object content) : base(DefaultStyles) {
        Content = content;
        RegisterRadio(this);
    }

    public RadioButton(string label) : base(DefaultStyles) {
        Content = label;
        RegisterRadio(this);
    }

    protected override void Toggle() {
        // RadioButtons can only be checked by click, they cannot be unchecked by click (only checked is mutually exclusive)
        if (IsChecked != true) {
            IsChecked = true;
        }
    }

    protected override void OnCheckedChanged() {
        base.OnCheckedChanged();
        if (IsChecked == true && !string.IsNullOrEmpty(GroupName)) {
            // Uncheck other radios in the same group
            lock (Groups) {
                if (Groups.TryGetValue(GroupName, out var list)) {
                    foreach (var weakRef in list) {
                        if (weakRef.TryGetTarget(out var radio) && radio != this) {
                            radio.IsChecked = false;
                        }
                    }
                }
            }
        }
    }

    private static void RegisterRadio(RadioButton radio) {
        if (string.IsNullOrEmpty(radio.GroupName)) return;
        lock (Groups) {
            if (!Groups.TryGetValue(radio.GroupName, out var list)) {
                list = new List<WeakReference<RadioButton>>();
                Groups[radio.GroupName] = list;
            }
            // Remove dead references
            list.RemoveAll(r => !r.TryGetTarget(out _));
            list.Add(new WeakReference<RadioButton>(radio));
        }
    }

    private static void UnregisterRadio(RadioButton radio) {
        if (string.IsNullOrEmpty(radio.GroupName)) return;
        lock (Groups) {
            if (Groups.TryGetValue(radio.GroupName, out var list)) {
                list.RemoveAll(r => !r.TryGetTarget(out var target) || target == radio);
                if (list.Count == 0) {
                    Groups.Remove(radio.GroupName);
                }
            }
        }
    }

    protected override Size MeasureOverride(Size availableSize) {
        Size contentSize;
        if (Content is IElement element) {
            element.Measure(availableSize);
            contentSize = element.DesiredSize;
        }
        else {
            var text = Content?.ToString() ?? string.Empty;
            contentSize = new Size(text.Length, 1);
        }

        return new Size(contentSize.Width + 4, Math.Max(contentSize.Height, 1));
    }

    protected override Size ArrangeOverride(Size finalSize) {
        if (Content is IElement element) {
            var contentPos = RenderBounds.Lower + RenderLayout.Content.Lower;
            element.Arrange(new Rectangle(contentPos + new Point(4, 0), new Size(Math.Max(0, RenderLayout.Content.Width - 4), RenderLayout.Content.Height)));
        }
        return finalSize;
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower;

        string marker = IsChecked == true ? "(o)" : "( )";

        var contentPos = pos + RenderLayout.Content.Lower;
        canvas.Place(marker, contentPos, 4, HorizontalAlign.LEFT);

        if (Content is IElement element) {
            element.Render(canvas);
        }
        else if (Content != null) {
            var text = Content.ToString() ?? string.Empty;
            canvas.Place(text, contentPos + new Point(4, 0), Math.Max(0, RenderLayout.Content.Width - 4), HorizontalAlign.LEFT);
        }
    }

    public override ElementInfo Info { get; } = new();
}
