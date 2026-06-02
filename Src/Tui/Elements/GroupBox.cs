// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a control that consists of a border outline and a styled header text overlay,
/// wrapping a single visual panel child. Replaces the legacy NamedDiv container.
/// </summary>
public class GroupBox : HeaderedContentControl {
    
    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Border = BorderStyle.Borderless,
        ReadOnly = true
    };

    public GroupBox(string header, IElement content) : base(DefaultStyles) {
        Header = header;
        Content = content;
    }

    public GroupBox() : base(DefaultStyles) { }

    protected override Size MeasureOverride(Size availableSize) {
        var contentSize = Size.Zero;
        if (Content is IElement element) {
            element.Measure(availableSize);
            contentSize = element.DesiredSize;
        }
        
        var headerText = Header?.ToString() ?? string.Empty;
        contentSize = contentSize with { Width = Math.Max(contentSize.Width, headerText.Length + 6) };

        return contentSize;
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower;

        if (Content is IElement element) {
            element.Render(canvas);
        }

        var headerText = Header?.ToString() ?? string.Empty;
        if (!string.IsNullOrEmpty(headerText)) {
            var borderVal = RenderLayout.Border;
            int padding = 2;
            canvas.Place(
                headerText,
                pos + borderVal.Lower + new Point(padding, 0),
                borderVal.Width - padding * 2,
                HorizontalAlign.LEFT
            );
        }
    }

    public override ElementInfo Info => ElementInfo.Default;
}
