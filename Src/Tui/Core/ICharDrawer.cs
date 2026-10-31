// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Common;
using SkiaSharp;

namespace NeoKolors.Tui.Core;

public interface ICharDrawer {
    
    /// <summary>
    /// Gets the dimensions of the character canvas, represented as a 2D size.
    /// </summary>
    /// <remarks>
    /// This property provides the width and height of the canvas used for rendering characters.
    /// It is useful for determining the drawable area and managing layout or rendering boundaries.
    /// </remarks>
    public Size2D Size { get; }

    public int Width  => Size.X;
    public int Height => Size.Y;

    /// <summary>
    /// Draws the content of the specified <see cref="ICharCanvas" /> onto the current canvas with
    /// an optional offset and z-index transformation.
    /// </summary>
    /// <param name="canvas">
    /// The source <see cref="ICharCanvas" /> whose content will be drawn onto
    /// the current canvas.
    /// </param>
    /// <param name="xOffset">
    /// The x coordinate of the point that specifies the position on the current canvas where the
    /// content will be drawn.
    /// </param>
    /// <param name="yOffset">
    /// The y coordinate of the point that specifies the position on the current canvas where the
    /// content will be drawn.
    /// </param>
    /// <param name="z">
    /// The z-index to use when drawing the specified <see cref="ICharCanvas" />
    /// to the canvas.
    /// </param>
    public void Draw(ICharCanvas canvas, int xOffset, int yOffset, int z);

    /// <summary>
    /// Draws a single <see cref="AnsiChar" /> onto the current canvas at the specified position
    /// and z-index.
    /// </summary>
    /// <param name="c">The ANSI character to be drawn on the canvas.</param>
    /// <param name="xOffset">
    /// The x coordinate of the point that specifies the position on the canvas where the
    /// character will be drawn.
    /// </param>
    /// <param name="yOffset">
    /// The y coordinate of the point that specifies the position on the canvas where the
    /// character will be drawn.
    /// </param>
    /// <param name="z">
    /// The z-index at which the character will be rendered. Higher values
    /// represent elements in the foreground.
    /// </param>
    public void Draw(AnsiChar c, int xOffset, int yOffset, int z);

    /// <summary>
    /// Draws the specified <see cref="AnsiString" /> on the canvas at the given offset, applying
    /// the specified z-index.
    /// </summary>
    /// <param name="str">The AnsiString to be drawn on the canvas.</param>
    /// <param name="xOffset">The x position on the canvas where the string will be drawn.</param>
    /// <param name="yOffset">The y position on the canvas where the string will be drawn.</param>
    /// <param name="z">
    /// The z-index transformation to be applied during the drawing
    /// operation.
    /// </param>
    public void Draw(AnsiString str, int xOffset, int yOffset, int z);

    /// <summary>
    /// Draws the specified image onto the canvas within the defined area and using the provided sampling options.
    /// </summary>
    /// <param name="image">
    ///     The <see cref="SKImage" /> to be drawn onto the canvas.
    /// </param>
    /// <param name="xOffset">The x-axis offset where the image will be placed.</param>
    /// <param name="yOffset">The y-axis offset where the image will be placed.</param>
    /// <param name="xSize">The width of the image in characters.</param>
    /// <param name="ySize">The height of the image in characters.</param>
    /// <param name="zLayer"></param>
    /// <param name="samplingOptions">
    ///     The options used for sampling when rendering the image. Defaults to the standard sampling
    ///     options if not specified.
    /// </param>
    public void Draw(
        SKImage           image,
        int               xOffset,
        int               yOffset,
        int               xSize,
        int               ySize,
        int               zLayer,
        SKSamplingOptions samplingOptions = default
    );

    /// <summary>
    /// Draws a vertical line on the canvas using the specified character, starting at the given
    /// position and extending for the specified length.
    /// </summary>
    /// <param name="c">
    /// The <see cref="AnsiChar" /> to be used for drawing the vertical line.
    /// </param>
    /// <param name="x">
    /// The x coordinate of the starting point of the vertical line.
    /// </param>
    /// <param name="y">
    /// The y coordinate of the starting point of the vertical line.
    /// </param>
    /// <param name="length">
    /// The number of characters the vertical line should span.
    /// </param>
    /// <param name="z">
    /// The z-index to use when drawing the vertical line on the canvas.
    /// </param>
    public void DrawVerticalLine(AnsiChar c, int x, int y, int length, int z);

    /// <summary>
    /// Draws a horizontal line on the canvas using the specified character, starting at the given position
    /// and extending for the specified length. The line is drawn with a specified z-index for depth control.
    /// </summary>
    /// <param name="c">
    /// The <see cref="AnsiChar" /> used to draw the line.
    /// </param>
    /// <param name="x">
    /// The x coordinate of the starting position of the horizontal line.
    /// </param>
    /// <param name="y">
    /// The y coordinate of the starting position of the horizontal line.
    /// </param>
    /// <param name="length">
    /// The total number of characters to draw for the horizontal line.
    /// </param>
    /// <param name="z">
    /// The z-index to use when drawing the line onto the canvas.
    /// </param>
    public void DrawHorizontalLine(AnsiChar c, int x, int y, int length, int z);

    /// <summary>
    /// Fills a rectangular area on the canvas with the specified <see cref="AnsiChar" />.
    /// </summary>
    /// <param name="c">
    /// The <see cref="AnsiChar" /> to fill the rectangular area with.
    /// </param>
    /// <param name="x">
    /// The x coordinate of the top-left corner of the rectangle to be filled.
    /// </param>
    /// <param name="y">
    /// The y coordinate of the top-left corner of the rectangle to be filled.
    /// </param>
    /// <param name="width">
    /// The width of the rectangle to be filled.
    /// </param>
    /// <param name="height">
    /// The height of the rectangle to be filled.
    /// </param>
    /// <param name="z">
    /// The z-index to use when filling the rectangle on the canvas.
    /// </param>
    public void DrawRect(AnsiChar c, int x, int y, int width, int height, int z);

    /// <summary>
    /// Clears the canvas by resetting all its content and associated state to their default values.
    /// This keeps the canvas dimensions.
    /// </summary>
    public void Clean();
    

    // -------------------------------------------------------------------------------------- //
    //                               EXTENSION DRAWING METHODS                                //
    // -------------------------------------------------------------------------------------- //

    #region EXTENSION DRAWING METHODS
    
    /// <summary>
    /// Draws the content of the specified <see cref="ICharCanvas" /> onto the current canvas with
    /// an optional offset and z-index transformation.
    /// </summary>
    /// <param name="canvas">
    /// The source <see cref="ICharCanvas" /> whose content will be drawn onto
    /// the current canvas.
    /// </param>
    /// <param name="offset">
    /// The point that specifies the position on the current canvas where the
    /// content will be drawn.
    /// </param>
    /// <param name="z">
    /// The z-index to use when drawing the specified <see cref="ICharCanvas" />
    /// to the canvas.
    /// </param>
    public void Draw(ICharCanvas canvas, Point2D offset, int z) {
        Draw(canvas, offset.X, offset.Y, z);
    }

    /// <summary>
    /// Draws a single <see cref="AnsiChar" /> onto the current canvas at the specified position
    /// and z-index.
    /// </summary>
    /// <param name="c">The ANSI character to be drawn on the canvas.</param>
    /// <param name="offset">
    /// The point that specifies the position on the canvas where the
    /// character will be drawn.
    /// </param>
    /// <param name="z">
    /// The z-index at which the character will be rendered. Higher values
    /// represent elements in the foreground.
    /// </param>
    public void Draw(AnsiChar c, Point2D offset, int z) {
        Draw(c, offset.X, offset.Y, z);
    }

    /// <summary>
    /// Draws the specified <see cref="AnsiString" /> on the canvas at the given offset, applying
    /// the specified z-index.
    /// </summary>
    /// <param name="str">The AnsiString to be drawn on the canvas.</param>
    /// <param name="offset">The position on the canvas where the string will be drawn.</param>
    /// <param name="z">
    /// The z-index transformation to be applied during the drawing
    /// operation.
    /// </param>
    public void Draw(AnsiString str, Point2D offset, int z) {
        Draw(str, offset.X, offset.Y, z);   
    }

    /// <summary>
    /// Draws the specified image onto the canvas within the defined area and using the provided sampling options.
    /// </summary>
    /// <param name="image">
    /// The <see cref="SKImage" /> to be drawn onto the canvas.
    /// </param>
    /// <param name="area">
    /// The area on the canvas where the image will be drawn.
    /// </param>
    /// <param name="samplingOptions">
    /// The options used for sampling when rendering the image. Defaults to the standard sampling options if not specified.
    /// </param>
    public void Draw(SKImage image, Rect2D area, SKSamplingOptions samplingOptions = default) {
        Draw(image, area.LowerX, area.LowerY, area.SizeX, area.SizeY, 0, samplingOptions);
    }

    /// <summary>
    /// Draws a vertical line on the canvas using the specified character, starting at the given
    /// position and extending for the specified length.
    /// </summary>
    /// <param name="c">
    /// The <see cref="AnsiChar" /> to be used for drawing the vertical line.
    /// </param>
    /// <param name="offset">
    /// The offset of the starting point of the vertical line.
    /// </param>
    /// <param name="length">
    /// The number of characters the vertical line should span.
    /// </param>
    /// <param name="z">
    /// The z-index to use when drawing the vertical line on the canvas.
    /// </param>
    public void DrawVerticalLine(AnsiChar c, Point2D offset, int length, int z) {
        DrawVerticalLine(c, offset.X, offset.Y, length, z);
    }

    /// <summary>
    /// Draws a horizontal line on the canvas using the specified character, starting at the given position
    /// and extending for the specified length. The line is drawn with a specified z-index for depth control.
    /// </summary>
    /// <param name="c">
    /// The <see cref="AnsiChar" /> used to draw the line.
    /// </param>
    /// <param name="offset">
    /// The offset of the starting point of the horizontal line.
    /// </param>
    /// <param name="length">
    /// The total number of characters to draw for the horizontal line.
    /// </param>
    /// <param name="z">
    /// The z-index to use when drawing the line onto the canvas.
    /// </param>
    public void DrawHorizontalLine(AnsiChar c, Point2D offset, int length, int z) {
        DrawHorizontalLine(c, offset.X, offset.Y, length, z);
    }

    /// <summary>
    /// Fills a rectangular area on the canvas with the specified <see cref="AnsiChar" />.
    /// </summary>
    /// <param name="c">
    /// The <see cref="AnsiChar" /> to fill the rectangular area with.
    /// </param>
    /// <param name="offset">The offset of the top-left corner of the rectangle to be filled.</param>
    /// <param name="size">The size of the rectangle to be filled.</param>
    /// <param name="z">
    /// The z-index to use when filling the rectangle on the canvas.
    /// </param>
    public void DrawRect(AnsiChar c, Point2D offset, Size2D size, int z) {
        DrawRect(c, offset.X, offset.Y, size.X, size.Y, z);
    }

    /// <summary>
    /// Fills a rectangular area on the canvas with the specified <see cref="AnsiChar" />.
    /// </summary>
    /// <param name="c">
    /// The <see cref="AnsiChar" /> used to fill the rectangular area.
    /// </param>
    /// <param name="rect">The rectangle to be filled.</param>
    /// <param name="z">
    /// The z-index to use when filling the rectangle on the canvas.
    /// </param>
    public void DrawRect(AnsiChar c, Rect2D rect, int z) {
        DrawRect(c, rect.LowerX, rect.LowerY, rect.SizeX, rect.SizeY, z);
    }

    #endregion

}