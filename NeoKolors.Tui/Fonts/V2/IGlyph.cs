// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Fonts.V2;

public interface IGlyph {
    
    /// <summary>
    /// Represents a two-dimensional character grid that defines the visual representation of a glyph in a font.
    /// Null characters are used to represent transparent space within the glyph.
    /// </summary>
    /// <remarks>
    /// The grid is of variable width and height and may include null characters
    /// to represent transparent or empty space within the glyph.
    /// </remarks>
    public char?[,] Glyph { get; }

    public int Width { get; }
    public int Height { get; }

    /// <summary>
    /// Represents the vertical offset of the baseline for a glyph relative to its visual representation.
    /// </summary>
    /// <remarks>
    /// The baseline offset is used to determine how a glyph aligns vertically within a line of text.
    /// It affects the positioning of the glyph in relation to other glyphs and the overall text layout.
    /// </remarks>
    public int BaselineOffset { get; }

    /// <summary>
    /// Represents a collection of alignment points for a glyph, used to define
    /// specific character-position mappings within the glyph for alignment operations.
    /// </summary>
    /// <remarks>
    /// The alignment points are stored as a collection where each entry associates
    /// a character with a specific <see cref="Point"/> in two-dimensional space. These
    /// points enable complex glyph positioning and alignment operations, particularly
    /// in scenarios involving compound or composite glyph assembly.
    /// </remarks>
    public GlyphAlignmentPointCollection AlignPoints { get; }
}