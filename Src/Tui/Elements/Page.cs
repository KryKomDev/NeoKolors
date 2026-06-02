// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;
using Portable.Xaml.Markup;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// The root application context screen container. Occupies the full viewport.
/// Replaces the legacy Body container.
/// </summary>
[ContentProperty(nameof(Children))]
public class Page : Panel {
    
    protected static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Width  = Dimension.Stretch,
        Height = Dimension.Stretch,
        ReadOnly = true
    };

    public Page(params IElement[] children) : base(DefaultStyles) {
        if (children != null) {
            foreach (var child in children) {
                AddChild(child);
            }
        }
    }

    public Page() : base(DefaultStyles) { }

    protected override Size MeasureOverride(Size availableSize) {
        foreach (var child in _children) {
            child?.Measure(availableSize);
        }
        return availableSize;
    }

    protected override Size ArrangeOverride(Size finalSize) {
        var contentBounds = RenderLayout.Content + RenderBounds.Lower;
        foreach (var child in _children) {
            child?.Arrange(contentBounds);
        }
        return finalSize;
    }

    protected override void RenderCore(ICharCanvas canvas) {
        foreach (var child in _children) {
            child?.Render(canvas);
        }
    }

    public override ElementInfo Info => ElementInfo.Default;
}
