// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.ComponentModel;
using System.Xml.Serialization;
using NeoKolors.Common;
using NeoKolors.Extensions;

namespace NeoKolors.Tui.Fonts.Serialization.Xml.V3;

public class XmlAutoCompoundGlyphDef : XmlGlyphDef {
    
    [XmlIgnore] public char                 Symbol      { get; set; }
    [XmlIgnore] public XmlGlyphAlign?       Align       { get; set; }
    [XmlIgnore] public XmlGlyphAlignPoint[] AlignPoints { get; set; }
    [XmlIgnore] public TextStyles           Styles      { get; set; }
    
    [XmlAttribute("Base")]       public string Base       { get; set; }
    [XmlAttribute("IsBaseMain")] public bool   IsBaseMain { get; set; }
    
    [XmlAttribute("SecondaryAfterBase")]
    [DefaultValue(false)] 
    public bool SecondaryAfterBase { get; set; }

    [XmlAttribute("Symbol")]
    public string SymbolString {
        get => Symbol.ToString();
        set {
            if (char.TryParseEsc(value, out char c)) {
                Symbol = c;
                return;
            }
            
            NKFontSerializer.LOGGER.Error($"The string '{value}' is not valid for Symbol.");
        }
    }
    
    [XmlAttribute("Align")]
    public string AlignString {
        get => Align.ToString() ?? "none";
        set => Align = XmlGlyphAlign.TryParse(value, out var align) ? align : null;
    }

    [XmlAttribute("AlignPoints")]
    [DefaultValue("")]
    public string AlignPointsString {
        get => AlignPoints.Select(p => p.ToString()).Join(", ");
        set => AlignPoints = XmlGlyphAlignPoint.ParseArray(value);
    }

    [XmlElement("ApplicableGroup", typeof(XmlAutoCompoundApplicableGroup))]
    [XmlElement("ApplicableChars", typeof(XmlAutoCompoundApplicableChars))]
    public XmlAutoCompoundApplicable[] Applicable { get; set; } = [];
    
    [XmlAttribute("Styles")]
    [DefaultValue("NONE")]
    public string StylesString {
        get => Styles.ToString().ToLowerInvariant();
        set => Styles = Enum.Parse<TextStyles>(value.ToUpperInvariant().Replace('-', '_'));
    }
    
    public XmlAutoCompoundGlyphDef() {
        Base        = null!;
        AlignPoints = [];
    }
}