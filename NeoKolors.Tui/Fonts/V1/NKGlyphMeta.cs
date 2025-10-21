//
// NeoKolors
// Copyright (c) 2025 KryKom (␕)
//

namespace NeoKolors.Tui.Fonts.V1;

/// <summary>
/// Contains information about a glyph.
/// </summary>
public readonly struct NKGlyphMeta {
        
    /// <summary>
    /// The value the glyph represents.
    /// </summary>
    public string Meaning { get; }
    
    /// <summary>
    /// Whether the glyph is a ligature.
    /// </summary>
    public bool IsLigature => Meaning.Length > 1;
    
    /// <summary>
    /// The horizontal offset of the glyph when printed. (+) -> right, (-) -> left
    /// </summary>
    public int XOffset { get; }
    
    /// <summary>
    /// The vertical offset of the glyph when printed. (+) -> down, (-) -> up
    /// </summary>
    public int YOffset { get; }

    
    public NKGlyphMeta(string meaning, int xOffset, int yOffset) {
        Meaning = meaning;
        XOffset = xOffset;
        YOffset = yOffset;
    }
}