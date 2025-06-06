//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a container element in the NeoKolors TUI framework, designed to manage and lay out child elements
/// using flexible styling and alignment properties. This abstract class extends <see cref="UniversalElement"/>
/// and additionally supports features like flex-direction, overflow behavior, and grid alignment.
/// </summary>
public abstract class ContainerElement : UniversalElement {
    
    public virtual FlexDirection FlexDirection {
        get => Style.Get(new FlexDirectionProperty()).Value;
        set => Style.Set(new FlexDirectionProperty(value));
    }

    public virtual OverflowType Overflow {
        get => Style.Get(new OverflowProperty()).Value;
        set => Style.Set(new OverflowProperty(value));
    }

    public virtual JustifyContent JustifyContent {
        get => Style.Get(new JustifyContentProperty()).Value;
        set => Style.Set(new JustifyContentProperty(value));
    }

    public virtual GridProperty Grid {
        get => Style.Get(new GridProperty());
        set => Style.Set(value);
    }

    public virtual AlignItems AlignItems {
        get => Style.Get(new AlignItemsProperty()).Value;
        set => Style.Set(new AlignItemsProperty(value));
    }
}