// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.Serialization.Xml.V3;

[XmlRoot("FontConfig")]
public class XmlFontConfig {

    private string _name;
    
    public string Name {
        get => _name;
        set => _name = !string.IsNullOrEmpty(value) && Regex.IsMatch(value, @"(\w|\.|_|-|)+") 
            ? value 
            : throw new IOp();
    }
    
    public bool Ligatures     { get; set; }
    public int  Leading       { get; set; }
    public int  LetterSpacing { get; set; }
    
    [XmlElement("Monospace", typeof(XmlMonospacedFontConfig))]
    [XmlElement("Variable",  typeof(XmlVariableFontConfig))]
    public XmlFontSpacing FontSpacing { get; set; }
    
    public XmlDefaultFontConfig Defaults { get; set; }

    public XmlFontConfig() {
        _name       = null!;
        FontSpacing = null!;
        Defaults    = null!;
    }
}