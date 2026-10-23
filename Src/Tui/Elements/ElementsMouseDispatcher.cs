// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Console;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Dispatches incoming console mouse events to the registered hit targets from a render pass.
/// Coordinates hover transitions, button presses, click detection, and event bubbling.
/// </summary>
public class ElementsMouseDispatcher {

    private readonly List<HitTarget> _lastHoveredTargets = new();
    private          HitTarget?       _pressedTarget;
    private          Rect2D?          _pressedBounds;
    private          object?          _pressedElement;
    private          MouseButton      _pressedButton = MouseButton.RELEASE;

    /// <summary>
    /// Gets the target currently pressed down, if any.
    /// </summary>
    public HitTarget? PressedTarget => _pressedTarget;

    /// <summary>
    /// Gets the targets hovered during the most recent mouse event.
    /// </summary>
    public IReadOnlyList<HitTarget> HoveredTargets => _lastHoveredTargets;

    /// <summary>
    /// Dispatches an incoming console <see cref="MouseEventArgs"/> to the active targets in the specified <see cref="HitMap"/>.
    /// </summary>
    /// <param name="hitMap">The hit map containing targets registered during the last render pass.</param>
    /// <param name="m">The raw console mouse event arguments.</param>
    public void Dispatch(HitMap hitMap, MouseEventArgs m) {
        if (hitMap == null) return;

        var currentHits = hitMap.HitTest(m.Position);

        // 1. Process Hover Transitions (MouseEnter / MouseLeave)
        for (int i = _lastHoveredTargets.Count - 1; i >= 0; i--) {
            var prev = _lastHoveredTargets[i];
            bool stillHovered = currentHits.Any(curr =>
                ReferenceEquals(curr, prev) ||
                (curr.Element != null && prev.Element != null && ReferenceEquals(curr.Element, prev.Element)) ||
                curr.GlobalBounds.Equals(prev.GlobalBounds)
            );

            if (!stillHovered) {
                prev.IsHovered?.Set(false);

                var localPos = m.Position - prev.LocalOrigin;
                var leaveArgs = new ElementMouseEventArgs(
                    button: m.Button,
                    globalPosition: m.Position,
                    localPosition: localPos,
                    modifiers: m.Modifiers,
                    isPress: false,
                    isRelease: false,
                    isClick: false,
                    isMove: false,
                    isHover: false,
                    target: prev.Element
                );

                prev.OnMouseLeave?.Invoke(leaveArgs);
            }
        }

        for (int i = 0; i < currentHits.Count; i++) {
            var curr = currentHits[i];
            bool wasHovered = _lastHoveredTargets.Any(prev =>
                ReferenceEquals(curr, prev) ||
                (curr.Element != null && prev.Element != null && ReferenceEquals(curr.Element, prev.Element)) ||
                curr.GlobalBounds.Equals(prev.GlobalBounds)
            );

            curr.IsHovered?.Set(true);

            if (!wasHovered) {
                var localPos = m.Position - curr.LocalOrigin;
                var enterArgs = new ElementMouseEventArgs(
                    button: m.Button,
                    globalPosition: m.Position,
                    localPosition: localPos,
                    modifiers: m.Modifiers,
                    isPress: false,
                    isRelease: false,
                    isClick: false,
                    isMove: false,
                    isHover: true,
                    target: curr.Element
                );

                curr.OnMouseEnter?.Invoke(enterArgs);
            }
        }

        _lastHoveredTargets.Clear();
        _lastHoveredTargets.AddRange(currentHits);

        // 2. Process Cursor Movement (MouseMove)
        if (m.Moved) {
            foreach (var target in currentHits) {
                var localPos = m.Position - target.LocalOrigin;
                var moveArgs = new ElementMouseEventArgs(
                    button: m.Button,
                    globalPosition: m.Position,
                    localPosition: localPos,
                    modifiers: m.Modifiers,
                    isPress: false,
                    isRelease: false,
                    isClick: false,
                    isMove: true,
                    isHover: true,
                    target: target.Element
                );

                target.OnMouseMove?.Invoke(moveArgs);
                if (moveArgs.Handled) break;
            }
        }

        // 3. Process Mouse Down (Press)
        if (m.IsPress) {
            if (currentHits.Count > 0) {
                _pressedTarget  = currentHits[0];
                _pressedBounds  = _pressedTarget.GlobalBounds;
                _pressedElement = _pressedTarget.Element;
                _pressedButton  = m.Button;
            }

            foreach (var target in currentHits) {
                target.IsPressed?.Set(true);

                var localPos = m.Position - target.LocalOrigin;
                var pressArgs = new ElementMouseEventArgs(
                    button: m.Button,
                    globalPosition: m.Position,
                    localPosition: localPos,
                    modifiers: m.Modifiers,
                    isPress: true,
                    isRelease: false,
                    isClick: false,
                    isMove: false,
                    isHover: true,
                    target: target.Element
                );

                target.OnMouseDown?.Invoke(pressArgs);
                if (pressArgs.Handled) break;
            }
        }

        // 4. Process Mouse Up (Release) & Completed Click
        if (m.IsRelease) {
            _pressedTarget?.IsPressed?.Set(false);

            foreach (var target in currentHits) {
                target.IsPressed?.Set(false);

                var localPos = m.Position - target.LocalOrigin;
                var releaseArgs = new ElementMouseEventArgs(
                    button: m.Button,
                    globalPosition: m.Position,
                    localPosition: localPos,
                    modifiers: m.Modifiers,
                    isPress: false,
                    isRelease: true,
                    isClick: false,
                    isMove: false,
                    isHover: true,
                    target: target.Element
                );

                target.OnMouseUp?.Invoke(releaseArgs);
                if (releaseArgs.Handled) break;
            }

            // Click detection: A full click occurred if the release occurs on the target that was pressed
            bool isClickOnPressed = false;
            if (_pressedTarget != null) {
                isClickOnPressed = currentHits.Any(t =>
                    ReferenceEquals(t, _pressedTarget) ||
                    (_pressedElement != null && ReferenceEquals(t.Element, _pressedElement)) ||
                    (_pressedBounds.HasValue && t.GlobalBounds.Equals(_pressedBounds.Value))
                );

                if (!isClickOnPressed && _pressedBounds.HasValue) {
                    var b = _pressedBounds.Value;
                    isClickOnPressed = m.Position.X >= b.LowerX && m.Position.X < b.HigherX &&
                                       m.Position.Y >= b.LowerY && m.Position.Y < b.HigherY;
                }
            }

            if (isClickOnPressed) {
                var clickButton = _pressedButton != MouseButton.RELEASE ? _pressedButton : m.Button;
                var clickTargets = currentHits.Count > 0
                    ? currentHits
                    : (_pressedTarget != null ? new List<HitTarget> { _pressedTarget } : currentHits);

                foreach (var target in clickTargets) {
                    var localPos = m.Position - target.LocalOrigin;
                    var clickArgs = new ElementMouseEventArgs(
                        button: clickButton,
                        globalPosition: m.Position,
                        localPosition: localPos,
                        modifiers: m.Modifiers,
                        isPress: false,
                        isRelease: false,
                        isClick: true,
                        isMove: false,
                        isHover: true,
                        target: target.Element
                    );

                    target.OnClick?.Invoke(clickArgs);
                    if (clickArgs.Handled) break;
                }
            }

            _pressedTarget  = null;
            _pressedBounds  = null;
            _pressedElement = null;
            _pressedButton  = MouseButton.RELEASE;
        }

        // 5. Process Wheel Scroll
        if (m.Button is MouseButton.WHEEL_UP or MouseButton.WHEEL_DOWN) {
            foreach (var target in currentHits) {
                var localPos = m.Position - target.LocalOrigin;
                var scrollArgs = new ElementMouseEventArgs(
                    button: m.Button,
                    globalPosition: m.Position,
                    localPosition: localPos,
                    modifiers: m.Modifiers,
                    isPress: false,
                    isRelease: false,
                    isClick: false,
                    isMove: false,
                    isHover: true,
                    target: target.Element
                );

                target.OnScroll?.Invoke(scrollArgs);
                if (scrollArgs.Handled) break;
            }
        }
    }

    /// <summary>
    /// Resets all internal state (hovered targets and pressed target).
    /// </summary>
    public void Reset() {
        foreach (var target in _lastHoveredTargets) {
            target.IsHovered?.Set(false);
            target.IsPressed?.Set(false);
        }

        _pressedTarget?.IsPressed?.Set(false);
        _lastHoveredTargets.Clear();
        _pressedTarget  = null;
        _pressedBounds  = null;
        _pressedElement = null;
        _pressedButton  = MouseButton.RELEASE;
    }
}
