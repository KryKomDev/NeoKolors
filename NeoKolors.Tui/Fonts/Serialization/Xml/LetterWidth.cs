// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;
#pragma warning disable CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or adding '[field: MaybeNull, AllowNull]' attributes.

namespace NeoKolors.Tui.Fonts.Serialization.Xml;

[XmlRoot("LetterWidth", Namespace = NKFontSchema.SCHEMA_NAMESPACE)]
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