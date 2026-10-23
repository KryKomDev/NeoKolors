// NeoKolors
// Copyright (c) krystof 2026

using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Provides fluent modifier extension methods for making any element interactive.
/// </summary>
public static class ElementMouseExtensions {

    /// <summary>
    /// Configures or wraps this element in a <see cref="MouseRegion"/> that responds to clicks.
    /// </summary>
    public static MouseRegion OnClick(this IElement element, Action<ElementMouseEventArgs> onClick) {
        if (element is MouseRegion mr) {
            mr.OnClick = onClick;
            return mr;
        }
        return new(element, onClick: onClick);
    }

    /// <summary>
    /// Configures or wraps this element in a <see cref="MouseRegion"/> that responds to cursor enter and leave events.
    /// </summary>
    public static MouseRegion OnHover(
        this IElement                  element,
        Action<ElementMouseEventArgs>? onMouseEnter = null,
        Action<ElementMouseEventArgs>? onMouseLeave = null
    ) {
        if (element is MouseRegion mr) {
            if (onMouseEnter != null) mr.OnMouseEnter = onMouseEnter;
            if (onMouseLeave != null) mr.OnMouseLeave = onMouseLeave;
            return mr;
        }
        return new(element, onMouseEnter: onMouseEnter, onMouseLeave: onMouseLeave);
    }

    /// <summary>
    /// Configures or wraps this element in a <see cref="MouseRegion"/> that responds to mouse button press and release.
    /// </summary>
    public static MouseRegion OnButton(
        this IElement                  element,
        Action<ElementMouseEventArgs>? onMouseDown = null,
        Action<ElementMouseEventArgs>? onMouseUp   = null
    ) {
        if (element is MouseRegion mr) {
            if (onMouseDown != null) mr.OnMouseDown = onMouseDown;
            if (onMouseUp != null) mr.OnMouseUp = onMouseUp;
            return mr;
        }
        return new(element, onMouseDown: onMouseDown, onMouseUp: onMouseUp);
    }

    /// <summary>
    /// Configures or wraps this element in a <see cref="MouseRegion"/> that responds to mouse wheel scrolling.
    /// </summary>
    public static MouseRegion OnScroll(this IElement element, Action<ElementMouseEventArgs> onScroll) {
        if (element is MouseRegion mr) {
            mr.OnScroll = onScroll;
            return mr;
        }
        return new(element, onScroll: onScroll);
    }

    /// <summary>
    /// Configures or wraps this element in a <see cref="MouseRegion"/> with the specified hit-test behavior.
    /// </summary>
    public static MouseRegion WithHitBehavior(this IElement element, HitTestBehavior behavior) {
        if (element is MouseRegion mr) {
            mr.Behavior = behavior;
            return mr;
        }
        return new(element, behavior: behavior);
    }
}
