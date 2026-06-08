// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;
using NeoKolors.Common;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A stateful content control representing an interactive click trigger.
/// Replaces the legacy text-only Button.
/// </summary>
public class Button : ButtonBase {
    
    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Border = BorderStyle.Borderless,
        ReadOnly = true
    };

    public Button(object content) : base(DefaultStyles) {
        Content = content;
    }

    public Button(string label) : base(DefaultStyles) {
        Content = label;
    }

    public Button() : base(DefaultStyles) { }

    protected override Size MeasureOverride(Size availableSize) {
        if (Content is IElement element) {
            element.Measure(availableSize);
            return element.DesiredSize;
        }

        var text = Content?.ToString() ?? string.Empty;
        return new Size(text.Length, 1);
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower;

        if (Content is IElement element) {
            element.Render(canvas);
        }
        else if (Content != null) {
            var text = Content.ToString() ?? string.Empty;
            var styledText = new AnsiString(text, new NKStyle(
                _style.TextColor,
                _style.BackgroundColor,
                _style.TextStyle
            ));
            canvas.Place(
                styledText,
                pos + RenderLayout.Content.Lower,
                RenderLayout.Content.Width,
                HorizontalAlign.CENTER
            );
        }
    }

    public override ElementInfo Info { get; } = new();
}