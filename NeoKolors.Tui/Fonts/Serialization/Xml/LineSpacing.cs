// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.Serialization.Xml;

[XmlRoot("LineSpacing", Namespace = NKFontSchema.SCHEMA_NAMESPACE)]
public partial class LineSpacing {
    
    [XmlText]
    public int Value { get; set; }
    
    public static implicit operator int(LineSpacing lineSpacing) => lineSpacing.Value;
    public static implicit operator LineSpacing(int value) => new() { Value = value };
    
    public override string ToString() => Value.ToString();
}