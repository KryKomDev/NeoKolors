// NeoKolors
// Copyright (c) krystof 2026

using Metriks;

namespace NeoKolors.Tui.Core;

public interface ITableStyle {

    /// <summary>
    ///     Draws the elements of a table starting at the specified offset using the provided
    ///     drawer and z-index.
    /// </summary>
    /// <param name="offset">
    ///     The point on the canvas where the drawing of the table will start.
    /// </param>
    /// <param name="elements">
    ///     A 2D array representing the size of each element in the table.
    /// </param>
    /// <param name="drawer">
    ///     The drawer used to render the table elements onto the canvas.
    /// </param>
    /// <param name="z">
    ///     The z-index to use when drawing the elements of the table, defining their rendering
    ///     order.
    /// </param>
    public void Draw(Point2D offset, Size2D[,] elements, in ICharDrawer drawer, int z);

    /// <summary>
    ///     Computes the offsets of each element in the table based on their size within the
    ///     specified layout configuration.
    /// </summary>
    /// <param name="elements">
    ///     A 2D array where each element represents the size of a table cell.
    /// </param>
    /// <returns>
    ///     A 2D array of offsets where each offset corresponds to the starting position of a table
    ///     cell in the layout.
    /// </returns>
    public Point2D[,] GetElementOffsets(Size2D[,] elements);

    /// <summary>
    ///     Computes the total size of a table based on the dimensions of its elements.
    /// </summary>
    /// <param name="elements">
    ///     A 2D array where each element represents the size of a table cell.
    /// </param>
    /// <returns>
    ///     The total size of the table as a single Size2D object, which encapsulates the combined
    ///     width and height of all elements.
    /// </returns>
    public Size2D GetTotalSize(Size2D[,] elements);
}