// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Fonts.V2;

/// <summary>
/// Represents information about a ligature glyph, associating a graphical glyph representation
/// with a specific ligature string.
/// </summary>
/// <remarks>
/// The <see cref="LigatureGlyphInfo"/> struct is a lightweight, immutable representation
/// of a ligature glyph used within the NeoKolors rendering system. It provides a mapping between
/// a ligature string and its corresponding glyph representation.
/// </remarks>
/// <seealso cref="IGlyph"/>
/// <seealso cref="GlyphInfo"/>
public readonly struct LigatureGlyphInfo : IEquatable<LigatureGlyphInfo> {
    
    public IGlyph Glyph { get; }
    public string Ligature { get; }
    
    public LigatureGlyphInfo(IGlyph glyph, string ligature) {
        Glyph = glyph;
        Ligature = ligature;
    }
    
    private LigatureGlyphInfo(string ligature) {
        Ligature = ligature;
        Glyph = null!;
    }
    
    public override int GetHashCode() => Ligature.GetHashCode();

    public bool Equals(LigatureGlyphInfo other) => Glyph.Equals(other.Glyph) && Ligature == other.Ligature;
    public override bool Equals(object? obj) => obj is LigatureGlyphInfo other && Equals(other);
    public static bool operator ==(LigatureGlyphInfo left, LigatureGlyphInfo right) => left.Equals(right);
    public static bool operator !=(LigatureGlyphInfo left, LigatureGlyphInfo right) => !left.Equals(right);
}