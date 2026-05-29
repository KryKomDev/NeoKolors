// NeoKolors
// Copyright (c) krystof 2026

using Metriks;

namespace NeoKolors.Tui.Fonts;

public interface IGlyph {

    /// <summary>
    /// Represents a two-dimensional grid of <see cref="GlyphCell"/> objects defining
    /// the visual structure of a glyph or character.
    /// </summary>
    /// <remarks>
    /// The <c>Glyph</c> property serves as the primary representation for a glyph
    /// within the NeoKolors.Tui framework. The top-left corner of the glyph has the
    /// (0, 0) coordinate.
    /// </remarks>
    public GlyphCell[,] Glyph { get; }
    
    public int    Width  { get; }
    public int    Height { get; }
    public Size2D Size   { get; }

    /// <summary>
    /// Specifies the vertical offset of the baseline relative to the glyph's visual representation.
    /// </summary>
    /// <remarks>
    /// The <c>BaselineOffset</c> property is used to determine the vertical alignment of the glyph
    /// when rendered in relation to other glyphs within the same font or graphical context.
    /// It enables precise positioning by defining the number of units from the top of the glyph grid
    /// to its baseline.
    /// </remarks>
    public int BaselineOffset { get; }

    /// <summary>
    /// Provides a collection of <see cref="AlignPoint"/> instances used to define
    /// alignment references within a glyph, allowing precise positioning and layout calculations.
    /// </summary>
    /// <remarks>
    /// The <c>AlignPoints</c> property is a key part used for managing and accessing
    /// alignment markers associated with a glyph, facilitating operations such as alignment
    /// and combination of glyphs based on predefined reference points.
    /// </remarks>
    public AlignPointCollection AlignPoints { get; }
}