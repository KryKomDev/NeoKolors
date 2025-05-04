//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

public enum DisplayType {
    
    /// <summary>
    /// The element is completely removed.
    /// </summary>
    NONE,
    
    /// <summary>
    /// Displays an element as an inline element (like &lt;span&gt;). Any height and width properties will have no
    /// effect. This is the default.
    /// </summary>
    INLINE,     
    
    /// <summary>
    /// Displays an element as an inline-level block container. The element itself is formatted as an inline element,
    /// but you can apply height and width values.
    /// </summary>
    INLINE_BLOCK,
    
    /// <summary>
    /// Displays an element as an inline-level flex container.
    /// </summary>
    INLINE_FLEX,
    
    /// <summary>
    /// Displays an element as an inline-level grid container.
    /// </summary>
    INLINE_GRID,
    
    /// <summary>
    /// Displays an element as a block element (like &lt;p&gt;). It starts on a new line and takes up the whole width.
    /// </summary>
    BLOCK,
    
    /// <summary>
    /// Displays an element as a block-level flex container.
    /// </summary>
    FLEX,
    
    /// <summary>
    /// Displays an element as a block-level grid container.
    /// </summary>
    GRID
}