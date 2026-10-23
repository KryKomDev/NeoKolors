// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Console;

namespace NeoKolors.Tui.Core;

/// <summary>
/// Provides event data for element-level mouse interaction events in NeoKolors.Tui.
/// </summary>
public class ElementMouseEventArgs {

    /// <summary>
    /// Gets the mouse button associated with this event.
    /// </summary>
    public MouseButton Button { get; init; }

    /// <summary>
    /// Gets the cursor position in global screen/canvas coordinates.
    /// </summary>
    public Point2D GlobalPosition { get; init; }

    /// <summary>
    /// Gets the cursor position relative to the target element's top-left origin.
    /// </summary>
    public Point2D LocalPosition { get; init; }

    /// <summary>
    /// Gets the keyboard modifier keys held down during the mouse event.
    /// </summary>
    public KeyModifiers Modifiers { get; init; }

    /// <summary>
    /// Gets whether this event represents a button press.
    /// </summary>
    public bool IsPress { get; init; }

    /// <summary>
    /// Gets whether this event represents a button release.
    /// </summary>
    public bool IsRelease { get; init; }

    /// <summary>
    /// Gets whether this event represents a completed click (press and release on the same target).
    /// </summary>
    public bool IsClick { get; init; }

    /// <summary>
    /// Gets whether this event represents cursor movement.
    /// </summary>
    public bool IsMove { get; init; }

    /// <summary>
    /// Gets whether this event represents hovering over the element.
    /// </summary>
    public bool IsHover { get; init; }

    /// <summary>
    /// Gets the target element that received the event.
    /// </summary>
    public object? Target { get; init; }

    /// <summary>
    /// Gets or sets whether this event has been handled.
    /// When set to true, event propagation/bubbling to parent elements is halted.
    /// </summary>
    public bool Handled { get; set; }

    /// <summary>
    /// Gets whether this event represents a scroll wheel up action.
    /// </summary>
    public bool IsWheelUp => Button == MouseButton.WHEEL_UP;

    /// <summary>
    /// Gets whether this event represents a scroll wheel down action.
    /// </summary>
    public bool IsWheelDown => Button == MouseButton.WHEEL_DOWN;

    public ElementMouseEventArgs(
        MouseButton  button,
        Point2D      globalPosition,
        Point2D      localPosition,
        KeyModifiers modifiers,
        bool         isPress,
        bool         isRelease,
        bool         isClick,
        bool         isMove,
        bool         isHover,
        object?      target
    ) {
        Button         = button;
        GlobalPosition = globalPosition;
        LocalPosition  = localPosition;
        Modifiers      = modifiers;
        IsPress        = isPress;
        IsRelease      = isRelease;
        IsClick        = isClick;
        IsMove         = isMove;
        IsHover        = isHover;
        Target         = target;
        Handled        = false;
    }

    /// <summary>
    /// Returns the target cast to the specified type, or default if incompatible.
    /// </summary>
    public T? TargetAs<T>() => Target is T t ? t : default;
}
