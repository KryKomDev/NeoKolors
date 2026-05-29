// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.Serialization.Xml.V3;

public class XmlDefaultFontConfig {
    
    [XmlIgnore] public XmlGlyphMask?              Mask              { get; set; }
    [XmlIgnore] public XmlGlyphAlign?             Align             { get; set; }
    [XmlIgnore] public XmlGlyphAlignPointReplace? AlignPointReplace { get; set; }
    [XmlIgnore] public char[]                     AlignPointMarkers   { get; set; }
    
    [XmlElement("Mask")]
    public string MaskString {
        get => Mask?.ToString() ?? "";
        set => Mask = XmlGlyphMask.TryParse(value, out var mask) ? mask : null;
    }

    [XmlElement("Align")]
    public string AlignString {
        get => Align?.ToString() ?? "";
        set => Align = XmlGlyphAlign.TryParse(value, out var align) ? align : null;
    }

    [XmlElement("AlignPointReplace")]
    public string AlignPointReplaceString {
        get => AlignPointReplace?.ToString() ?? "";
        set => AlignPointReplace = XmlGlyphAlignPointReplace.Parse(value);
    }

    [XmlElement("AlignPointMarkers")]
    public string AlignPointMarkersString {
        get => new(AlignPointMarkers);
        set => AlignPointMarkers = value.ToCharArray();
    }

    public XmlDefaultFontConfig() {
        AlignPointMarkers = [];
    }
}