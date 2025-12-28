//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Runtime.InteropServices;

namespace NeoKolors.Tui.Styles.Values;

[StructLayout(LayoutKind.Sequential, Size = 6 * sizeof(char) + 8 * sizeof(long))]
public struct BorderStyle {
    
    public char Vertical    { get; set; }
    public char Horizontal  { get; set; }
    public char TopRight    { get; set; }
    public char TopLeft     { get; set; }
    public char BottomRight { get; set; }
    public char BottomLeft  { get; set; }
    
    public NKStyle StyleTop         { get; set; }
    public NKStyle StyleBottom      { get; set; }
    public NKStyle StyleLeft        { get; set; }
    public NKStyle StyleRight       { get; set; }
    public NKStyle StyleTopLeft     { get; set; }
    public NKStyle StyleTopRight    { get; set; }
    public NKStyle StyleBottomLeft  { get; set; }
    public NKStyle StyleBottomRight { get; set; }
    
    public bool IsBorderless { get; }

    public BorderStyle(
        char vertical,
        char horizontal,
        char topLeft,
        char topRight,
        char bottomLeft,
        char bottomRight,
        NKStyle? styleTop         = null, 
        NKStyle? styleBottom      = null, 
        NKStyle? styleLeft        = null, 
        NKStyle? styleRight       = null, 
        NKStyle? styleTopLeft     = null, 
        NKStyle? styleTopRight    = null, 
        NKStyle? styleBottomLeft  = null, 
        NKStyle? styleBottomRight = null) 
    {
        Vertical         = vertical;
        Horizontal       = horizontal;
        TopRight         = topRight;
        TopLeft          = topLeft;
        BottomRight      = bottomRight;
        BottomLeft       = bottomLeft;
        StyleTop         = styleTop         ?? new NKStyle(NKColor.Default, NKColor.Inherit, TextStyles.NONE);
        StyleBottom      = styleBottom      ?? new NKStyle(NKColor.Default, NKColor.Inherit, TextStyles.NONE);
        StyleLeft        = styleLeft        ?? new NKStyle(NKColor.Default, NKColor.Inherit, TextStyles.NONE);
        StyleRight       = styleRight       ?? new NKStyle(NKColor.Default, NKColor.Inherit, TextStyles.NONE);
        StyleTopLeft     = styleTopLeft     ?? new NKStyle(NKColor.Default, NKColor.Inherit, TextStyles.NONE);
        StyleTopRight    = styleTopRight    ?? new NKStyle(NKColor.Default, NKColor.Inherit, TextStyles.NONE);
        StyleBottomLeft  = styleBottomLeft  ?? new NKStyle(NKColor.Default, NKColor.Inherit, TextStyles.NONE);
        StyleBottomRight = styleBottomRight ?? new NKStyle(NKColor.Default, NKColor.Inherit, TextStyles.NONE);
        IsBorderless     = false;
    }
    
    public BorderStyle(
        char vertical,
        char horizontal,
        char topLeft,
        char topRight,
        char bottomLeft,
        char bottomRight,
        NKColor? colorTop         = null, 
        NKColor? colorBottom      = null, 
        NKColor? colorLeft        = null, 
        NKColor? colorRight       = null, 
        NKColor? colorTopLeft     = null, 
        NKColor? colorTopRight    = null, 
        NKColor? colorBottomLeft  = null, 
        NKColor? colorBottomRight = null,
        NKColor? background       = null) 
    {
        Vertical         = vertical;
        Horizontal       = horizontal;
        TopRight         = topRight;
        TopLeft          = topLeft;
        BottomRight      = bottomRight;
        BottomLeft       = bottomLeft;
        StyleTop         = new NKStyle(colorTop         ?? NKColor.Default, background ?? NKColor.Inherit, TextStyles.NONE);
        StyleBottom      = new NKStyle(colorBottom      ?? NKColor.Default, background ?? NKColor.Inherit, TextStyles.NONE);
        StyleLeft        = new NKStyle(colorLeft        ?? NKColor.Default, background ?? NKColor.Inherit, TextStyles.NONE);
        StyleRight       = new NKStyle(colorRight       ?? NKColor.Default, background ?? NKColor.Inherit, TextStyles.NONE);
        StyleTopLeft     = new NKStyle(colorTopLeft     ?? NKColor.Default, background ?? NKColor.Inherit, TextStyles.NONE);
        StyleTopRight    = new NKStyle(colorTopRight    ?? NKColor.Default, background ?? NKColor.Inherit, TextStyles.NONE);
        StyleBottomLeft  = new NKStyle(colorBottomLeft  ?? NKColor.Default, background ?? NKColor.Inherit, TextStyles.NONE);
        StyleBottomRight = new NKStyle(colorBottomRight ?? NKColor.Default, background ?? NKColor.Inherit, TextStyles.NONE);
        IsBorderless     = false;
    }

    public BorderStyle(
        char vertical,
        char horizontal,
        char topLeft,
        char topRight,
        char bottomLeft,
        char bottomRight,
        NKColor? textColor = null,
        NKColor? backgroundColor = null) 
    {
        Vertical         = vertical;
        Horizontal       = horizontal;
        TopRight         = topRight;
        TopLeft          = topLeft;
        BottomRight      = bottomRight;
        BottomLeft       = bottomLeft;
        StyleTop         = new NKStyle(textColor ?? NKColor.Default, backgroundColor ?? NKColor.Inherit, TextStyles.NONE);
        StyleBottom      = new NKStyle(textColor ?? NKColor.Default, backgroundColor ?? NKColor.Inherit, TextStyles.NONE);
        StyleLeft        = new NKStyle(textColor ?? NKColor.Default, backgroundColor ?? NKColor.Inherit, TextStyles.NONE);
        StyleRight       = new NKStyle(textColor ?? NKColor.Default, backgroundColor ?? NKColor.Inherit, TextStyles.NONE);
        StyleTopLeft     = new NKStyle(textColor ?? NKColor.Default, backgroundColor ?? NKColor.Inherit, TextStyles.NONE);
        StyleTopRight    = new NKStyle(textColor ?? NKColor.Default, backgroundColor ?? NKColor.Inherit, TextStyles.NONE);
        StyleBottomLeft  = new NKStyle(textColor ?? NKColor.Default, backgroundColor ?? NKColor.Inherit, TextStyles.NONE);
        StyleBottomRight = new NKStyle(textColor ?? NKColor.Default, backgroundColor ?? NKColor.Inherit, TextStyles.NONE);
        IsBorderless     = false;
    }

    private BorderStyle(bool isBorderless) => IsBorderless = isBorderless;

    public BorderStyle() => GetAscii();

    /// <summary>
    /// Creates a BorderStyle object with ASCII-style borders.
    /// </summary>
    /// <param name="textColor">The text color for the border. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="backgroundColor">
    /// The background color for the border. Defaults to <see cref="NKColor.Inherit"/>.
    /// </param>
    /// <returns>A BorderStyle instance with ASCII characters.</returns>
    public static BorderStyle GetAscii(NKColor? textColor = null, NKColor? backgroundColor = null) =>
        new('|', '-', '+', '+', '+', '+', textColor, backgroundColor);

    /// <summary>
    /// Creates a BorderStyle object with thin line borders.
    /// </summary>
    /// <param name="textColor">The text color for the border. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="backgroundColor">
    /// The background color for the border. Defaults to <see cref="NKColor.Inherit"/>.
    /// </param>
    /// <returns>A BorderStyle instance with normal characters.</returns>
    public static BorderStyle GetNormal(NKColor? textColor = null, NKColor? backgroundColor = null) =>
        new('│', '─', '┌', '┐', '└', '┘', textColor, backgroundColor);

    /// <summary>
    /// Creates a BorderStyle object with thin line rounded borders.
    /// </summary>
    /// <param name="textColor">The text color for the border. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="backgroundColor">
    /// The background color for the border. Defaults to <see cref="NKColor.Inherit"/>.
    /// </param>
    /// <returns>A BorderStyle instance with rounded characters.</returns>
    public static BorderStyle GetRounded(NKColor? textColor = null, NKColor? backgroundColor = null) =>
        new('│', '─', '╭', '╮', '╰', '╯', textColor, backgroundColor);

    /// <summary>
    /// Creates a BorderStyle object with thick line borders.
    /// </summary>
    /// <param name="textColor">The text color for the border. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="backgroundColor">
    /// The background color for the border. Defaults to <see cref="NKColor.Inherit"/>.
    /// </param>
    /// <returns>A BorderStyle instance with thick characters.</returns>
    public static BorderStyle GetThick(NKColor? textColor = null, NKColor? backgroundColor = null) =>
        new('┃', '━', '┏', '┓', '┗', '┛', textColor, backgroundColor);

    /// <summary>
    /// Creates a BorderStyle object with double line borders.
    /// </summary>
    /// <param name="textColor">The text color for the border. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="backgroundColor">
    /// The background color for the border. Defaults to <see cref="NKColor.Inherit"/>.
    /// </param>
    /// <returns>A BorderStyle instance with double-line characters.</returns>
    public static BorderStyle GetDouble(NKColor? textColor = null, NKColor? backgroundColor = null) =>
        new('║', '═', '╔', '╗', '╚', '╝', textColor, backgroundColor);

    /// <summary>
    /// Creates a BorderStyle object with a solid "borderless" style, where all border characters are spaces.
    /// </summary>
    /// <param name="backgroundColor">
    /// The background color to apply to the border. Defaults to <see cref="NKColor.Default"/> if not provided.
    /// </param>
    /// <returns>
    /// A BorderStyle instance with all border characters replaced by spaces and the specified background color.
    /// </returns>
    public static BorderStyle GetSolid(NKColor? backgroundColor) =>
        new(' ', ' ', ' ', ' ', ' ', ' ', backgroundColor: backgroundColor ?? NKColor.Default);

    /// <summary>
    /// Creates a BorderStyle object with no visual borders, representing a borderless design.
    /// </summary>
    /// <returns>A BorderStyle instance that is configured to be borderless.</returns>
    public static BorderStyle GetBorderless() =>
        new(true);

    /// <summary>
    /// Creates a BorderStyle object with an inset appearance.
    /// </summary>
    /// <param name="shadow">The shadow color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="highlight">The highlight color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="background">The background color for the border. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <returns>A BorderStyle instance with outset-style borders.</returns>
    public static BorderStyle Inset(NKColor? shadow = null, NKColor? highlight = null, NKColor? background = null) =>
        new('│', '─', '┌', '┐', '└', '┘',
            shadow, highlight, shadow, highlight, shadow, highlight, shadow, highlight, background);
    
    /// <summary>
    /// Creates a BorderStyle object with an outset appearance.
    /// </summary>
    /// <param name="shadow">The shadow color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="highlight">The highlight color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="background">The background color for the border. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <returns>A BorderStyle instance with outset-style borders.</returns>
    public static BorderStyle Outset(NKColor? shadow = null, NKColor? highlight = null, NKColor? background = null) =>
        new('│', '─', '┌', '┐', '└', '┘',
            highlight, shadow, highlight, shadow, highlight, shadow, highlight, shadow, background);
    
    

    /// <summary>
    /// Creates a BorderStyle object with an inset appearance.
    /// </summary>
    /// <param name="shadow">The shadow color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="highlight">The highlight color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="background">The background color for the border. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <returns>A BorderStyle instance with outset-style borders.</returns>
    public static BorderStyle InsetThick(NKColor? shadow = null, NKColor? highlight = null, NKColor? background = null) =>
        new('┃', '━', '┏', '┓', '┗', '┛',
            shadow, highlight, shadow, highlight, shadow, highlight, shadow, highlight, background);
    
    /// <summary>
    /// Creates a BorderStyle object with an outset appearance.
    /// </summary>
    /// <param name="shadow">The shadow color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="highlight">The highlight color applied to the border elements. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <param name="background">The background color for the border. Defaults to <see cref="NKColor.Default"/>.</param>
    /// <returns>A BorderStyle instance with outset-style borders.</returns>
    public static BorderStyle OutsetThick(NKColor? shadow = null, NKColor? highlight = null, NKColor? background = null) =>
        new('┃', '━', '┏', '┓', '┗', '┛',
            highlight, shadow, highlight, shadow, highlight, shadow, highlight, shadow, background);
    
    public static BorderStyle Borderless => new(true);
}