// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct ListPointStyleProperty : IStyleProperty<NKStyle, ListPointStyleProperty> {
    public NKStyle Value { get; }
    
    public ListPointStyleProperty(NKStyle value) {
        Value = value;
    }
    
    public ListPointStyleProperty(ListPointStyleProperty source) {
        Value = source.Value;
    }

    public ListPointStyleProperty() {
        Value = NKStyle.Default;
    }
}