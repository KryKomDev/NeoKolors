// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Common;

public record struct AnsiChar {
    public char    Char  { get; }
    public NKStyle Style { get; }

    public AnsiChar(char c, NKStyle style) {
        Char  = c;
        Style = style;
    }
    
    public static implicit operator char   (AnsiChar ansiChar) => ansiChar.Char;
    public static implicit operator NKStyle(AnsiChar ansiChar) => ansiChar.Style;
}