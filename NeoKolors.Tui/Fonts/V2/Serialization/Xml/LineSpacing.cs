// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.V2.Serialization.Xml;

[XmlRoot("LineSpacing", Namespace = NKFontSchema.CONFIG_SCHEMA_LOCATION)]
public partial class LineSpacing {
    
    [XmlText]
    public int Value { get; set; }
    
    public static implicit operator int(LineSpacing lineSpacing) => lineSpacing.Value;
    public static implicit operator LineSpacing(int value) => new() { Value = value };
    
    public override string ToString() => Value.ToString();
}