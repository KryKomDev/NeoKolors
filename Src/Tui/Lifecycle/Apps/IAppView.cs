// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle.Apps;

/// <summary>
/// Contract representing an application view or presentation surface that supports invalidation and rendering.
/// </summary>
public interface IAppView {
    /// <summary>
    /// Gets a value indicating whether the view requires rendering on the next pass.
    /// </summary>
    bool IsDirty { get; }

    /// <summary>
    /// Marks the view as dirty, signaling that visual content must be repainted.
    /// </summary>
    void Invalidate();

    /// <summary>
    /// Resets the dirty flag after rendering has completed.
    /// </summary>
    void ResetDirty();

    /// <summary>
    /// Renders the current visual state of the application view.
    /// </summary>
    void Render();
}
