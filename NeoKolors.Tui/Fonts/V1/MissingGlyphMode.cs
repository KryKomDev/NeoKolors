//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.V1;

/// <summary>
/// Determines what will happen when a missing glyph is encountered.
/// </summary>
public enum MissingGlyphMode {
    
    /// <summary>
    /// A missing glyph will be substituted with the character.
    /// </summary>
    [XmlEnum("char")]
    CHAR = 0,

    /// <summary>
    /// A missing glyph is substituted with another, predefined glyph.
    /// </summary>
    [XmlEnum("glyph")]
    GLYPH = 1,
    
    /// <summary>
    /// The missing glyph will be skipped.
    /// </summary>
    [XmlEnum("skip")]
    SKIP = 2,
    
    /// <summary>
    /// An exception will be thrown.
    /// </summary>
    [XmlEnum("throw")]
    THROW = 3
}