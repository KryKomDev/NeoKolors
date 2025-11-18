// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.Serialization.Xml;

[XmlRoot("MapPath", Namespace = NKFontSchema.SCHEMA_NAMESPACE)]
public partial class MapPath {
    
    [XmlText]
    public string? Value { get; set; }
    
    public static implicit operator string?(MapPath mapPath) => mapPath.Value;
    public static implicit operator MapPath(string value) => new() { Value = value };
    
    public override string? ToString() => Value;
}