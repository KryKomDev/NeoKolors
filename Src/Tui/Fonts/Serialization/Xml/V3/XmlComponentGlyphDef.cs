// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.ComponentModel;
using System.Xml.Serialization;
using NeoKolors.Common;
using NeoKolors.Extensions;

namespace NeoKolors.Tui.Fonts.Serialization.Xml.V3;

public class XmlComponentGlyphDef : XmlGlyphDef {
    
    private string? _id;
    
    [XmlIgnore] 
    public bool IsIdDefined => _id != null;
    
    [XmlAttribute("File")] 
    public string File { get; set; }
    
    [XmlIgnore] public XmlGlyphAlignPoint[]       AlignPoints       { get; set; }
    [XmlIgnore] public char[]?                    AlignPointMarkers { get; set; }
    [XmlIgnore] public XmlGlyphAlignPointReplace? AlignPointReplace { get; set; }
    [XmlIgnore] public char                       Symbol            { get; set; }
    [XmlIgnore] public XmlGlyphMask?              Mask              { get; set; }
    [XmlIgnore] public TextStyles                 Styles            { get; set; }

    [XmlAttribute("Symbol")]
    public string SymbolString {
        get => Symbol.ToString();
        set {
            if (char.TryParseEsc(value, out char c)) {
                Symbol = c;
                _id    = value;
                return;
            }
            
            NKFontSerializer.LOGGER.Error($"The string '{value}' is not valid for Symbol.");
        }
    }

    [XmlAttribute("Id")]
    [DefaultValue("")]
    public string Id {
        get => _id ?? Symbol.ToString();
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

    public XmlComponentGlyphDef() {
        File        = null!;
        AlignPoints = [];
    }
}