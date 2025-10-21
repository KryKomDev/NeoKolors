// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.V2.Serialization.Xml;

[XmlRoot("Monospace", Namespace = NKFontSchema.CONFIG_SCHEMA_LOCATION)]
public partial class Monospace {
    
    private bool? _field;
    
    [XmlText]
    public bool Value {
        get {
            if (_field is not null)
                return _field.Value;
            _field = false;
            return false;
        }
        set => _field = value;
    }

    public static implicit operator bool(Monospace monospace) => monospace?.Value ?? false;
    public static implicit operator Monospace(bool value) => new() { Value = value };
    
    public override string ToString() => Value.ToString();
}