// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.Serialization.Xml;

[XmlRoot("WordSpacing", Namespace = NKFontSchema.SCHEMA_NAMESPACE)]
public partial class WordSpacing {
    
    [XmlText]
    public int Value { get; set; }
    
    public static implicit operator int(WordSpacing wordSpacing) => wordSpacing?.Value ?? 0;
    public static implicit operator WordSpacing(int value) => new() { Value = value };
    
    public override string ToString() => Value.ToString();
}