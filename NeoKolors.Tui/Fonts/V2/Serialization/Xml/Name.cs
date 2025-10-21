// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;
using NeoKolors.Tui.Fonts.V2.Exceptions;

namespace NeoKolors.Tui.Fonts.V2.Serialization.Xml;

[XmlRoot("Name", Namespace = NKFontSchema.CONFIG_SCHEMA_LOCATION)]
public partial class Name {
    
    [XmlText]
    public string? Value { 
        get;
        set {
            if (value is null || !value.All(c => char.IsLetterOrDigit(c) || c is '_' or '-' or '.' or ' '))
                throw FontSerializerException.InvalidFontName(value ?? "<null>");
            
            field = value;
        } 
    }
    
    public static implicit operator string?(Name name) => name.Value;
    public static implicit operator Name(string value) => new() { Value = value };
    
    public override string? ToString() => Value;
}
