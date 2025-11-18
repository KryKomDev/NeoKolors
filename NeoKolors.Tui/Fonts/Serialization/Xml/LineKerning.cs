// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.Serialization.Xml;

[XmlRoot("LineKerning", Namespace = NKFontSchema.SCHEMA_NAMESPACE)]
public partial class LineKerning {
    
    [XmlText]
    public bool Value { get; set; }
    
    public static implicit operator bool(LineKerning kerning) => kerning?.Value ?? true;
    public static implicit operator LineKerning(bool value) => new() { Value = value };
    
    public override string ToString() => Value.ToString();
}