// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.ComponentModel;
using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.Serialization.Xml.V3;

public class XmlUnderlineGlyphDef : XmlGlyphDef {
    
    [XmlIgnore] 
    public XmlGlyphMask? Mask { get; set; }

    [XmlAttribute("File")]         public string File         { get; set; }
    [XmlAttribute("Baseline")]     public int    Baseline     { get; set; }
    [XmlAttribute("AboveLetters")] public bool   AboveLetters { get; set; }
    
    [XmlAttribute("Mask")]
    [DefaultValue(null)]
    public string MaskString {
        get => Mask.ToString() ?? "null";
        set => Mask = XmlGlyphMask.TryParse(value, out var mask) ? mask.Value : null;
    }

    public XmlUnderlineGlyphDef() {
        Mask         = null;
        File         = null!;
        Baseline     = 0;
        AboveLetters = false;
    }
}