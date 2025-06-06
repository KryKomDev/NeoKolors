//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a universal element in the NeoKolors TUI framework. It provides common styling and layout properties
/// shared by all elements and defines the fundamental structure and behavior for custom elements.
/// Every property is overridable, so any element can define its defaults.
/// </summary>
public abstract class UniversalElement {

    public virtual DisplayType Display {
        get => Style.Get(new DisplayProperty()).Value;
        set => Style.Set(new DisplayProperty(value));
    }

    public virtual SizeValue MinWidth {
        get => Style.Get(new MinWidth(0)).Value;
        set => Style.Set(new MinWidth(value));
    }

    public virtual SizeValue MaxWidth {
        get => Style.Get(new MaxWidth(SizeValue.Chars(int.MaxValue))).Value;
        set => Style.Set(new MaxWidth(value));
    }

    public virtual SizeValue MinHeight {
        get => Style.Get(new MinHeight(0)).Value;
        set => Style.Set(new MinHeight(value));
    }

    public virtual SizeValue MaxHeight {
        get => Style.Get(new MaxHeight(int.MaxValue)).Value;
        set => Style.Set(new MaxHeight(value));
    }

    public virtual BorderStyle Border {
        get => Style.Get(new BorderProperty()).Value;
        set => Style.Set(new BorderProperty(value));
    }

    public virtual PaddingProperty Padding {
        get => Style.Get(new PaddingProperty());
        set => Style.Set(value);
    }

    public virtual MarginProperty Margin {
        get => Style.Get(new MarginProperty());
        set => Style.Set(value);
    }

    public virtual NKColor BackgroundColor {
        get => Style.Get(new BackgroundColorProperty()).Value;
        set => Style.Set(new BackgroundColorProperty(value));
    }

    public virtual GridAlignProperty GridAlign {
        get => Style.Get(new GridAlignProperty());
        set => Style.Set(value);
    }

    public virtual AlignSelfProperty AlignSelf {
        get => Style.Get(new AlignSelfProperty());
        set => Style.Set(value);
    }

    public abstract StyleCollection Style { get; }
}