// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A control that supports navigation between Pages, keeping a history stack.
/// </summary>
public class Frame : ContentControl {
    
    private readonly Stack<IElement> _navigationHistory = new();

    public bool CanGoBack => _navigationHistory.Count > 1;

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Border = BorderStyle.Borderless,
        ReadOnly = true
    };

    public Frame() : base(DefaultStyles) { }

    public void Navigate(IElement page) {
        if (page == null) throw new ArgumentNullException(nameof(page));
        
        _navigationHistory.Push(page);
        Content = page;
    }

    public bool GoBack() {
        if (!CanGoBack) return false;

        _navigationHistory.Pop(); // pop current
        var previousPage = _navigationHistory.Peek();
        Content = previousPage;
        return true;
    }

    protected override Size MeasureOverride(Size availableSize) {
        var contentSize = Size.Zero;
        if (Content is IElement element) {
            element.Measure(availableSize);
            contentSize = element.DesiredSize;
        }
        return contentSize;
    }

    protected override void RenderCore(ICharCanvas canvas) {
        if (Content is IElement element) {
            element.Render(canvas);
        }
    }

    public override ElementInfo Info { get; } = new();
}
