// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct MaxHeightProperty : IStyleProperty<Dimension, MaxHeightProperty> {
    public Dimension Value { get; }
    
    public MaxHeightProperty(Dimension value) {
        Value = value;
    }

    public MaxHeightProperty() {
        Value = Dimension.Auto;
    }
}