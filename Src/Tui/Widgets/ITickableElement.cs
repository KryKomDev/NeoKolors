// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Widgets;

/// <summary>
/// Defines an element that receives periodic tick updates for animation or time-based transitions.
/// </summary>
public interface ITickableElement {
    /// <summary>
    /// Updates the element's internal state given the elapsed time since the previous tick.
    /// </summary>
    /// <param name="delta">The time elapsed since the last tick.</param>
    void Tick(TimeSpan delta);
}