//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;

namespace NeoKolors.Tui.Styles;

public struct BorderStyle {
    
    public char Vertical { get; set; }
    public char Horizontal { get; set; }
    public char TopRight { get; set; }
    public char TopLeft { get; set; }
    public char BottomRight { get; set; }
    public char BottomLeft { get; set; }
    
    public NKColor FColor { get; set; }
    public NKColor BColor { get; set; }

    public bool IsBorderless { get; }

    public BorderStyle(char vertical,
        char horizontal,
        char topLeft,
        char topRight,
        char bottomLeft,
        char bottomRight,
        NKColor? textColor = null,
        NKColor? backgroundColor = null) 
    {
        Vertical = vertical;
        Horizontal = horizontal;
        TopRight = topRight;
        TopLeft = topLeft;
        BottomRight = bottomRight;
        BottomLeft = bottomLeft;
        FColor = textColor ?? NKColor.Default;
        BColor = backgroundColor ?? NKColor.Inherit;
        IsBorderless = false;
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
    
    public static BorderStyle Borderless => new(true);
}