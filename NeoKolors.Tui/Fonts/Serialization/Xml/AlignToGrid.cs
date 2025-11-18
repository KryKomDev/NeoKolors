// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.Serialization.Xml;

[XmlRoot("AlignToGrid", Namespace = NKFontSchema.SCHEMA_NAMESPACE)]
public partial class AlignToGrid {
    
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

    public static implicit operator bool(AlignToGrid monospace) => monospace?.Value ?? false;
    public static implicit operator AlignToGrid(bool value) => new() { Value = value };
    
    public override string ToString() => Value.ToString();
}