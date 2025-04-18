//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Exceptions;

public class FontReaderException : Exception {
    private FontReaderException(string message) : base("Cannot read font file." + message) { }

    public static FontReaderException FontFileDoesNotExist(string path) =>
        new($"Font file specified by path '{path}' does not exist.");

    public static FontReaderException InvalidHeader() =>
        new("Invalid header.");
    
    public static FontReaderException InvalidFileVersion() =>
        new("Specified font file is in invalid version.");

    public static FontReaderException InvalidUnknownGlyphMode(string line) =>
        new($"Invalid Unknown Glyph Mode '{line}'.");

    public static FontReaderException InvalidUnknownGlyphModeGlyph(string actual) =>
        new($"Invalid Unknown Glyph Mode argument for Glyph. Expected a single character, got {actual}.");

    public static FontReaderException InvalidSpacings(string actual) =>
        new($"Invalid spacings inputted. Expected: <int> <int> <int>, got: {actual}.");

    public static FontReaderException InvalidLetterSpacing(Exception inner) =>
        new($"Invalid Letter Spacing. {inner}");
    
    public static FontReaderException InvalidWordSpacing(Exception inner) =>
        new($"Invalid Word Spacing. {inner}");
    
    public static FontReaderException InvalidLineSpacing(Exception inner) =>
        new($"Invalid Line Spacing. {inner}");

    public static FontReaderException InvalidLineSize(Exception inner) =>
        new($"Invalid Line Size. {inner}");

    public static FontReaderException InvalidWhitespaceMode(string actual) =>
        new($"Invalid Whitespace Mode. Expected: '<mode>' or 'mask <char>', got '{actual}' instead.");

    public static FontReaderException InvalidWhitespaceMaskCharacter(string actual) =>
        new($"Invalid Whitespace Mask character. Expected single character, got '{actual}' instead");

    public static FontReaderException InvalidOffsets(char glyph, string actual) =>
        new($"Invalid offsets for glyph '{glyph}'. Expected '<int> <int>', got '{actual}'.");

    public static FontReaderException InvalidXOffset(char glyph, Exception inner) =>
        new($"Invalid x offset for glyph '{glyph}'. {inner}");
    
    public static FontReaderException InvalidYOffset(char glyph, Exception inner) =>
        new($"Invalid y offset for glyph '{glyph}'. {inner}");
}