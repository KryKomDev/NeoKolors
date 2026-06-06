// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A stateful popup control that displays information for an element when hovered or focused.
/// </summary>
public class ToolTip : ContentControl {
    
    private bool _isOpen;
    private Point _offset = new(0, 1);

    public bool IsOpen {
        get => _isOpen;
        set {
            if (_isOpen == value) return;
            _isOpen = value;
            InvokeElementUpdated();
        }
    }

    public Point Offset {
        get => _offset;
        set {
            if (_offset.X == value.X && _offset.Y == value.Y) return;
            _offset = value;
            InvokeElementUpdated();
        }
    }

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Border = BorderStyle.GetNormal(),
        ReadOnly = true
    };

    public ToolTip() : base(DefaultStyles) { }

    public ToolTip(object content) : base(DefaultStyles) {
        Content = content;
    }

    protected override Size MeasureOverride(Size availableSize) {
        if (!IsOpen) return Size.Zero;

        Size contentSize;
        if (Content is IElement element) {
            element.Measure(availableSize);
            contentSize = element.DesiredSize;
        }
        else {
            var text = Content?.ToString() ?? string.Empty;
            contentSize = new Size(text.Length, 1);
        }

        return contentSize;
    }

    protected override Size ArrangeOverride(Size finalSize) {
        if (Content is IElement element) {
            var contentPos = RenderBounds.Lower + Offset + RenderLayout.Content.Lower;
            element.Arrange(new Rectangle(contentPos, RenderLayout.Content.Size));
        }
        return finalSize;
    }

    public override void Render(ICharCanvas canvas) {
        if (!IsOpen || !_style.Visible) return;

        Measure(new Size(canvas.Width, canvas.Height));
        
        var pos = RenderBounds.Lower + Offset;

        if (!_style.BackgroundColor.IsInherit) {
            canvas.StyleBackground(RenderLayout.Border + pos, _style.BackgroundColor);
            canvas.Fill(RenderLayout.Content + pos, ' ');
        }

        if (!_style.Border.IsBorderless) {
            canvas.PlaceRectangle(RenderLayout.Border + pos, _style.Border);
        }

        RenderCore(canvas);
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower + Offset;

        if (Content is IElement element) {
            element.Render(canvas);
        }
        else if (Content != null) {
            var text = Content.ToString() ?? string.Empty;
            canvas.Place(text, pos + RenderLayout.Content.Lower, RenderLayout.Content.Width, HorizontalAlign.LEFT);
        }
    }

    public override ElementInfo Info { get; } = new();
}
