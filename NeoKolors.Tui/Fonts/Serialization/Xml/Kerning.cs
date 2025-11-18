// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.Serialization.Xml;

[XmlRoot("Kerning", Namespace = NKFontSchema.SCHEMA_NAMESPACE)]
public partial class Kerning {
    
    [XmlText]
    public bool Value { get; set; }
    
    public static implicit operator bool(Kerning kerning) => kerning?.Value ?? true;
    public static implicit operator Kerning(bool value) => new() { Value = value };
    
    public override string ToString() => Value.ToString();
}