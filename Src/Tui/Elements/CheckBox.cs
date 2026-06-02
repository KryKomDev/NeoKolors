// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// An interactive control that represents a standard CheckBox.
/// Shows [ ], [x], or [■] (for indeterminate state).
/// </summary>
public class CheckBox : ToggleButton {
    
    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Border = BorderStyle.Borderless,
        ReadOnly = true
    };

    public CheckBox() : base(DefaultStyles) { }

    public CheckBox(object content) : base(DefaultStyles) {
        Content = content;
    }

    public CheckBox(string label) : base(DefaultStyles) {
        Content = label;
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

        string marker = IsChecked switch {
            true => "[x]",
            false => "[ ]",
            _ => "[■]"
        };

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
