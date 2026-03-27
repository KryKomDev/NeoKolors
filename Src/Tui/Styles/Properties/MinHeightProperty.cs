// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct MinHeightProperty : IStyleProperty<Dimension, MinHeightProperty> {
    public Dimension Value { get; }
    
    public MinHeightProperty(Dimension value) {
        Value = value;
    }

    public MinHeightProperty() {
        Value = Dimension.Auto;
    }
}