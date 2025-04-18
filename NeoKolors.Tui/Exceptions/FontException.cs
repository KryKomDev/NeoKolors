//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Exceptions;

public class FontException : Exception {
    private FontException(string message) : base(message) { }

    public static FontException InvalidUnknownGlyphMode() =>
        new FontException("Could not set up font. Tried to use 'glyph' mode without specifying the substitute.");
}