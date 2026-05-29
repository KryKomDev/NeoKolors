// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Fonts;

public interface IAsciiFont {
    
    public string Name { get; }
    
    
    
    public void PlaceString(string str, ICharCanvas canvas) => 
        PlaceString(str, canvas, canvas.Width);
    
    public void PlaceString(string str, ICharCanvas canvas, int maxWidth) =>
        PlaceString(str, canvas, new Area2D(Point2D.Zero, new Point2D(maxWidth, canvas.Height)));
    
    public void PlaceString(
        string str, ICharCanvas canvas, Area2D bounds, NKStyle style,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign   verticalAlign   = VerticalAlign.TOP, 
        bool            overflow        = false);
    
    
    public void PlaceString(AnsiString str, ICharCanvas canvas) =>
        PlaceString(str, canvas, canvas.Width);
    
    public void PlaceString(AnsiString str, ICharCanvas canvas, int maxWidth) =>
        PlaceString(str, canvas, new Area2D(Point2D.Zero, new Point2D(maxWidth, canvas.Height)));
    
    public void PlaceString(
        AnsiString str, ICharCanvas canvas, Area2D bounds,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign   verticalAlign   = VerticalAlign.TOP, 
        bool            overflow        = false);

    /// <summary>
    /// Calculates the size of the string if the longest word in the string is the maximum width of the canvas.
    /// </summary>
    /// <param name="str">The string for which the minimum size is to be calculated.</param>
    /// <returns>A <see cref="Size"/> object representing the minimum width and height necessary
    /// to render the string.</returns>
    public Size2D GetMinSize(string str);

    /// <summary>
    /// Calculates the size required to render the provided string using the current font.
    /// </summary>
    /// <param name="str">The string for which the size is to be calculated.</param>
    /// <returns>A <see cref="Size"/> object representing the width and height necessary to render the string.</returns>
    public Size2D GetSize(string str);

    /// <summary>
    /// Calculates the size of the string when rendered with a maximum width.
    /// </summary>
    /// <param name="str">The string to measure.</param>
    /// <param name="maxWidth">The maximum width allowed for the string.</param>
    /// <returns>A <see cref="Size"/> object representing the size of the string.</returns>
    public Size2D GetSize(string str, int maxWidth);
    
    /// <summary>
    /// Calculates the size of the string if the longest word in the string is the maximum width of the canvas.
    /// </summary>
    /// <param name="str">The string for which the minimum size is to be calculated.</param>
    /// <returns>A <see cref="Size"/> object representing the minimum width and height necessary
    /// to render the string.</returns>
    public Size2D GetMinSize(AnsiString str);

    /// <summary>
    /// Calculates the size required to render the provided string using the current font.
    /// </summary>
    /// <param name="str">The string for which the size is to be calculated.</param>
    /// <returns>A <see cref="Size"/> object representing the width and height necessary to render the string.</returns>
    public Size2D GetSize(AnsiString str);

    /// <summary>
    /// Calculates the size of the string when rendered with a maximum width.
    /// </summary>
    /// <param name="str">The string to measure.</param>
    /// <param name="maxWidth">The maximum width allowed for the string.</param>
    /// <returns>A <see cref="Size"/> object representing the size of the string.</returns>
    public Size2D GetSize(AnsiString str, int maxWidth);
    
    /// <summary>
    /// Returns the default char font. This font will render a character as it is.
    /// </summary>
    public static IAsciiFont Default { get; } = new DefaultFont();
}