// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Common;

namespace NeoKolors.Tui.Core;

public record NKBorderStyle : IBorderStyle {

    public AnsiChar TopLeft     { get; init; }
    public AnsiChar Top         { get; init; }
    public AnsiChar TopRight    { get; init; }
    public AnsiChar Right       { get; init; }
    public AnsiChar BottomRight { get; init; }
    public AnsiChar Bottom      { get; init; }
    public AnsiChar BottomLeft  { get; init; }
    public AnsiChar Left        { get; init; }

    public bool IsBorderless { get; }

    public NKBorderStyle(
        AnsiChar topLeft,
        AnsiChar top,
        AnsiChar topRight,
        AnsiChar right,
        AnsiChar bottomRight,
        AnsiChar bottom,
        AnsiChar bottomLeft,
        AnsiChar left
    ) {
        IsBorderless = false;
        TopLeft      = topLeft;
        Top          = top;
        TopRight     = topRight;
        Right        = right;
        BottomRight  = bottomRight;
        Bottom       = bottom;
        BottomLeft   = bottomLeft;
        Left         = left;
    }

    public NKBorderStyle(
        char    topLeft,
        char    top,
        char    topRight,
        char    right,
        char    bottomRight,
        char    bottom,
        char    bottomLeft,
        char    left,
        NKStyle topLeftStyle     = default,
        NKStyle topStyle         = default,
        NKStyle topRightStyle    = default,
        NKStyle rightStyle       = default,
        NKStyle bottomRightStyle = default,
        NKStyle bottomStyle      = default,
        NKStyle bottomLeftStyle  = default,
        NKStyle leftStyle        = default
    ) {
        IsBorderless = false;
        TopLeft      = new AnsiChar(topLeft,     topLeftStyle);
        Top          = new AnsiChar(top,         topStyle);
        TopRight     = new AnsiChar(topRight,    topRightStyle);
        Right        = new AnsiChar(right,       rightStyle);
        BottomRight  = new AnsiChar(bottomRight, bottomRightStyle);
        Bottom       = new AnsiChar(bottom,      bottomStyle);
        BottomLeft   = new AnsiChar(bottomLeft,  bottomLeftStyle);
        Left         = new AnsiChar(left,        leftStyle);
    }

    public NKBorderStyle(
        char    topLeft,
        char    top,
        char    topRight,
        char    right,
        char    bottomRight,
        char    bottom,
        char    bottomLeft,
        char    left,
        NKStyle style = default
    ) {
        IsBorderless = false;
        TopLeft      = new AnsiChar(topLeft,     style);
        Top          = new AnsiChar(top,         style);
        TopRight     = new AnsiChar(topRight,    style);
        Right        = new AnsiChar(right,       style);
        BottomRight  = new AnsiChar(bottomRight, style);
        Bottom       = new AnsiChar(bottom,      style);
        BottomLeft   = new AnsiChar(bottomLeft,  style);
        Left         = new AnsiChar(left,        style);
    }

    public NKBorderStyle(
        char    vertical,
        char    horizontal,
        char    topLeft,
        char    topRight,
        char    bottomLeft,
        char    bottomRight,
        NKStyle verticalStyle    = default,
        NKStyle horizontalStyle  = default,
        NKStyle topLeftStyle     = default,
        NKStyle topRightStyle    = default,
        NKStyle bottomLeftStyle  = default,
        NKStyle bottomRightStyle = default
    ) {
        IsBorderless = false;
        TopLeft      = new AnsiChar(topLeft,     topLeftStyle);
        Top          = new AnsiChar(horizontal,  horizontalStyle);
        TopRight     = new AnsiChar(topRight,    topRightStyle);
        Right        = new AnsiChar(vertical,    verticalStyle);
        BottomRight  = new AnsiChar(bottomRight, bottomRightStyle);
        Bottom       = new AnsiChar(horizontal,  horizontalStyle);
        BottomLeft   = new AnsiChar(bottomLeft,  bottomLeftStyle);
        Left         = new AnsiChar(vertical,    verticalStyle);
    }

    public NKBorderStyle(
        char    vertical,
        char    horizontal,
        char    topLeft,
        char    topRight,
        char    bottomLeft,
        char    bottomRight,
        NKStyle style = default
    ) {
        IsBorderless = false;
        TopLeft      = new AnsiChar(topLeft,     style);
        Top          = new AnsiChar(horizontal,  style);
        TopRight     = new AnsiChar(topRight,    style);
        Right        = new AnsiChar(vertical,    style);
        BottomRight  = new AnsiChar(bottomRight, style);
        Bottom       = new AnsiChar(horizontal,  style);
        BottomLeft   = new AnsiChar(bottomLeft,  style);
        Left         = new AnsiChar(vertical,    style);
    }

    private NKBorderStyle(bool isBorderless) => IsBorderless = isBorderless;

    public void Draw(Rect2D rect, in ICharDrawer canvas, int z) {
        if (IsBorderless || rect.SizeX <= 0 || rect.SizeY <= 0) return;

        int lx = rect.LowerX;
        int ly = rect.LowerY;
        int rx = rect.HigherX - 1;
        int by = rect.HigherY - 1;

        if (rect is { SizeX: 1, SizeY: 1 }) {
            canvas.Draw(TopLeft, lx, ly, z);
            return;
        }

        canvas.Draw(TopLeft,     lx, ly, z);
        canvas.Draw(TopRight,    rx, ly, z);
        canvas.Draw(BottomLeft,  lx, by, z);
        canvas.Draw(BottomRight, rx, by, z);

        if (rect.SizeX > 2) {
            canvas.DrawHorizontalLine(Top,    lx + 1, ly, rect.SizeX - 2, z);
            canvas.DrawHorizontalLine(Bottom, lx + 1, by, rect.SizeX - 2, z);
        }

        if (rect.SizeY > 2) {
            canvas.DrawVerticalLine(Left,  lx, ly + 1, rect.SizeY - 2, z);
            canvas.DrawVerticalLine(Right, rx, ly + 1, rect.SizeY - 2, z);
        }
    }
    
    public Rect2D GetContentBox(Size2D size) {
        return IsBorderless 
            ? new Rect2D(Point2D.Zero, size) 
            : new Rect2D(new Point2D(1, 1), new Size2D(Math.Max(0, size.X - 2), Math.Max(0, size.Y - 2)));
    }

    public Size2D GetBorderBox(Size2D size) {
        return IsBorderless 
            ? size 
            : new Size2D(size.X + 2, size.Y + 2);
    }

    public NKBorderStyle() : this('|', '-', '+', '+', '+', '+', default) { }

    /// <summary>
    /// Creates a NKBorderStyle object with ASCII-style borders.
    /// </summary>
    /// The background color for the border. Defaults to <see cref="NKColor.Inherit"/>.
    /// <returns>A NKBorderStyle instance with ASCII characters.</returns>
    public static NKBorderStyle GetAscii(NKStyle style = default) {
        return new NKBorderStyle('|', '-', '+', '+', '+', '+', style);
    }

    /// <summary>
    /// Creates a NKBorderStyle object with thin line borders.
    /// </summary>
    /// The background color for the border. Defaults to <see cref="NKColor.Inherit"/>.
    /// <returns>A NKBorderStyle instance with normal characters.</returns>
    public static NKBorderStyle GetNormal(NKStyle style = default) {
        return new NKBorderStyle('│', '─', '┌', '┐', '└', '┘', style);
    }

    /// <summary>
    /// Creates a NKBorderStyle object with thin line rounded borders.
    /// </summary>
    /// The background color for the border. Defaults to <see cref="NKColor.Inherit"/>.
    /// <returns>A NKBorderStyle instance with rounded characters.</returns>
    public static NKBorderStyle GetRounded(NKStyle style = default) {
        return new NKBorderStyle('│', '─', '╭', '╮', '╰', '╯', style);
    }

    /// <summary>
    /// Creates a NKBorderStyle object with thick line borders.
    /// </summary>
    /// <returns>A NKBorderStyle instance with thick characters.</returns>
    public static NKBorderStyle GetThick(NKStyle style = default) {
        return new NKBorderStyle('┃', '━', '┏', '┓', '┗', '┛', style);
    }

    /// <summary>
    /// Creates a NKBorderStyle object with double line borders.
    /// </summary>
    /// <returns>A NKBorderStyle instance with double-line characters.</returns>
    public static NKBorderStyle GetDouble(NKStyle style = default) {
        return new NKBorderStyle('║', '═', '╔', '╗', '╚', '╝', style);
    }

    /// <summary>
    /// Creates a NKBorderStyle object with a solid "borderless" style, where all border characters are spaces.
    /// </summary>
    /// <returns>
    /// A NKBorderStyle instance with all border characters replaced by spaces and the specified background color.
    /// </returns>
    public static NKBorderStyle GetSolid(NKColor color) {
        return new NKBorderStyle(' ', ' ', ' ', ' ', ' ', ' ', new NKStyle(backgroundColor: color));
    }

    /// <summary>
    /// Creates a NKBorderStyle object with no visual borders, representing a borderless design.
    /// </summary>
    /// <returns>A NKBorderStyle instance that is configured to be borderless.</returns>
    public static NKBorderStyle GetBorderless() {
        return new NKBorderStyle(true);
    }

    /// <summary>
    /// Creates a NKBorderStyle object with an inset appearance.
    /// </summary>
    /// <param name="shadow">The shadow style applied to the border elements.</param>
    /// <param name="highlight">The highlight style applied to the border elements.</param>
    /// <returns>A NKBorderStyle instance with inset-style borders.</returns>
    public static NKBorderStyle GetInset(NKStyle shadow = default, NKStyle highlight = default) {
        return new NKBorderStyle(
            '┌',
            '─',
            '┐',
            '│',
            '┘',
            '─',
            '└',
            '│',
            shadow,
            shadow,
            highlight,
            highlight,
            highlight,
            highlight,
            shadow,
            shadow
        );
    }

    /// <summary>
    /// Creates a NKBorderStyle object with an inset appearance.
    /// </summary>
    /// <param name="shadow">The shadow color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="highlight">The highlight color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="background">The background color for the border. Defaults to <see cref="NKColor.Inherit"/>.</param>
    /// <returns>A NKBorderStyle instance with inset-style borders.</returns>
    public static NKBorderStyle GetInset(
        NKColor? shadow,
        NKColor? highlight  = null,
        NKColor? background = null
    ) => GetInset(
        new NKStyle(shadow    ?? NKColor.Default, background ?? NKColor.Inherit),
        new NKStyle(highlight ?? NKColor.Default, background ?? NKColor.Inherit)
    );

    /// <summary>
    /// Creates a NKBorderStyle object with an outset appearance.
    /// </summary>
    /// <param name="shadow">The shadow style applied to the border elements.</param>
    /// <param name="highlight">The highlight style applied to the border elements.</param>
    /// <returns>A NKBorderStyle instance with outset-style borders.</returns>
    public static NKBorderStyle GetOutset(NKStyle shadow = default, NKStyle highlight = default) {
        return new NKBorderStyle(
            '┌',
            '─',
            '┐',
            '│',
            '┘',
            '─',
            '└',
            '│',
            highlight,
            highlight,
            shadow,
            shadow,
            shadow,
            shadow,
            highlight,
            highlight
        );
    }

    /// <summary>
    /// Creates a NKBorderStyle object with an outset appearance.
    /// </summary>
    /// <param name="shadow">The shadow color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="highlight">The highlight color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="background">The background color for the border. Defaults to <see cref="NKColor.Inherit"/>.</param>
    /// <returns>A NKBorderStyle instance with outset-style borders.</returns>
    public static NKBorderStyle GetOutset(
        NKColor? shadow,
        NKColor? highlight  = null,
        NKColor? background = null
    ) => GetOutset(
        new NKStyle(shadow    ?? NKColor.Default, background ?? NKColor.Inherit),
        new NKStyle(highlight ?? NKColor.Default, background ?? NKColor.Inherit)
    );

    /// <summary>
    /// Creates a NKBorderStyle object with a thick inset appearance.
    /// </summary>
    /// <param name="shadow">The shadow style applied to the border elements.</param>
    /// <param name="highlight">The highlight style applied to the border elements.</param>
    /// <returns>A NKBorderStyle instance with inset-style borders.</returns>
    public static NKBorderStyle GetInsetThick(NKStyle shadow = default, NKStyle highlight = default) {
        return new NKBorderStyle(
            '┏',
            '━',
            '┓',
            '┃',
            '┛',
            '━',
            '┗',
            '┃',
            shadow,
            shadow,
            highlight,
            highlight,
            highlight,
            highlight,
            shadow,
            shadow
        );
    }

    /// <summary>
    /// Creates a NKBorderStyle object with a thick inset appearance.
    /// </summary>
    /// <param name="shadow">The shadow color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="highlight">The highlight color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="background">The background color for the border. Defaults to <see cref="NKColor.Inherit"/>.</param>
    /// <returns>A NKBorderStyle instance with inset-style borders.</returns>
    public static NKBorderStyle GetInsetThick(
        NKColor? shadow,
        NKColor? highlight  = null,
        NKColor? background = null
    ) => GetInsetThick(
        new NKStyle(shadow    ?? NKColor.Default, background ?? NKColor.Inherit),
        new NKStyle(highlight ?? NKColor.Default, background ?? NKColor.Inherit)
    );

    /// <summary>
    /// Creates a NKBorderStyle object with a thick outset appearance.
    /// </summary>
    /// <param name="shadow">The shadow style applied to the border elements.</param>
    /// <param name="highlight">The highlight style applied to the border elements.</param>
    /// <returns>A NKBorderStyle instance with outset-style borders.</returns>
    public static NKBorderStyle GetOutsetThick(NKStyle shadow = default, NKStyle highlight = default) {
        return new NKBorderStyle(
            '┏',
            '━',
            '┓',
            '┃',
            '┛',
            '━',
            '┗',
            '┃',
            highlight,
            highlight,
            shadow,
            shadow,
            shadow,
            shadow,
            highlight,
            highlight
        );
    }

    /// <summary>
    /// Creates a NKBorderStyle object with a thick outset appearance.
    /// </summary>
    /// <param name="shadow">The shadow color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="highlight">The highlight color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="background">The background color for the border. Defaults to <see cref="NKColor.Inherit"/>.</param>
    /// <returns>A NKBorderStyle instance with outset-style borders.</returns>
    public static NKBorderStyle GetOutsetThick(
        NKColor? shadow,
        NKColor? highlight  = null,
        NKColor? background = null
    ) {
        return GetOutsetThick(
            new NKStyle(shadow    ?? NKColor.Default, background ?? NKColor.Inherit),
            new NKStyle(highlight ?? NKColor.Default, background ?? NKColor.Inherit)
        );
    }

    public static NKBorderStyle Borderless => new(true);
}