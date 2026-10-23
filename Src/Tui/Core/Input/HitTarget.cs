// NeoKolors
// Copyright (c) krystof 2026

using Metriks;

namespace NeoKolors.Tui.Core;

/// <summary>
/// Represents a registered interactive spatial region for hit-testing in NeoKolors.Tui.
/// </summary>
public class HitTarget {

    /// <summary>
    /// Gets the element instance associated with this hit target.
    /// </summary>
    public object? Element { get; init; }

    /// <summary>
    /// Gets the bounding rectangle of the target in global screen/canvas coordinates,
    /// clipped to any active parent clipping boundaries.
    /// </summary>
    public Rect2D GlobalBounds { get; init; }

    /// <summary>
    /// Gets the top-left origin of the element in global coordinates, used to compute relative local coordinates.
    /// </summary>
    public Point2D LocalOrigin { get; init; }

    /// <summary>
    /// Gets the z-index visual layer of this target.
    /// </summary>
    public int ZIndex { get; init; }

    /// <summary>
    /// Gets the rendering sequence order of this target within the current render pass.
    /// Used as secondary sort key for elements sharing the same z-index (Painter's Algorithm).
    /// </summary>
    public int PaintOrder { get; init; }

    /// <summary>
    /// Gets how this target behaves during hit-testing (opaque, translucent, or none).
    /// </summary>
    public HitTestBehavior Behavior { get; init; } = HitTestBehavior.OPAQUE;

    /// <summary>
    /// Callback invoked when a click is completed on this element.
    /// </summary>
    public Action<ElementMouseEventArgs>? OnClick { get; init; }

    /// <summary>
    /// Callback invoked when a mouse button is pressed down within this element.
    /// </summary>
    public Action<ElementMouseEventArgs>? OnMouseDown { get; init; }

    /// <summary>
    /// Callback invoked when a mouse button is released within this element.
    /// </summary>
    public Action<ElementMouseEventArgs>? OnMouseUp { get; init; }

    /// <summary>
    /// Callback invoked when the cursor enters the element boundary.
    /// </summary>
    public Action<ElementMouseEventArgs>? OnMouseEnter { get; init; }

    /// <summary>
    /// Callback invoked when the cursor leaves the element boundary.
    /// </summary>
    public Action<ElementMouseEventArgs>? OnMouseLeave { get; init; }

    /// <summary>
    /// Callback invoked when the cursor moves within the element boundary.
    /// </summary>
    public Action<ElementMouseEventArgs>? OnMouseMove { get; init; }

    /// <summary>
    /// Callback invoked when the mouse wheel is scrolled within the element boundary.
    /// </summary>
    public Action<ElementMouseEventArgs>? OnScroll { get; init; }

    /// <summary>
    /// Optional bindable boolean tracking whether this target is currently hovered.
    /// </summary>
    public Bindable<bool>? IsHovered { get; init; }

    /// <summary>
    /// Optional bindable boolean tracking whether this target is currently pressed.
    /// </summary>
    public Bindable<bool>? IsPressed { get; init; }
}
