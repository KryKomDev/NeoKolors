//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using NeoKolors.Tui.Rendering;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a universal element in the NeoKolors TUI framework. It provides common styling and layout properties
/// shared by all elements and defines the fundamental structure and behavior for custom elements.
/// Every property is overridable, so any element can define its defaults.
/// </summary>
public abstract class AbstractElement<T> : IElement<T> {
    
    protected StyleCollection _style;

    public StyleCollection Style {
        get  => _style;
        init => _style.Apply(value);
    }

    protected AbstractElement(StyleCollection defaultStyle) {
        _style = new StyleCollection(defaultStyle);
    }

    protected AbstractElement() {
        _style = new StyleCollection(AbstractElement.DefaultStyle);
    }

    // =========================== IELEMENT IMPLEMENTATION =========================== // 
    
    public abstract void Render(ICharCanvas canvas, Rectangle rect);
    public abstract Size GetMinSize(Size parent);
    public abstract Size GetMaxSize(Size parent);
    public abstract Size GetRenderSize(Size parent);
    public abstract ElementInfo Info { get; }
    public abstract event Action? OnElementUpdated;
    public abstract T GetChildNode();
    public abstract void SetChildNode(T childNode);
}

public static class AbstractElement {
    public static StyleCollection DefaultStyle { get; } = new() {
        Visible         = true,
        Padding         = Spacing.Zero,
        Margin          = Spacing.Zero,
        Position        = new Position(),
        Overflow        = false,
        GridAlign       = new Rectangle(0, 0, 0, 0),
        ZIndex          = 0,
        BackgroundColor = NKColor.Inherit,
        Border          = BorderStyle.Borderless,
        Width           = Dimension.Auto,
        Height          = Dimension.Auto,
        MinWidth        = Dimension.Auto,
        MinHeight       = Dimension.Auto,
        MaxWidth        = Dimension.Auto,
        MaxHeight       = Dimension.Auto,
        
        ReadOnly = true
    };
}