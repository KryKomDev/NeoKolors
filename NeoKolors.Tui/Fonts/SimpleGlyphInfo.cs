// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Fonts;

/// <summary>
/// Represents information about a simple glyph, including its associated character and its glyph data.
/// A simple glyph is a component glyph or an explicitly defined compound glyph.
/// </summary>
/// <remarks>
/// The <see cref="SimpleGlyphInfo"/> struct provides a mapping between a character and its glyph representation.
/// It is immutable and implements the <see cref="IEquatable{T}"/> interface for value-based equality checks.
/// </remarks>
/// <seealso cref="IGlyph"/>
/// <seealso cref="GlyphInfo"/>
public readonly struct SimpleGlyphInfo : IEquatable<SimpleGlyphInfo> {
    
    public IGlyph Glyph { get; }
    public char Character { get; }
    
    public SimpleGlyphInfo(IGlyph glyph, char character) {
        Glyph = glyph;
        Character = character;
    }

    private SimpleGlyphInfo(char character) {
        Character = character;
        Glyph = null!;
    }

    public override int GetHashCode() => Character.GetHashCode();

    public bool Equals(SimpleGlyphInfo other) => Glyph.Equals(other.Glyph) && Character == other.Character;
    public override bool Equals(object? obj) => obj is SimpleGlyphInfo other && Equals(other);
    public static bool operator ==(SimpleGlyphInfo left, SimpleGlyphInfo right) => left.Equals(right);
    public static bool operator !=(SimpleGlyphInfo left, SimpleGlyphInfo right) => !left.Equals(right);
}