// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.ComponentModel;
using System.Xml.Serialization;
using NeoKolors.Common;
using NeoKolors.Extensions;

namespace NeoKolors.Tui.Fonts.Serialization.Xml.V3;

public class XmlLigatureGlyphDef : XmlGlyphDef {
    
    private string? _id;
    
    [XmlIgnore] 
    public bool IsIdDefined => _id != null;
    
    [XmlAttribute("File")] 
    public string File { get; set; }
    
    [XmlIgnore] public XmlGlyphAlignPoint[]       AlignPoints       { get; set; }
    [XmlIgnore] public char[]?                    AlignPointMarkers { get; set; }
    [XmlIgnore] public XmlGlyphAlignPointReplace? AlignPointReplace { get; set; }
    [XmlIgnore] public XmlGlyphMask?              Mask              { get; set; }
    [XmlIgnore] public TextStyles                 Styles            { get; set; }

    [XmlAttribute("Symbol")]
    public string Symbol { get; set; }

    [XmlAttribute("Id")]
    [DefaultValue(null)]
    public string Id {
        get => _id ?? Symbol;
        set {
            if (!string.IsNullOrEmpty(value))
                _id = value;
        }
    }

    [XmlAttribute("AlignPoints")]
    [DefaultValue("")]
    public string AlignPointsString {
        get => AlignPoints.Select(p => p.ToString()).Join(", ");
        set => AlignPoints = XmlGlyphAlignPoint.ParseArray(value);
    }

    [XmlAttribute("AlignPointMarkers")]
    [DefaultValue("")]
    public string AlignPointMarkersString {
        get => new(AlignPointMarkers);
        set => AlignPointMarkers = value.ToCharArray();
    }

    [XmlAttribute("AlignPointReplace")]
    [DefaultValue("")]
    public string AlignPointReplaceString {
        get => AlignPointReplace.ToString() ?? "null";
        set => AlignPointReplace = XmlGlyphAlignPointReplace.Parse(value);
    }

    [XmlAttribute("Mask")]
    [DefaultValue(null)]
    public string MaskString {
        get => Mask.ToString() ?? "null";
        set => Mask = XmlGlyphMask.TryParse(value, out var mask) ? mask.Value : null;
    }
    
    [XmlAttribute("Baseline")]
    [DefaultValue(0)]
    public int Baseline { get; set; }
    
    [XmlAttribute("Styles")]
    [DefaultValue("NONE")]
    public string StylesString {
        get => Styles.ToString().ToLowerInvariant();
        set => Styles = Enum.Parse<TextStyles>(value.ToUpperInvariant().Replace('-', '_'));
    }

    public XmlLigatureGlyphDef() {
        Symbol          = null!;
        File            = null!;
        AlignPoints     = [];
        Styles          = TextStyles.NONE;
    }
}