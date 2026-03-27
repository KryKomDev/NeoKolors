// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct ListPointGapProperty : IStyleProperty<Dimension, ListPointGapProperty> {
    public Dimension Value { get; }

    public ListPointGapProperty(Dimension value) {
        Value = value;
    }
    
    public ListPointGapProperty(ListPointGapProperty source) {
        Value = source.Value;
    }

    public ListPointGapProperty() {
        Value = Dimension.Chars(1);
    }
}