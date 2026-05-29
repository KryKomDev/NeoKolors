// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Tui.Fonts;

public readonly record struct GlyphCell {
    public GlyphCellType Type      { get; }
    public char          Character { get; }

    private GlyphCell(GlyphCellType type, char character) {
        Type      = type;
        Character = character;
    }

    public static GlyphCell Foreground { get; } = new(GlyphCellType.FOREGROUND, '\0');
    public static GlyphCell Background { get; } = new(GlyphCellType.BACKGROUND, '\0');

    public static GlyphCell Char(char c) => new(GlyphCellType.CHARACTER, c);
    
    public static bool operator ==(GlyphCell cell, char c) => 
        cell.Type == GlyphCellType.CHARACTER && cell.Character == c;
    
    public static bool operator !=(GlyphCell cell, char c) => !(cell == c);
}