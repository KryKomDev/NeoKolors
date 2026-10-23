// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A declarative element that wraps a child element and registers an interactive spatial region
/// for mouse interactions (clicks, hover states, scroll, and cursor movements).
/// </summary>
public class MouseRegion : IElement {

    /// <summary>
    /// Gets the wrapped child element.
    /// </summary>
    public IElement Child { get; init; }

    /// <summary>
    /// Gets or sets the hit-test behavior (OPAQUE, TRANSLUCENT, or NONE).
    /// </summary>
    public Bindable<HitTestBehavior> Behavior { get; set; }

    /// <summary>
    /// Reactive state tracking whether the mouse cursor is currently hovering over this region.
    /// </summary>
    public Bindable<bool> IsHovered { get; init; } = new Signal<bool>(false);

    /// <summary>
    /// Reactive state tracking whether a mouse button is currently held down within this region.
    /// </summary>
    public Bindable<bool> IsPressed { get; init; } = new Signal<bool>(false);

    public Bindable<Action<ElementMouseEventArgs>?> OnClick      { get; set; } = default;
    public Bindable<Action<ElementMouseEventArgs>?> OnMouseDown  { get; set; } = default;
    public Bindable<Action<ElementMouseEventArgs>?> OnMouseUp    { get; set; } = default;
    public Bindable<Action<ElementMouseEventArgs>?> OnMouseEnter { get; set; } = default;
    public Bindable<Action<ElementMouseEventArgs>?> OnMouseLeave { get; set; } = default;
    public Bindable<Action<ElementMouseEventArgs>?> OnMouseMove  { get; set; } = default;
    public Bindable<Action<ElementMouseEventArgs>?> OnScroll     { get; set; } = default;

    public MouseRegion(
        IElement                               child,
        Action<ElementMouseEventArgs>?         onClick      = null,
        Action<ElementMouseEventArgs>?         onMouseEnter = null,
        Action<ElementMouseEventArgs>?         onMouseLeave = null,
        Action<ElementMouseEventArgs>?         onMouseDown  = null,
        Action<ElementMouseEventArgs>?         onMouseUp    = null,
        Action<ElementMouseEventArgs>?         onMouseMove  = null,
        Action<ElementMouseEventArgs>?         onScroll     = null,
        HitTestBehavior                        behavior     = HitTestBehavior.OPAQUE,
        Bindable<bool>?                        isHovered    = null,
        Bindable<bool>?                        isPressed    = null
    ) {
        Child    = child ?? throw new ArgumentNullException(nameof(child));
        Behavior = behavior;
        if (onClick != null) OnClick = onClick;
        if (onMouseEnter != null) OnMouseEnter = onMouseEnter;
        if (onMouseLeave != null) OnMouseLeave = onMouseLeave;
        if (onMouseDown != null) OnMouseDown = onMouseDown;
        if (onMouseUp != null) OnMouseUp = onMouseUp;
        if (onMouseMove != null) OnMouseMove = onMouseMove;
        if (onScroll != null) OnScroll = onScroll;
        if (isHovered.HasValue) IsHovered = isHovered.Value;
        if (isPressed.HasValue) IsPressed = isPressed.Value;
    }

    public Size2D Measure(LayoutContext context) => Child.Measure(context);

    public void Render(RenderContext context, Size2D parent) {
        context.RegisterHitTarget(
            element: this,
            localBounds: new Rect2D(0, 0, parent.X, parent.Y),
            behavior: Behavior.AsT,
            onClick: OnClick.AsT,
            onMouseEnter: OnMouseEnter.AsT,
            onMouseLeave: OnMouseLeave.AsT,
            onMouseDown: OnMouseDown.AsT,
            onMouseUp: OnMouseUp.AsT,
            onMouseMove: OnMouseMove.AsT,
            onScroll: OnScroll.AsT,
            isHovered: IsHovered,
            isPressed: IsPressed
        );

        Child.Render(context, parent);
    }
}
