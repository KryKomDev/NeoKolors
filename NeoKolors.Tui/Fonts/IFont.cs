// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Fonts;

public interface IFont {
    public IFontInfo Info { get; }
    
    public void PlaceString(string str, ICharCanvas canvas);
    public void PlaceString(string str, ICharCanvas canvas, int maxWidth);
    public void PlaceString(
        string str, ICharCanvas canvas, Rectangle bounds, NKStyle style,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign   verticalAlign   = VerticalAlign.TOP, 
        bool            overflow        = false);

    /// <summary>
    /// Calculates the size of the string if the longest word in the string is the maximum width of the canvas.
    /// </summary>
    /// <param name="str">The string for which the minimum size is to be calculated.</param>
    /// <returns>A <see cref="Size"/> object representing the minimum width and height necessary
    /// to render the string.</returns>
    public Size GetMinSize(string str);

    /// <summary>
    /// Calculates the size required to render the provided string using the current font.
    /// </summary>
    /// <param name="str">The string for which the size is to be calculated.</param>
    /// <returns>A <see cref="Size"/> object representing the width and height necessary to render the string.</returns>
    public Size GetSize(string str);

    /// <summary>
    /// Calculates the size of the string when rendered with a maximum width.
    /// </summary>
    /// <param name="str">The string to measure.</param>
    /// <param name="maxWidth">The maximum width allowed for the string.</param>
    /// <returns>A <see cref="Size"/> object representing the size of the string.</returns>
    public Size GetSize(string str, int maxWidth);

    /// <summary>
    /// Returns the default char font. This font will render a character as it is.
    /// </summary>
    public static IFont Default { get; } = new DefaultFont();
}