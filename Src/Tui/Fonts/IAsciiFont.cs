// NeoKolors
// Copyright (c) 2026 KryKom

using System.Drawing;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Fonts;

public interface IAsciiFont {

    public string Name { get; }

    /// <summary>
    ///     Renders a string onto a canvas using the specified drawer, position, and depth.
    /// </summary>
    /// <param name="str">The textual content to be drawn.</param>
    /// <param name="drawer">The character drawer responsible for rendering the string.</param>
    /// <param name="position">
    ///     The canvas position where the string should be drawn. Defaults to
    ///     the origin if not specified.
    /// </param>
    /// <param name="z">The depth order of the string. Defaults to 0 if not specified.</param>
    public void DrawString(string str, ICharDrawer drawer, Point2D position = default, int z = 0);

    /// <summary>
    ///     Renders a string onto a canvas using the specified drawer, maximum width, position, and
    ///     depth.
    /// </summary>
    /// <param name="str">The textual content to be rendered.</param>
    /// <param name="drawer">The character drawer responsible for rendering the string.</param>
    /// <param name="maxWidth">
    ///     The maximum width, in characters, for the rendered string. Text
    ///     exceeding this width may be clipped or wrapped, depending on implementation.
    /// </param>
    /// <param name="position">
    ///     The canvas position where the string should be drawn. Defaults to
    ///     the origin if not specified.
    /// </param>
    /// <param name="z">The depth order of the string. Defaults to 0 if not specified.</param>
    public void DrawString(string str, ICharDrawer drawer, int maxWidth, Point2D position = default, int z = 0);

    /// <summary>
    ///     Draws a string using the specified character drawer, font style, and alignment options
    ///     within defined bounds.
    /// </summary>
    /// <param name="str">The string to be drawn.</param>
    /// <param name="drawer">The character drawer responsible for rendering the string.</param>
    /// <param name="bounds">The rectangular boundary within which the string is drawn.</param>
    /// <param name="style">The graphical style applied to the string.</param>
    /// <param name="horizontalAlign">
    ///     The horizontal alignment of the string within the bounds.
    ///     Defaults to left alignment.
    /// </param>
    /// <param name="verticalAlign">
    ///     The vertical alignment of the string within the bounds.
    ///     Defaults to the top alignment.
    /// </param>
    /// <param name="overflow">
    ///     Indicates whether the string is allowed to overflow beyond the
    ///     bounds. Defaults to false.
    /// </param>
    /// <param name="z">
    ///     The depth order of the string, controlling its rendering stack position.
    ///     Defaults to 0.
    /// </param>
    public void DrawString(
        string          str,
        ICharDrawer     drawer,
        Rect2D          bounds,
        NKStyle         style,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign   verticalAlign   = VerticalAlign.TOP,
        bool            overflow        = false,
        int             z               = 0
    );

    /// <summary>
    /// Draws a string onto a canvas using the specified character drawer, position, and depth.
    /// </summary>
    /// <param name="str">
    /// The string to be drawn. It can either be a plain or styled ANSI string depending on the method overload used.
    /// </param>
    /// <param name="drawer">
    /// The character drawer responsible for rendering the string onto the canvas.
    /// </param>
    /// <param name="bounds">The rectangular boundary within which the string is drawn.</param>
    /// <param name="options">The rendering options for the string.</param>
    /// <param name="z">
    /// The depth order for rendering the string. Defaults to 0 if not provided.
    /// </param>
    public void DrawString(
        string               str,
        ICharDrawer          drawer,
        Rect2D               bounds,
        TextRenderingOptions options,
        int                  z = 0
    );

    /// <summary>
    /// Draws a string onto a canvas using the specified character drawer and text rendering options,
    /// using the drawer's bounds as the rendering boundaries.
    /// </summary>
    /// <param name="str">The string to be drawn.</param>
    /// <param name="drawer">The character drawer responsible for rendering the string.</param>
    /// <param name="options">The rendering options for the string.</param>
    /// <param name="z">The depth order for rendering the string. Defaults to 0 if not provided.</param>
    public void DrawString(
        string               str,
        ICharDrawer          drawer,
        TextRenderingOptions options,
        int                  z = 0
    ) => DrawString(str, drawer, new Rect2D(Point2D.Zero, drawer.Size), options, z);

    /// <summary>
    ///     Renders a string onto a canvas using the specified drawer, position, and depth.
    /// </summary>
    /// <param name="str">The textual content to be drawn.</param>
    /// <param name="drawer">The character drawer responsible for rendering the string.</param>
    /// <param name="position">
    ///     The canvas position where the string should be drawn. Defaults to
    ///     the origin if not specified.
    /// </param>
    /// <param name="z">The depth order of the string. Defaults to 0 if not specified.</param>
    public void DrawString(
        AnsiString  str,
        ICharDrawer drawer,
        Point2D     position = default,
        int         z        = 0
    ) {
        DrawString(str, drawer, drawer.Width, position, z);
    }

    /// <summary>
    ///     Renders a string onto a canvas using the specified drawer, maximum width, position, and
    ///     depth.
    /// </summary>
    /// <param name="str">The textual content to be rendered.</param>
    /// <param name="drawer">The character drawer responsible for rendering the string.</param>
    /// <param name="maxWidth">
    ///     The maximum width, in characters, for the rendered string. Text
    ///     exceeding this width may be clipped or wrapped, depending on implementation.
    /// </param>
    /// <param name="position">
    ///     The canvas position where the string should be drawn. Defaults to
    ///     the origin if not specified.
    /// </param>
    /// <param name="z">The depth order of the string. Defaults to 0 if not specified.</param>
    public void DrawString(
        AnsiString  str,
        ICharDrawer drawer,
        int         maxWidth,
        Point2D     position = default,
        int         z        = 0
    ) {
        DrawString(
            str,
            drawer,
            new Rect2D(position, new Point2D(maxWidth, drawer.Height)),
            HorizontalAlign.LEFT,
            VerticalAlign.TOP,
            false,
            z
        );
    }

    /// <summary>
    ///     Draws a string using the specified character drawer, font style, and alignment options
    ///     within defined bounds.
    /// </summary>
    /// <param name="str">The string to be drawn.</param>
    /// <param name="drawer">The character drawer responsible for rendering the string.</param>
    /// <param name="bounds">The rectangular boundary within which the string is drawn.</param>
    /// <param name="horizontalAlign">
    ///     The horizontal alignment of the string within the bounds.
    ///     Defaults to left alignment.
    /// </param>
    /// <param name="verticalAlign">
    ///     The vertical alignment of the string within the bounds.
    ///     Defaults to the top alignment.
    /// </param>
    /// <param name="overflow">
    ///     Indicates whether the string is allowed to overflow beyond the
    ///     bounds. Defaults to false.
    /// </param>
    /// <param name="z">
    ///     The depth order of the string, controlling its rendering stack position.
    ///     Defaults to 0.
    /// </param>
    public void DrawString(
        AnsiString      str,
        ICharDrawer     drawer,
        Rect2D          bounds,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign   verticalAlign   = VerticalAlign.TOP,
        bool            overflow        = false,
        int             z               = 0
    );

    /// <summary>
    /// Draws a string onto a canvas using the specified character drawer, position, and depth.
    /// </summary>
    /// <param name="str">
    /// The string to be drawn. It can either be a plain or styled ANSI string depending on the method overload used.
    /// </param>
    /// <param name="drawer">
    /// The character drawer responsible for rendering the string onto the canvas.
    /// </param>
    /// <param name="bounds">The rectangular boundary within which the string is drawn.</param>
    /// <param name="options">The rendering options for the string.</param>
    /// <param name="z">
    /// The depth order for rendering the string. Defaults to 0 if not provided.
    /// </param>
    public void DrawString(
        AnsiString           str,
        ICharDrawer          drawer,
        Rect2D               bounds,
        TextRenderingOptions options,
        int                  z = 0
    );

    /// <summary>
    /// Draws a styled ANSI string onto a canvas using the specified character drawer and text rendering options,
    /// using the drawer's bounds as the rendering boundaries.
    /// </summary>
    /// <param name="str">The ANSI string to be drawn.</param>
    /// <param name="drawer">The character drawer responsible for rendering the string.</param>
    /// <param name="options">The rendering options for the string.</param>
    /// <param name="z">The depth order for rendering the string. Defaults to 0 if not provided.</param>
    public void DrawString(
        AnsiString           str,
        ICharDrawer          drawer,
        TextRenderingOptions options,
        int                  z = 0
    ) => DrawString(str, drawer, new Rect2D(Point2D.Zero, drawer.Size), options, z);

    /// <summary>
    ///     Calculates the size of the string if the longest word in the string is the maximum
    ///     width of the canvas.
    /// </summary>
    /// <param name="str">The string for which the minimum size is to be calculated.</param>
    /// <returns>
    ///     A <see cref="Size" /> object representing the minimum width and height necessary
    ///     to render the string.
    /// </returns>
    public Size2D GetMinSize(string str);

    /// <summary>
    ///     Calculates the size required to render the provided string using the current font.
    /// </summary>
    /// <param name="str">The string for which the size is to be calculated.</param>
    /// <returns>
    ///     A <see cref="Size" /> object representing the width and height necessary to render
    ///     the string.
    /// </returns>
    public Size2D GetSize(string str);

    /// <summary>
    ///     Calculates the size of the string when rendered with a maximum width.
    /// </summary>
    /// <param name="str">The string to measure.</param>
    /// <param name="maxWidth">The maximum width allowed for the string.</param>
    /// <returns>A <see cref="Size" /> object representing the size of the string.</returns>
    public Size2D GetSize(string str, int maxWidth);

    /// <summary>
    ///     Calculates the size of the string if the longest word in the string is the maximum
    ///     width of the canvas.
    /// </summary>
    /// <param name="str">The string for which the minimum size is to be calculated.</param>
    /// <returns>
    ///     A <see cref="Size" /> object representing the minimum width and height necessary
    ///     to render the string.
    /// </returns>
    public Size2D GetMinSize(AnsiString str);

    /// <summary>
    ///     Calculates the size required to render the provided string using the current font.
    /// </summary>
    /// <param name="str">The string for which the size is to be calculated.</param>
    /// <returns>
    ///     A <see cref="Size" /> object representing the width and height necessary to render
    ///     the string.
    /// </returns>
    public Size2D GetSize(AnsiString str);

    /// <summary>
    ///     Calculates the size of the string when rendered with a maximum width.
    /// </summary>
    /// <param name="str">The string to measure.</param>
    /// <param name="maxWidth">The maximum width allowed for the string.</param>
    /// <returns>A <see cref="Size" /> object representing the size of the string.</returns>
    public Size2D GetSize(AnsiString str, int maxWidth);

    /// <summary>
    ///     Calculates the size required to render the provided string using the given text rendering options.
    /// </summary>
    /// <param name="str">The string to measure.</param>
    /// <param name="options">The text rendering options.</param>
    /// <param name="maxWidth">Optional maximum width constraint.</param>
    /// <returns>A <see cref="Size2D"/> object representing the size of the string.</returns>
    public Size2D GetSize(AnsiString str, TextRenderingOptions options, int? maxWidth = null) =>
        maxWidth.HasValue ? GetSize(str, maxWidth.Value) : GetSize(str);

    /// <summary>
    ///     Returns the default char font. This font will render a character as it is.
    /// </summary>
    public static IAsciiFont Default { get; } = new DefaultFont();
}