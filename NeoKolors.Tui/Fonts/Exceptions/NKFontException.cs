// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Fonts.Exceptions;

public class NKFontException : Exception {
    private NKFontException(string message) : base(message) { }
    
    public static NKFontException DuplicateGlyphSymbols(string symbol) 
        => new($"Cannot load font. Duplicate glyph symbol '{symbol}' found."); 
}