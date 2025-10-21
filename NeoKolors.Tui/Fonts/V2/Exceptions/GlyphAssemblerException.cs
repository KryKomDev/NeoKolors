// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Fonts.V2.Exceptions;

public class GlyphAssemblerException : Exception {
    public GlyphAssemblerException(string message) : base(message) { }
    
    public static GlyphAssemblerException NoMatchingAlignPoint(char c) =>
        new($"No matching align point marked as '{c}' was found.");
}