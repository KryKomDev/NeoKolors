// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.V2.Serialization.Xml;

[XmlRoot("LetterWidth", Namespace = NKFontSchema.CONFIG_SCHEMA_LOCATION)]
public partial class LetterWidth {
    
    [XmlText]
    public string Raw { 
        get;
        set {
            _value = null;
            field = value;
        } 
    }
    
    private int? _value;
    
    [XmlIgnore]
    public int Value {
        get {
            if (Equals(Raw, null))
                throw new InvalidOperationException("Value is not set.");
            if (_value is not null)
                return _value.Value;

            _value = int.Parse(Raw);
            return _value.Value;
        }
    }
}