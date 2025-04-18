//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Exceptions;

public class GlyphTooBigException : Exception {
    public static GlyphTooBigException Width(int actual) =>
        new($"Tried to create a glyph with size {actual}. Max allowed width is {byte.MaxValue}.");
    
    
    public static GlyphTooBigException Height(int actual) =>
        new($"Tried to create a glyph height size {actual}. Max allowed height is {byte.MaxValue}.");

    private GlyphTooBigException(string message) : base(message) { } 
}