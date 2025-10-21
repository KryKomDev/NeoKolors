// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.V2.Serialization.Xml;

[XmlRoot("MapPath", Namespace = NKFontSchema.CONFIG_SCHEMA_LOCATION)]
public partial class MapPath {
    
    [XmlText]
    public string? Value { get; set; }
    
    public static implicit operator string?(MapPath mapPath) => mapPath.Value;
    public static implicit operator MapPath(string value) => new() { Value = value };
    
    public override string? ToString() => Value;
}