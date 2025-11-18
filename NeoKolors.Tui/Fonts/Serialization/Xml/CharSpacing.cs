// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.Serialization.Xml;

[XmlRoot("CharSpacing", Namespace = NKFontSchema.SCHEMA_NAMESPACE)]
public partial class CharSpacing {
    
    [XmlText]
    public int Value { get; set; }
    
    public static implicit operator int(CharSpacing charSpacing) => charSpacing.Value;
    public static implicit operator CharSpacing(int value) => new() { Value = value };
    
    public override string ToString() => Value.ToString();
}