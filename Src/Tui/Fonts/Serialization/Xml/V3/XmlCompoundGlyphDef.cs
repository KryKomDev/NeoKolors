// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.ComponentModel;
using System.Xml.Serialization;
using NeoKolors.Common;
using NeoKolors.Extensions;

namespace NeoKolors.Tui.Fonts.Serialization.Xml.V3;

public class XmlCompoundGlyphDef : XmlGlyphDef {
    private string? _id;
    
    [XmlIgnore] 
    public bool IsIdDefined => _id != null;

    [XmlIgnore] public char                 Symbol      { get; set; }
    [XmlIgnore] public XmlGlyphAlignPoint[] AlignPoints { get; set; }
    [XmlIgnore] public XmlGlyphAlign?       Align       { get; set; }
    [XmlIgnore] public TextStyles           Styles      { get; set; }

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
    [DefaultValue(null)]
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

    [XmlAttribute("Align")]
    public string AlignString {
        get => Align.ToString() ?? "null";
        set => Align = XmlGlyphAlign.TryParse(value, out var align) ? align : null;
    }
    
    [XmlAttribute("Main")]      public string Main      { get; set; }
    [XmlAttribute("Secondary")] public string Secondary { get; set; }

    [XmlAttribute("Styles")]
    [DefaultValue("NONE")]
    public string StylesString {
        get => Styles.ToString().ToLowerInvariant();
        set => Styles = Enum.Parse<TextStyles>(value.ToUpperInvariant().Replace('-', '_'));
    }
    
    public XmlCompoundGlyphDef() {
        AlignPoints = [];
        Main        = null!;
        Secondary   = null!;
    }

    public XmlCompoundGlyphDef(
        char                 symbol,
        XmlGlyphAlign?       align,
        TextStyles           styles,
        string               main,
        string               secondary) 
    {
        Symbol      = symbol;
        AlignPoints = [];
        Align       = align;
        Styles      = styles;
        Main        = main;
        Secondary   = secondary;
    }
}