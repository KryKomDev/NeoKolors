// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Tui.Core;

/// <summary>
/// Determines how an element behaves during mouse hit-testing.
/// </summary>
public enum HitTestBehavior {
    /// <summary>
    /// Targets that are hit consume pointer events and prevent targets beneath them from being hit.
    /// </summary>
    OPAQUE,

    /// <summary>
    /// Targets that are hit receive events, but allow events to pass through to underlying targets if unhandled.
    /// </summary>
    TRANSLUCENT,

    /// <summary>
    /// Completely ignores hit-testing (equivalent to CSS 'pointer-events: none').
    /// </summary>
    NONE
}
