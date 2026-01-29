//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Rendering;
using NeoKolors.Tui.Styles.Properties;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a universal element in the NeoKolors TUI framework. It provides common styling and layout properties
/// shared by all elements and defines the fundamental structure and behavior for custom elements.
/// Every property is overridable, so any element can define its defaults.
/// </summary>
public abstract class UniversalElement<T> : IElement<T> {
    
    public virtual bool Visible {
        get => _style.Get(DefaultVisible).Value;
        set => _style.Set(new VisibleProperty(value));
    }

    protected virtual VisibleProperty DefaultVisible => new(true);

    public virtual Dimension MinWidth {
        get => _style.Get(DefaultMinWidth).Value;
        set => _style.Set(new MinWidthProperty(value));
    }

    protected virtual MinWidthProperty DefaultMinWidth => new(Dimension.Auto);

    public virtual Dimension MaxWidth {
        get => _style.Get(DefaultMaxWidth).Value;
        set => _style.Set(new MaxWidthProperty(value));
    }

    protected virtual MaxWidthProperty DefaultMaxWidth => new(Dimension.Auto);

    public virtual Dimension Width {
        get => _style.Get(DefaultWidth).Value;
        set => _style.Set(new WidthProperty(value));
    }

    protected virtual WidthProperty DefaultWidth => new(Dimension.Auto);

    public virtual Dimension MinHeight {
        get => _style.Get(DefaultMinHeight).Value;
        set => _style.Set(new MinHeightProperty(value));
    }

    protected virtual MaxHeightProperty DefaultMinHeight => new(Dimension.Auto);

    public virtual Dimension MaxHeight {
        get => _style.Get(DefaultMaxHeight).Value;
        set => _style.Set(new MaxHeightProperty(value));
    }

    protected virtual MaxHeightProperty DefaultMaxHeight => new(Dimension.Auto);
    
    public virtual Dimension Height {
        get => _style.Get(DefaultHeight).Value;
        set => _style.Set(new HeightProperty(value));
    }

    protected virtual HeightProperty DefaultHeight => new(Dimension.Auto);

    public virtual BorderStyle Border {
        get => _style.Get(new BorderProperty(DefaultBorder)).Value;
        set => _style.Set(new BorderProperty(value));
    }

    protected virtual BorderStyle DefaultBorder => BorderStyle.Borderless;

    public virtual Spacing Padding {
        get => _style.Get(DefaultPadding).Value;
        set => _style.Set(new PaddingProperty(value));
    }

    protected virtual PaddingProperty DefaultPadding => new();

    public virtual Spacing Margin {
        get => _style.Get(DefaultMargin).Value;
        set => _style.Set(new MarginProperty(value));
    }

    protected virtual MarginProperty DefaultMargin => new();

    public virtual NKColor BackgroundColor {
        get => _style.Get(DefaultBackgroundColor).Value;
        set => _style.Set(new BackgroundColorProperty(value));
    }

    protected virtual BackgroundColorProperty DefaultBackgroundColor => new(NKColor.Inherit);

    public Rectangle GridAlign {
        get => _style.Get(DefaultGridAlign).Value;
        set => _style.Set(new GridAlignProperty(value));
    }

    protected virtual GridAlignProperty DefaultGridAlign => new(new Rectangle(0, 0, 0, 0));
    
    public virtual bool Overflow {
        get => _style.Get(new OverflowProperty(DefaultOverflow)).Value;
        set => _style.Set(new OverflowProperty(value));
    }
    
    protected virtual bool DefaultOverflow => false;

    public virtual Position Position {
        get => _style.Get(DefaultPosition).Value;
        set => _style.Set(new PositionProperty(value));
    }
    
    protected  virtual PositionProperty DefaultPosition => new();

    public virtual int ZIndex {
        get => _style.Get(new ZIndexProperty(0)).Value;
        set => _style.Set(new ZIndexProperty(value));
    }
    
    // =========================== STYLE CHANGE NOTIFICATIONS =========================== // 

    protected Styles.StyleCollection _style = new();
    protected event Action? OnStyleAccess = () => {};
    
    public Styles.StyleCollection Style {
        get {
            OnStyleAccess?.Invoke();
            return _style;
        }
    }

    public abstract ElementInfo Info { get; }


    // =========================== IELEMENT IMPLEMENTATION =========================== // 
    
    public abstract void Render(ICharCanvas canvas, Rectangle rect);
    public abstract Size GetMinSize(Size parent);
    public abstract Size GetMaxSize(Size parent);
    public abstract Size GetRenderSize(Size parent);
    public abstract event Action? OnElementUpdated;
    public abstract T GetChildNode();
    public abstract void SetChildNode(T childNode);
}