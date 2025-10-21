// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.V2.Serialization.Xml;

[XmlRoot("Ligatures", Namespace = NKFontSchema.CONFIG_SCHEMA_LOCATION)]
public partial class Ligatures {
    
    [XmlText]
    public bool Value { get; set; }
    
    public static implicit operator bool(Ligatures ligatures) => ligatures?.Value ?? true;
    public static implicit operator Ligatures(bool value) => new() { Value = value };
    
    public override string ToString() => Value.ToString();
}