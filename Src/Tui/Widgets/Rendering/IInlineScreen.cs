// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Widgets.Rendering;

/// <summary>
/// Contract representing an inline terminal character screen that renders using relative cursor movements
/// directly within the standard console stream.
/// </summary>
public interface IInlineScreen : ICharScreen {
    /// <summary>
    /// Gets the number of rows rendered in the previous frame.
    /// </summary>
    int RenderedHeight { get; }

    /// <summary>
    /// Completes the inline rendering session.
    /// </summary>
    /// <param name="persist">If <c>true</c>, preserves the final output on screen and advances cursor to a new line; otherwise, clears the rendered area.</param>
    void Finish(bool persist = true);
}