// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Console.Input;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A drawer control that can expand or collapse to show or hide its content.
/// Displays a header line with an expand/collapse indicator (▼/►).
/// </summary>
public class Expander : HeaderedContentControl, IMouseInteractableElement<IElement> {
    
    private bool _isExpanded;

    public bool IsExpanded {
        get => _isExpanded;
        set {
            if (_isExpanded == value) return;
            _isExpanded = value;
            InvokeElementUpdated();
        }
    }

    public event Action<MouseButton> OnClick = delegate { };
    public event Action<MouseButton> OnRelease = delegate { };
    public event Action OnHover = delegate { };
    public event Action OnHoverOut = delegate { };

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Border = BorderStyle.Borderless,
        ReadOnly = true
    };

    public Expander() : base(DefaultStyles) {
        OnClick += HandleClick;
    }

    public Expander(object header, IElement content) : base(DefaultStyles) {
        Header = header;
        Content = content;
        OnClick += HandleClick;
    }

    private void HandleClick(MouseButton button) {
        if (!IsEnabled) return;
        IsExpanded = !IsExpanded;
    }

    public void Click(MouseButton button) => OnClick(button);
    public void Release(MouseButton button) => OnRelease(button);
    public void Hover() => OnHover();
    public void HoverOut() => OnHoverOut();

    protected override Size MeasureOverride(Size availableSize) {
        var headerText = Header?.ToString() ?? string.Empty;
        int width = headerText.Length + 4;
        int height = 1;

        if (IsExpanded && Content is IElement element) {
            element.Measure(availableSize);
            var childSize = element.DesiredSize;
            width = Math.Max(width, childSize.Width + 2);
            height += childSize.Height;
        }

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize) {
        if (IsExpanded && Content is IElement element) {
            var contentPos = RenderBounds.Lower + RenderLayout.Content.Lower;
            element.Arrange(new Rectangle(contentPos + new Point(2, 1), new Size(Math.Max(0, RenderLayout.Content.Width - 2), Math.Max(0, RenderLayout.Content.Height - 1))));
        }
        return finalSize;
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower;
        var contentPos = pos + RenderLayout.Content.Lower;
        var contentWidth = RenderLayout.Content.Width;

        string symbol = IsExpanded ? "▼ " : "► ";
        string headerText = Header?.ToString() ?? string.Empty;
        canvas.Place(symbol + headerText, contentPos, contentWidth, HorizontalAlign.LEFT);

        if (IsExpanded && Content is IElement element) {
            element.Render(canvas);
        }
    }

    public override ElementInfo Info { get; } = new();
}
