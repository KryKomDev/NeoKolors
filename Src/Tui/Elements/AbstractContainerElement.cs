//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a container elementOld in the NeoKolors TUI framework, designed to manage and lay out child elements
/// using flexible styling and alignment properties. This abstract class extends <see cref="AbstractElement{T}"/>
/// and additionally supports features like flex-direction, overflow behavior, and grid alignment.
/// </summary>
public abstract class AbstractContainerElement : AbstractElement<IElement[]> {
    
    public static StyleCollection DefaultStyle { get; } = new(AbstractElement.DefaultStyle) {
        JustifyContent = JustifyContent.START,
        Direction = Direction.TOP_TO_BOTTOM,
        Grid = new GridDimensions(),
        EnableGrid = false,
        
        ReadOnly = true
    };
    
    protected AbstractContainerElement() 
        : base(DefaultStyle) { }
    
    protected AbstractContainerElement(StyleCollection defaultStyle) 
        : base(defaultStyle) { }
}