//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;

namespace NeoKolors.Tui.Style;

public struct BorderStyle {
    public char Vertical { get; }
    public char Horizontal { get; }
    public char TopRight { get; }
    public char TopLeft { get; }
    public char BottomRight { get; }
    public char BottomLeft { get; }
    
    public NKColor FColor { get; } = null!;

    public NKColor BColor { get; } = null!;

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
        BColor = backgroundColor ?? NKColor.Default;
    }

    public BorderStyle() => GetAscii();
    
    public static BorderStyle GetAscii(NKColor? textColor = null, NKColor? backgroundColor = null) =>
        new('|', '-', '+', '+', '+', '+', textColor, backgroundColor);

    public static BorderStyle GetNormal(NKColor? textColor = null, NKColor? backgroundColor = null) =>
        new('│', '─', '┌', '┐', '└', '┘', textColor, backgroundColor);

    public static BorderStyle GetRounded(NKColor? textColor = null, NKColor? backgroundColor = null) =>
        new('│', '─', '╭', '╮', '╰', '╯', textColor, backgroundColor);

    public static BorderStyle GetThick(NKColor? textColor = null, NKColor? backgroundColor = null) => 
        new('┃', '━', '┏', '┓', '┗', '┛', textColor, backgroundColor);
    
    public static BorderStyle GetDouble(NKColor? textColor = null, NKColor? backgroundColor = null) => 
        new('║', '═', '╔', '╗', '╚', '╝', textColor, backgroundColor);

    public static BorderStyle GetSolid(NKColor? backgroundColor) => 
        new(' ', ' ', ' ', ' ', ' ', ' ', backgroundColor: backgroundColor);
}