// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.Serialization.Xml;

[XmlRoot("EmptyLineHeight", Namespace = NKFontSchema.SCHEMA_NAMESPACE)]
public partial class EmptyLineHeight {
    
    [XmlText]
    public int Value { get; set; }
    
    public static implicit operator int(EmptyLineHeight emptyLineHeight) => emptyLineHeight.Value;
}