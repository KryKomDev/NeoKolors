// NeoKolors
// Copyright (c) krystof 2026

using Metriks;

namespace NeoKolors.Tui.Core;

public interface IBorderStyle {
    
    /// <summary>
    /// Draws a border within the specified rectangular region onto the provided character canvas.
    /// </summary>
    /// <param name="rect">The rectangular region where the border will be drawn.</param>
    /// <param name="canvas">The character canvas onto which the border will be rendered.</param>
    /// <param name="z">The z-layer of the border.</param>
    public void Draw(Rect2D rect, in ICharDrawer canvas, int z);

    /// <summary>
    /// Calculates the content box dimensions by removing the border thickness from the given size.
    /// </summary>
    /// <param name="size">The size of the border box, including borders.</param>
    /// <returns>
    /// A rectangular region representing the dimensions of the content box, excluding the borders.
    /// </returns>
    public Rect2D GetContentBox(Size2D size);

    /// <summary>
    /// Calculates the border box dimensions by adding the border thickness to the given content size.
    /// </summary>
    /// <param name="size">The size of the content box, excluding borders.</param>
    /// <returns>
    /// A rectangular region representing the dimensions of the border box, including the borders.
    /// </returns>
    public Size2D GetBorderBox(Size2D size);
}