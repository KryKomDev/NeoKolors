// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Fonts;

public interface ICharCanvas {
    
    /// <summary>
    /// Retrieves all the lines currently stored in the character canvas.
    /// </summary>
    /// <returns>
    /// An array of strings representing the lines in the character canvas.
    /// </returns>
    public string[] GetLines();

    /// <summary>
    /// Retrieves all the characters stored in the character canvas as a two-dimensional array.
    /// </summary>
    /// <returns>
    /// A two-dimensional array of nullable characters representing the content of the character canvas.
    /// </returns>
    public char?[,] GetChars();

    /// <summary>
    /// Gets the width of the character canvas.
    /// </summary>
    /// <value>
    /// The width of the canvas, typically represented as the number of columns available in the canvas.
    /// </value>
    public int Width { get; }

    /// <summary>
    /// Gets the height of the character canvas.
    /// </summary>
    /// <value>
    /// The height of the canvas, typically represented as the number of rows available in the canvas.
    /// </value>
    public int Height { get; }

    /// <summary>
    /// Clears all the content currently stored in the character canvas.
    /// </summary>
    /// <remarks>
    /// This operation resets the canvas, removing all lines and restoring it to an empty state.
    /// </remarks>
    public void Clear();

    /// <summary>
    /// Places a glyph at the specified coordinates on the character canvas.
    /// </summary>
    /// <param name="x">The x-coordinate where the glyph will be placed.</param>
    /// <param name="y">The y-coordinate where the glyph will be placed.</param>
    /// <param name="glyph">The glyph to place on the canvas.</param>
    public void PlaceGlyph(int x, int y, IGlyph glyph);

    /// <summary>
    /// Places a string at the specified position on the character canvas.
    /// </summary>
    /// <param name="x">The x-coordinate where the string will start.</param>
    /// <param name="y">The y-coordinate where the string will start.</param>
    /// <param name="s">The string to place on the canvas.</param>
    public void PlaceString(int x, int y, string s);

    /// <summary>
    /// Places the content of another character canvas at the specified position on the current canvas.
    /// </summary>
    /// <param name="x">The x-coordinate where the top-left corner of the canvas will be placed.</param>
    /// <param name="y">The y-coordinate where the top-left corner of the canvas will be placed.</param>
    /// <param name="canvas">The character canvas to be placed onto the current canvas.</param>
    public void PlaceCanvas(int x, int y, ICharCanvas canvas);
}