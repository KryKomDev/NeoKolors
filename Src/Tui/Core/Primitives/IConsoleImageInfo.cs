// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using SkiaSharp;

namespace NeoKolors.Tui.Core;

public interface IConsoleImageInfo {
    
    /// <summary>
    /// Represents the bitmap image associated with the object, encapsulating
    /// pixel data and rendering information. Provides functionality for image
    /// manipulation and rendering within the SkiaSharp framework.
    /// </summary>
    public SKBitmap Image { get; }

    /// <summary>
    /// Specifies the positional offset of the image relative to its rendering context,
    /// defined as a two-dimensional point with X and Y coordinates.
    /// Facilitates precise placement within the user interface layout or rendering surface.
    /// </summary>
    public Point2D Offset { get; }

    /// <summary>
    /// Defines the size of each character cell in the rendered output,
    /// measured in pixels. This property is essential for determining
    /// the scaling of the image within a character-based terminal environment.
    /// </summary>
    public Size2D CharSize { get; }
    
    /// <summary>
    /// Defines the rendering order of the object relative to other elements.
    /// A higher value indicates that the object is rendered above those with lower values,
    /// thereby determining its layering priority within the render stack.
    /// </summary>
    public int ZIndex { get; }
}