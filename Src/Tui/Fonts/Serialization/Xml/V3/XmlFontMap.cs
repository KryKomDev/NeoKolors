// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.Serialization.Xml.V3;

[XmlRoot("FontMap")]
public class XmlFontMap {
    
    [XmlElement("Component",     typeof(XmlComponentGlyphDef))]
    [XmlElement("Ligature",      typeof(XmlLigatureGlyphDef))]
    [XmlElement("Compound",      typeof(XmlCompoundGlyphDef))]
    [XmlElement("AutoCompound",  typeof(XmlAutoCompoundGlyphDef))]
    [XmlElement("Strikethrough", typeof(XmlStrikethroughGlyphDef))]
    [XmlElement("Underline",     typeof(XmlUnderlineGlyphDef))]
    public XmlGlyphDef[] Glyphs { get; set; } = [];
}