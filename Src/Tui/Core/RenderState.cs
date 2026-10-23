// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Common;

namespace NeoKolors.Tui.Core;

/// <summary>
/// Represents an immutable snapshot of the rendering state in the <see cref="RenderContext"/> state stack.
/// </summary>
public readonly record struct RenderState {
    /// <summary>
    /// The coordinate translation offset relative to the canvas origin.
    /// </summary>
    public Point2D Origin { get; init; }

    /// <summary>
    /// The clipping boundary in global canvas coordinates. If null, no clip is applied beyond canvas bounds.
    /// </summary>
    public Rect2D? Clip { get; init; }

    /// <summary>
    /// The active z-index priority for drawing operations.
    /// </summary>
    public int ZIndex { get; init; }

    /// <summary>
    /// The active base style applied to rendered text and cells.
    /// </summary>
    public NKStyle Style { get; init; }

    /// <summary>
    /// Flag indicating whether drawing operations are permitted to write outside the current element's clip/context area.
    /// </summary>
    public bool AllowOverflow { get; init; }

    /// <summary>
    /// The designated context area bounds of the current element (in global canvas coordinates).
    /// </summary>
    public Rect2D? ContextArea { get; init; }

    public RenderState() {
        Origin        = Point2D.Zero;
        Clip          = null;
        ZIndex        = 0;
        Style         = NKStyle.Default;
        AllowOverflow = false;
        ContextArea   = null;
    }

    public RenderState(
        Point2D origin,
        Rect2D? clip,
        int     zIndex,
        NKStyle style,
        bool    allowOverflow,
        Rect2D? contextArea
    ) {
        Origin        = origin;
        Clip          = clip;
        ZIndex        = zIndex;
        Style         = style;
        AllowOverflow = allowOverflow;
        ContextArea   = contextArea;
    }
}