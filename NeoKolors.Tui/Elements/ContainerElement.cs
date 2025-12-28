//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Styles.Properties;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a container elementOld in the NeoKolors TUI framework, designed to manage and lay out child elements
/// using flexible styling and alignment properties. This abstract class extends <see cref="UniversalElement"/>
/// and additionally supports features like flex-direction, overflow behavior, and grid alignment.
/// </summary>
public abstract class ContainerElement : UniversalElement {

    public virtual JustifyContent JustifyContent {
        get => _style.Get(new JustifyContentProperty(DefaultJustifyContent)).Value;
        set => _style.Set(new JustifyContentProperty(value));
    }

    protected virtual JustifyContent DefaultJustifyContent => JustifyContent.START;
    
    public virtual GridDimensions Grid {
        get => _style.Get(new GridProperty(DefaultGrid)).Value;
        set => _style.Set(new GridProperty(value));
    }
    
    protected virtual GridDimensions DefaultGrid => new(); 

    public virtual bool EnableGrid {
        get => _style.Get(new EnableGridProperty(DefaultEnableGrid)).Value;
        set => _style.Set(new EnableGridProperty(value));
    }

    protected virtual bool DefaultEnableGrid => false;

    public virtual Direction Direction {
        get => _style.Get(new DirectionProperty(DefaultDirection)).Value;
        set => _style.Set(new DirectionProperty(value));
    }

    protected virtual Direction DefaultDirection => Direction.TOP_TO_BOTTOM;
}