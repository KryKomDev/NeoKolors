// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.Serialization.Xml;

[XmlRoot("LetterHeight", Namespace = NKFontSchema.SCHEMA_NAMESPACE)]
public partial class LetterHeight {
    
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

    public LetterHeight() {
        Raw = "";
    }
}